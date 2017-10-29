using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PackManager : MonoBehaviour
{
    public List<Pack> loadedPacks;
    public List<Pack> allowedPacks;

    void Awake()
    {
        if(!Directory.Exists(Application.dataPath + "/Packs"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Packs");
        }
        SaveTemplatePack();
        LoadAllPacks();
        AddAllPacks();
    }

    public void LoadPack(string packDirectory)
    {
        string packAsString = File.ReadAllText(packDirectory);
        Pack packToLoad = JsonUtility.FromJson<Pack>(packAsString);

        if (!loadedPacks.Contains(packToLoad))
        {
            loadedPacks.Add(packToLoad);
            Debug.Log("Loaded Pack : " + packToLoad.name);
        }
    }

    public void AddPack(Pack packToAdd)
    {
        if (!allowedPacks.Contains(packToAdd))
        {
            allowedPacks.Add(packToAdd);
            Debug.Log("Added Pack : " + packToAdd.name);
        }
    }
    public void RemovePack(Pack packToRemove)
    {
        if (allowedPacks.Contains(packToRemove))
        {
            allowedPacks.Remove(packToRemove);
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

    public void SaveTemplatePack()
    {
        Pack templatePack = new Pack();
        templatePack.name = "Template Pack";
        templatePack.question = "The Topic Of The Pack";
        templatePack.responses = new string[3];
        templatePack.responses[0] = "Response 1";
        templatePack.responses[1] = "Another Response";
        templatePack.responses[2] = "You Get The Idea";
        string packAsString = JsonUtility.ToJson(templatePack);
        File.WriteAllText(Application.dataPath + "/Packs/Template.txt", packAsString);
    }
}