using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    TextMeshProUGUI countText;

    bool dragging = false;
    [HideInInspector] public Transform parentAfterDrag;

    RectTransform rectTransform;

    private void Start()
    {
        countText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void InitializeItem(Item newItem)
    {
        item = newItem;
        itemId = newItem.itemID;
        itemType = newItem.itemType;
        stackable = newItem.stackable;
        maxStack = newItem.stackMax;
        count = 1;
        img.sprite = newItem.icon;
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
            /*item = null;
            itemId = -1;
            itemName = null;
            maxStack = 0;
            count = 0;
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(0, 0, 0, 0);
            itemType = ItemType.Default;*/
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
