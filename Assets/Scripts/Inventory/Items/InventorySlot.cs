using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class InventorySlotData
{
    public SlotType slotType = SlotType.Default;
    public int itemId = -1;
    public string itemName;
    public ItemType itemType;
    public bool stackable;
    public int stackMax;
    public int count = 0;
    public AmmoType ammoType;
}
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventorySlotData slotData = new InventorySlotData();

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        if(slotData.itemId == inventoryItem.itemId && slotData.stackable)
        {
            InventoryItem targetItem = GetComponentInChildren<InventoryItem>();
            int totalAmount = slotData.count + inventoryItem.count;

            if (slotData.stackMax >= totalAmount)
            {
                slotData.count = totalAmount;
                targetItem.count = totalAmount;
                targetItem.RefreshCount();
                Destroy(inventoryItem.gameObject);
            }
            else
            {
                slotData.count = slotData.stackMax;
                targetItem.count = slotData.stackMax;
                targetItem.RefreshCount();
                inventoryItem.count = totalAmount - slotData.stackMax;
                inventoryItem.RefreshCount();
            }
        }
        else
        {
            if(slotData.slotType == SlotType.Default)
                inventoryItem.parentAfterDrag = transform;
            else if(slotData.slotType == SlotType.Armor && inventoryItem.itemType == ItemType.Armor)
                inventoryItem.parentAfterDrag = transform;
            else if(slotData.slotType == SlotType.Weapon && inventoryItem.itemType == ItemType.Weapon)
                inventoryItem.parentAfterDrag = transform;
        }
    }
}

public enum SlotType
{
    Default,
    Armor,
    Weapon
}