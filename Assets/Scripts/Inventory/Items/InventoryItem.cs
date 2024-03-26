using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public Item item;
    public int itemId;
    public Image img;
    public ItemType itemType;
    public string itemName;
    public bool stackable;
    public int maxStack;
    public int count;
    public TextMeshProUGUI countText;

    bool dragging = false;

    /*private void Start()
    {
        countText = GetComponentInChildren<TextMeshProUGUI>();
        img = GetComponent<Image>();
        img.sprite = item.icon;
    }*/

    public void InitializeItem()
    {
        itemId = item.itemID;
        itemType = item.itemType;
        stackable = item.stackable;
        maxStack = item.stackMax;
        img.sprite = item.icon;
        count += 1;
    }

    public void RefreshCount()
    {
        bool textActive = count > 1;
        if (textActive)
            countText.text = count.ToString();
        else
        {
            countText.text = "";
        }
    }

    public void UseItem()
    {
        if (item != null)
        {
            if (!dragging)
            {
                item.Use();
                if (stackable)
                {
                    count--;
                    RefreshCount();
                    DestroyItem();
                }
                else
                {
                    count--;
                    DestroyItem();
                }
            }
        }
    }

    void DestroyItem()
    {
        if (count <= 0)
        {
            InventorySlot slot = GetComponentInParent<InventorySlot>();
            slot.itemId = -1;
            slot.itemType = ItemType.Default;
            slot.stackable = false;
            slot.count = 0;
            Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        }
    }
}
