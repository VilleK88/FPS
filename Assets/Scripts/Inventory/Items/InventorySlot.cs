using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int itemId = -1;
    public ItemType itemType;
    public bool stackable;
    public int stackMax;
    public int count = 0;

    public void OnDrop(PointerEventData eventData)
    {
        if(itemId == -1)
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
        /*else
        {
            if(stackable)
            {
                GameObject dropped = eventData.pointerDrag;
                InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
                if(itemId == inventoryItem.itemId && )
            }
        }*/
    }
}
