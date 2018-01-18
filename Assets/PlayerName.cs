using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public static class PlayerName
{
    private static string m_name;
    private static bool steamInit;

    static PlayerName()
    {
        steamInit = SteamAPI.Init();
        Init();
    }

    public static void Init()
    {
        if (steamInit)
        {
            m_name = SteamFriends.GetPersonaName();
        }
        else
        {
            m_name = PlayerPrefs.GetString("PlayerName");
        }

        SetName(m_name);
    }

    public static string GetName()
    {
        return m_name;
    }

    public static void SetName(string newName)
    {
        m_name = newName;
        PlayerPrefs.SetString("PlayerName", newName);
        Debug.Log("Set Name To: " + newName);
    }
}