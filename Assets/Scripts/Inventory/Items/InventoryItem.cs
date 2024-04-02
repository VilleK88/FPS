using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
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

    [HideInInspector] public Transform parentAfterDrag;
    bool dragging = false;

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
                    GetComponentInParent<InventorySlot>().slotData.count--;
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
            slot.slotData.itemId = -1;
            slot.slotData.itemType = ItemType.Default;
            slot.slotData.stackable = false;
            slot.slotData.stackMax = 0;
            slot.slotData.count = 0;
            Destroy(gameObject);
        }
    }

    void CleanSlot()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = -1;
        slot.slotData.itemType = ItemType.Default;
        slot.slotData.stackable = false;
        slot.slotData.stackMax = 0;
        slot.slotData.count = 0;
    }

    void InitializeSlot()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = itemId;
        slot.slotData.itemType = itemType;
        slot.slotData.stackable = stackable;
        slot.slotData.stackMax = maxStack;
        slot.slotData.count = count;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CleanSlot();
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling(); //
        img.raycastTarget = false;
        countText.raycastTarget = false;
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        img.raycastTarget = true;
        countText.raycastTarget = true;
        dragging = false;
        InitializeSlot();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        if (itemId == inventoryItem.itemId && stackable)
        {
            InventoryItem targetItem = GetComponent<InventoryItem>();
            int totalAmount = count + inventoryItem.count;

            if (maxStack == totalAmount)
            {
                count = totalAmount;
                targetItem.count = totalAmount;
                targetItem.RefreshCount();
                Destroy(inventoryItem.gameObject);
            }
            /*else
            {
                if(totalAmount < maxStack && count < maxStack)
                {
                    int remainingAmount = maxStack - totalAmount;
                    count = remainingAmount + totalAmount;
                    targetItem.count = remainingAmount;
                    targetItem.RefreshCount();
                    RefreshCount();
                }
            }*/
        }
    }
}
