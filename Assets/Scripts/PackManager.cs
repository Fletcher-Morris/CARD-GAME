using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PackManager : MonoBehaviour
{
    public bool debug = false;
    public List<Pack> loadedPacks;
    public List<Pack> addedPacks;

    public GameObject packInfoPrefab;
    public GameObject loadedPacksPanel;
    public GameObject addedPacksPanel;

    public Server server;
    public Client client;

    void Start()
    {
        loadedPacksPanel = GameObject.Find("Loaded Packs Panel");

        if (!Directory.Exists(Application.dataPath + "/Packs"))
        {
            if (debug)
            {
                Debug.LogWarning("Pack directory does not exist, creating a new one.");
            }
            Directory.CreateDirectory(Application.dataPath + "/Packs");
        }
        SaveTemplatePack();
        LoadAllPacks();
    }

    public void LoadPack(string packDirectory)
    {
        string packAsString = File.ReadAllText(packDirectory);
        Pack packToLoad = JsonUtility.FromJson<Pack>(packAsString);

        if (!loadedPacks.Contains(packToLoad))
        {
            if (packToLoad.name == "Template Pack")
                return;

            loadedPacks.Add(packToLoad);

            GameObject packUI = Instantiate(packInfoPrefab, loadedPacksPanel.transform);
            PackUiController controller = packUI.GetComponent<PackUiController>();
            controller.packData = packToLoad;
            controller.packNameText.text = packToLoad.name;

            if (debug)
            {
                Debug.Log("Loaded Pack : " + packToLoad.name);
            }
        }
    }

    public void AddPack(Pack packToAdd)
    {
        if (!addedPacks.Contains(packToAdd))
        {
            addedPacks.Add(packToAdd);

            if (debug)
            {
                Debug.Log("Added Pack : " + packToAdd.name);
            }

            if (server.isStarted)
            {
                SendPacksInUse();
            }
        }
    }
    public void RemovePack(Pack packToRemove)
    {
        if (addedPacks.Contains(packToRemove))
        {
            addedPacks.Remove(packToRemove);

            Debug.Log("Removed Pack : " + packToRemove.name);

            if (server.isStarted)
            {
                SendPacksInUse();
            }
        }
    }
    public void AddAllPacks()
    {
        foreach(Pack pack in loadedPacks)
        {
            AddPack(pack);
        }
    }

    public void LoadAllPacks()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/Packs");

        foreach(string file in files)
        {
            if (!file.Contains(".meta"))
            {
                LoadPack(file);
            }
        }
    }

    public void ClearPacks()
    {
        loadedPacks = new List<Pack>();
        addedPacks = new List<Pack>();

        RemoveChildrenOf(loadedPacksPanel.transform);
        RemoveChildrenOf(addedPacksPanel.transform);
    }

    public void RefreshPacks()
    {
        if (client.isConnected)
        {
            client.SendReliable("PACKSREQUEST|");
        }
        else
        {
            ClearPacks();
            LoadAllPacks();

            if (server.isStarted)
            {
                SendPacksInUse();
            }
        }
    }

    public void SaveTemplatePack()
    {
        string n = Mathf.RoundToInt(Random.Range(1000000, 9999999)).ToString();

        Pack templatePack = new Pack();
        templatePack.name = n;
        templatePack.calls = new string[1];
        templatePack.calls[0] = "The Topic Of The Pack";
        templatePack.responses = new string[4];
        templatePack.responses[0] = "Response 1";
        templatePack.responses[1] = "Another Response";
        templatePack.responses[2] = "You Get The Idea";
        templatePack.responses[3] = "A Pack Must Have At Least 16 Of These";
        string packAsString = JsonUtility.ToJson(templatePack);
        File.WriteAllText(Application.dataPath + "/Packs/" + n + ".txt", packAsString);
    }

    public void SendPacksInUse()
    {
        string msg = "PACKSINUSE|";
        foreach(Pack pack in addedPacks)
        {
            msg += pack.name;
            msg += "%";
        }
        msg = msg.Trim('%');
        msg += "|";

        server.SendReliable(msg);
    }

    public void RecievePacksInUse(string msg)
    {
        ClearPacks();
        string[] splitdata = msg.Split('%');
        foreach(string data in splitdata)
        {
            if (data == "")
                return;

            Pack newPack = new Pack();
            newPack.name = data;

            GameObject packUI = Instantiate(packInfoPrefab, addedPacksPanel.transform);
            PackUiController controller = packUI.GetComponent<PackUiController>();
            controller.packData = newPack;
            controller.packNameText.text = newPack.name;
            controller.isAdded = true;
            controller.packButton.gameObject.SetActive(false);
        }
    }

    public void RemoveChildrenOf(Transform parent)
    {
        for(int i = parent.childCount - 1; i >= 0; i--)
        {
            if(parent.childCount >= 1)
            {
                GameObject child = parent.GetChild(i).gameObject;
                Destroy(child);
            }
        }
    }
}