using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Item item;
    public int itemId = -1;
    public Sprite icon;
    public ItemType itemType;
    public string itemName;
    public bool stackable;
    public int maxStack;
    public int count;
    TextMeshProUGUI countText;

    private void Start()
    {

        countText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void RefreshCount()
    {
        bool textActive = count > 1;
        if(textActive)
            countText.text = count.ToString();
    }
}
