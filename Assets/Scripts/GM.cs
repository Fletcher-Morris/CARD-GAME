using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public Server server;
    public Client client;
    public PackManager packManager;

    public GameObject blackCardPrefab;
    public GameObject whiteCardPrefab;
    public GameObject deckCardPrefab;

    public bool isServer = false;

    public GameObject cardDeckPanel;
    public int cardsPerDeck = 9;
    public float rotationAtEdges = 30.0f;
    public Vector2 cardSeparation = new Vector2(100f, 150f);
    public AnimationCurve yPosCurve;

    void Start()
    {
        for(int i = 0; i < cardsPerDeck; i++)
        {
            GameObject newCard = Instantiate(deckCardPrefab, cardDeckPanel.transform);
            int n = Random.Range(0, 1000000);
            newCard.GetComponent<CardData>().card.text = n.ToString();
            newCard.GetComponent<CardData>().SetText();
        }

        OrganiseCards();
    }

    void Update()
    {

    }

    public void OrganiseCards()
    {
        int cardCount = cardDeckPanel.transform.childCount;
        float xOffset = (cardCount / 2) * cardSeparation.x;

        for(int i = 0; i < cardCount; i++)
        {
            RectTransform child = cardDeckPanel.transform.GetChild(i).GetComponent<RectTransform>();

            float j;
            float y = ((float)i) / ((float)cardCount - 1);

            if (IsOdd(cardCount))
            {
                j = ((float)i + 0.5f) / (float)cardCount;
            }
            else
            {
                j = ((float)i - 0.5f) / (float)cardCount;
            }

            float lerpFactor = Mathf.Lerp(rotationAtEdges, -rotationAtEdges, y);
            child.anchoredPosition = new Vector3((i * cardSeparation.x) - xOffset, (yPosCurve.Evaluate(y) * cardSeparation.y) + 75, 0);
            child.rotation = Quaternion.Euler(new Vector3(0, 0, lerpFactor));
        }
    }

    public bool IsOdd(int value)
    {
        return value % 2 != 0;
    }
}