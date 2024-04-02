using System;
using System.Collections;
using System.Collections.Generic;
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

    public void OnDrop(PointerEventData eventData)
    {
        if(slotData.itemId == -1)
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
    }
}
