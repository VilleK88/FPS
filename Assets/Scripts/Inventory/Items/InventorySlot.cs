using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
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

    bool dragging = false;
    [HideInInspector] public Transform parentAfterDrag;

    RectTransform rectTransform;

    private void Start()
    {
        countText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void RefreshCount()
    {
        bool textActive = count > 1;
        if(textActive)
            countText.text = count.ToString();
        else
        {
            countText.text = "";
        }
    }

    public void UseItem()
    {
        if(item != null)
        {
            if(!dragging)
            {
                item.Use();
                if(stackable)
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
            item = null;
            itemId = -1;
            itemName = null;
            maxStack = 0;
            count = 0;
            transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0);
            itemType = ItemType.Default;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        }
    }
}
