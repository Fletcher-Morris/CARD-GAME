using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

public class Client : MonoBehaviour
{
    private const int MAX_CONNECTION = 100;

    private int port = 5705;

    private int hostId;
    private int webHostId;

    private int reliableChannel;
    private int unreliableChannel;

    private int outClientId;
    private int connectionId;

    private float connectionTime;
    [SerializeField]
    public bool isConnected = false;
    private bool isStarted = false;
    private byte error;
    [SerializeField]
    private string playerName;

    public GM gm;
    public PackManager packMan;

    public string connectToAddress;

    void Start()
    {
        GameObject.Find("Address Field").GetComponent<InputField>().text = Network.player.ipAddress.ToString();

        PlayerName.Init();
        playerName = PlayerName.GetName();
        GameObject.Find("Name Field").GetComponent<InputField>().text = playerName;
    }

    public void Connect()
    {
        string playerName = GameObject.Find("Name Field").GetComponent<InputField>().text;
        connectToAddress = GameObject.Find("Address Field").GetComponent<InputField>().text;
        if(playerName == "")
        {
            Debug.LogWarning("You must have a name!");
            playerName = PlayerName.GetName();
        }

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        hostId = NetworkTransport.AddHost(topo, 0);
        connectionId = NetworkTransport.Connect(hostId, connectToAddress, port, 0, out error);

        connectionTime = Time.time;
        isConnected = true;

        packMan.ClearPacks();

        Debug.Log("Started Client! : Error " + error + " : Port " + port);
    }

    void Update()
    {
        if (!isConnected)
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
            case NetworkEventType.DataEvent:       //3

                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                Debug.Log("Recieving : " + msg);

                string[] splitData = msg.Split('|');

                switch (splitData[0])
                {
                    case "ASKNAME":
                        OnAskName(splitData);
                        break;

                    case "CNN":
                        SpawnPlayer(splitData[1], int.Parse(splitData[2]));
                        break;

                    case "DC":
                        break;

                    case "PACKSINUSE":
                        packMan.ClearPacks();
                        packMan.RecievePacksInUse(splitData[1]);
                        break;

                    default:
                        Debug.LogWarning("Recieved invalid message : " + msg);
                        break;
                }

                break;
        }
    }

    public void SendReliable(string message)
    {
        Send(message, reliableChannel);
    }
    public void SendUnreliable(string message)
    {
        Send(message, unreliableChannel);
    }
    private void Send(string message, int channelId)
    {
        Debug.Log("Sending : " + message);
        byte[] msg = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
    }

    private void OnAskName(string[] data)
    {
        outClientId = int.Parse(data[1]);

        SendReliable("NAMEIS|" + playerName);

        for(int i = 2; i < data.Length - 1; i++)
        {
            string[] d = data[i].Split('%');

            SpawnPlayer(d[0],int.Parse(d[1]));
        }
    }

    private void SpawnPlayer(string playerName, int cnnId)
    {

    }
}