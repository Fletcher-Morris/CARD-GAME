using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 100;

    private int port = 5705;

    private int hostId;
    private int webHostId;

    public int reliableChannel;
    public int unreliableChannel;
    
    public bool isStarted = false;
    private byte error;

    public GM gm;
    public PackManager packMan;

    [SerializeField]
    private List<ServerClient> clients = new List<ServerClient>();

    public List<Card> unplayedCards;
    public List<Card> playedCards;

    [SerializeField]
    private string hostName;

    public void ChangeName(InputField field)
    {
        hostName = field.text;
    }

    public void StartServer()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        hostId = NetworkTransport.AddHost(topo, port, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, port, null);

        isStarted = true;

        Debug.Log("Started Server! : Error " + error + " : Port " + port);

        CreateHostPlayer();
    }

    void CreateHostPlayer()
    {
        PlayerName.SetName(hostName);

        ServerClient c = new ServerClient();
        c.connectionId = 0;
        c.playerName = PlayerName.GetName();
        clients.Add(c);

        UpdateClientsList();
    }

    void Update()
    {
        if (!isStarted)
            return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.ConnectEvent:
                Debug.Log("Player " + connectionId + " has connected");
                OnConnection(connectionId);
                break;

            case NetworkEventType.DataEvent:

                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                Debug.Log("Recieving from " + connectionId + " : " + msg);

                string[] splitData = msg.Split('|');

                switch (splitData[0])
                {
                    case "NAMEIS":
                        OnNameIs(connectionId, splitData[1]);
                        break;

                    case "PACKSREQUEST":
                        packMan.SendPacksInUse();
                        break;

                    case "CNN":
                        break;

                    case "DC":
                        OnDisconnect(connectionId);
                        break;

                    default:
                        Debug.LogWarning("Recieved invalid message : " + msg);
                        break;
                }

                break;

            case NetworkEventType.DisconnectEvent:
                OnDisconnect(connectionId);
                break;
        }
    }

    private void OnConnection(int cnnId)
    {
        ServerClient c = new ServerClient();
        c.connectionId = cnnId;
        c.playerName = "TEMPNAME";
        clients.Add(c);

        string msg = "ASKNAME|" + cnnId + "|";
        foreach(ServerClient sc in clients)
        {
            msg += sc.playerName + "%" + sc.connectionId + "|";
        }
        msg = msg.Trim('|');

        Send(msg, reliableChannel, cnnId);

        packMan.SendPacksInUse();
    }


    public void SendReliable(string message)
    {
        Send(message, reliableChannel, clients);
    }
    public void SendUnreliable(string message)
    {
        Send(message, unreliableChannel, clients);
    }
    public void Send(string message, int channelId, int cnnId)
    {
        List<ServerClient> c = new List<ServerClient>();
        c.Add(clients.Find(x => x.connectionId == cnnId));
        Send(message, channelId, c);
    }
    private void Send(string message, int channelId,List<ServerClient> c)
    {
        Debug.Log("Sending : " + message);
        byte[] msg = Encoding.Unicode.GetBytes(message);

        foreach(ServerClient sc in c)
        {
            NetworkTransport.Send(hostId, sc.connectionId, channelId, msg, message.Length * sizeof(char), out error);
        }
    }


    private void OnNameIs(int cnnId, string name)
    {
        clients.Find(x => x.connectionId == cnnId).playerName = name;
        SendReliable("CNN|" + name + "|" + cnnId);
        UpdateClientsList();
    }

    public void OnDisconnect(int cnnId)
    {
        if (clients.Find(x => x.connectionId == cnnId) != null)
        {
            Debug.Log("Player Disconnected: " + clients.Find(x => x.connectionId == cnnId).playerName);
            clients.Remove(clients.Find(x => x.connectionId == cnnId));
            SendReliable("DC|" + cnnId);
            UpdateClientsList(); 
        }
    }



    void UpdateClientsList()
    {
        string playerListString = "";
        foreach (ServerClient c in clients)
        {
            playerListString += c.playerName + "\n";
        }
        GameObject.Find("Connected Players Text").GetComponent<Text>().text = playerListString;
    }
}

[System.Serializable]
class ServerClient
{
    public int connectionId;
    public string playerName;
}