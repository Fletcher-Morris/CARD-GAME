using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pack
{
    public string name;
    public string question;
    public string[] responses;

    public Pack()
    {
        name = "";
        question = "";
        responses = new string[0];
    }
}