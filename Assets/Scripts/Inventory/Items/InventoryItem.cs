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
        InventorySlot thisItemsSlot = GetComponentInParent<InventorySlot>();
        InventoryItem thisItem = GetComponent<InventoryItem>();
        if (itemId == inventoryItem.itemId && stackable)
        {
            InventoryItem targetItem = GetComponent<InventoryItem>();
            int totalAmount = count + inventoryItem.count;

            if (maxStack >= totalAmount)
            {
                count = totalAmount;
                targetItem.count = totalAmount;
                targetItem.RefreshCount();
                thisItemsSlot.slotData.count = totalAmount;
                Destroy(inventoryItem.gameObject);
            }
            else
            {
                count = maxStack;
                targetItem.count = maxStack;
                targetItem.RefreshCount();
                inventoryItem.count = totalAmount - maxStack;
                thisItemsSlot.slotData.count = maxStack;
                inventoryItem.RefreshCount();
            }
        }
        else
        {
            SwapItems(inventoryItem, thisItem);
            Debug.Log("Swap items");
        }
    }

    void SwapItems(InventoryItem item1, InventoryItem item2)
    {
        Item tempItem = item1.item;
        int tempItemId = item1.itemId;
        ItemType tempItemType = item1.itemType;
        string tempItemName = item1.itemName;
        bool tempStackable = item1.stackable;
        int tempMaxStack = item1.maxStack;
        int tempCount = item1.count;
        Sprite tempImg = item1.img.sprite;

        item1.item = item2.item;
        item1.itemId = item2.itemId;
        item1.itemType = item2.itemType;
        item1.itemName = item2.itemName;
        item1.stackable = item2.stackable;
        item1.maxStack = item2.maxStack;
        item1.count = item2.count;
        item1.img.sprite = item2.img.sprite;

        item2.item = tempItem;
        item2.itemId = tempItemId;
        item2.itemType = tempItemType;
        item2.itemName = tempItemName;
        item2.stackable = tempStackable;
        item2.maxStack = tempMaxStack;
        item2.count = tempCount;
        item2.img.sprite = tempImg;
    }
}
