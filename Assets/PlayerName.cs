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
            PlayerPrefs.SetString("PlayerName", m_name);
        }
        else
        {
            m_name = PlayerPrefs.GetString("PlayerName");
        }
    }

    public static string GetName()
    {
        if (steamInit)
        {
            m_name = SteamFriends.GetPersonaName();
            PlayerPrefs.SetString("PlayerName", m_name);
        }
        else
        {
            m_name = PlayerPrefs.GetString("PlayerName");
        }

        return m_name;
    }
}