using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class InventorySlotData
{
    public int itemId = -1;
    public ItemType itemType;
    public bool stackable;
    public int stackMax;
    public int count = 0;
}
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventorySlotData slotData = new InventorySlotData();
    bool isDragging = false;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        if (slotData.itemId == -1)
        {
            //GameObject dropped = eventData.pointerDrag;
            //InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
        else if(slotData.itemId == inventoryItem.itemId && slotData.stackable)
        {
            InventoryItem targetItem = GetComponentInChildren<InventoryItem>();
            int totalAmount = slotData.count + inventoryItem.count;

            if (slotData.stackMax >= totalAmount)
            {
                slotData.count = totalAmount;
                //InventoryItem targetItem = GetComponentInChildren<InventoryItem>();
                targetItem.count = totalAmount;
                targetItem.RefreshCount();
                Destroy(inventoryItem.gameObject);
            }
            /*else
            {
                if (slotData.stackMax < totalAmount && slotData.count < slotData.stackMax)
                {
                    int remainingAmount = slotData.stackMax - totalAmount;
                    slotData.count = remainingAmount + totalAmount;
                    targetItem.count = remainingAmount;
                    targetItem.RefreshCount();
                    RefreshCount();
                }
            }*/
        }
    }
}
