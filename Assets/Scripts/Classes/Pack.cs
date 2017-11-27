using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pack
{
    public string name;
    public string[] calls;
    public string[] responses;

    public Pack()
    {
        name = "";
        calls = new string[0];
        responses = new string[0];
    }
}