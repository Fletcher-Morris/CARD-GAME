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

    private bool isServer = false;

    public void StartGameAsServer()
    {
        
    }
}