using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackUiController : MonoBehaviour
{
    public Text packNameText;
    public Button packButton;

    public GameObject addedPacksPanel;
    public GameObject loadedPacksPanel;

    public PackManager packManager;

    public Pack packData;

    public Sprite tickSprite;
    public Sprite crossSprite;

    public bool isAdded = false;

    void Start()
    {
        addedPacksPanel = GameObject.Find("Added Packs Panel");
        loadedPacksPanel = GameObject.Find("Loaded Packs Panel");
        packManager = GameObject.Find("Pack Manager").GetComponent<PackManager>();
    }

    public void ToggleActive()
    {
        isAdded = !isAdded;

        if (isAdded)
        {
            transform.SetParent(addedPacksPanel.transform);
            packManager.AddPack(packData);
            packButton.image.sprite = crossSprite;
        }
        else
        {
            transform.SetParent(loadedPacksPanel.transform);
            packManager.RemovePack(packData);
            packButton.image.sprite = tickSprite;
        }
    }
}