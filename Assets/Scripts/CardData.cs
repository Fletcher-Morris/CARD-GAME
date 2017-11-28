using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardData : MonoBehaviour
{
    public Card card;
    public Text text;

    public void SetText()
    {
        text.text = card.text;
    }
}

[System.Serializable]
public class Card
{
    public string text;
    public int uniqueId;
    public bool isBlank;
}