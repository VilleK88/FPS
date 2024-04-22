using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class EquipmentSlot : InventorySlot
{
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }
    void OnButtonClicked()
    {
        InventoryItem itemInThisSlot = GetComponentInChildren<InventoryItem>();
        if (!InventoryManager.instance.closed)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (itemInThisSlot != null)
                    itemInThisSlot.UseItem();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (itemInThisSlot != null)
                {
                    if (itemInThisSlot.count <= 1)
                    {
                        itemInThisSlot.itemMenu.SetActive(!itemInThisSlot.itemMenu.activeSelf);
                        itemInThisSlot.menuCloseButton.Select();
                    }
                    else
                    {
                        itemInThisSlot.itemMenuMoreThanOne.SetActive(!itemInThisSlot.itemMenuMoreThanOne.activeSelf);
                        itemInThisSlot.menuCloseMoreThanOneButton.Select();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (itemInThisSlot != null && InventoryManager.instance.tempInventoryItem == null)
                {
                    InventoryManager.instance.MakeTempInventoryItemForTransfer(itemInThisSlot); // clone the item for transfer
                    itemInThisSlot.transparent.SetActive(true);
                }
                else if (InventoryManager.instance.tempInventoryItem != null && itemInThisSlot == null)
                {
                    InventorySlot thisSlot = GetComponent<InventorySlot>();
                    if (thisSlot != null && InventoryManager.instance.tempInventoryItem.itemType == ItemType.Weapon && thisSlot.slotData.slotType == SlotType.Weapon)
                        TransferItemToAnotherEmptySlot();
                    else if(thisSlot != null && InventoryManager.instance.tempInventoryItem.itemType == ItemType.Armor && thisSlot.slotData.slotType == SlotType.Armor)
                        TransferItemToAnotherEmptySlot();
                }
                else if (InventoryManager.instance.tempInventoryItem != null && itemInThisSlot != null)
                {
                    if(InventoryManager.instance.tempInventoryItem.itemType == ItemType.Weapon &&
                        itemInThisSlot.itemType == ItemType.Weapon)
                        SwapItems(itemInThisSlot);
                    else if(InventoryManager.instance.tempInventoryItem.itemType == ItemType.Armor &&
                        itemInThisSlot.itemType == ItemType.Armor)
                        SwapItems(itemInThisSlot);
                }
            }
        }
    }
    void TransferItemToAnotherEmptySlot()
    {
        InventoryItem newItem = Instantiate(InventoryManager.instance.tempInventoryItem, this.transform);
        newItem.InitializeSlot();
        CloseTransparentBG(newItem);
        InventoryManager.instance.tempInventoryItem.PublicRemoveItem();
    } // inventory keyboard use
    void SwapItems(InventoryItem itemInThisSlot)
    {
        InventoryItem newItem = Instantiate(InventoryManager.instance.tempInventoryItem, this.transform);
        InventoryItem secondNewItem = Instantiate(itemInThisSlot, InventoryManager.instance.tempInventoryItem.GetComponentInParent<InventorySlot>().transform);
        itemInThisSlot.PublicRemoveItem();
        InventoryManager.instance.tempInventoryItem.PublicRemoveItem();
        CloseTransparentBG(newItem);
        newItem.InitializeSlot();
        secondNewItem.InitializeSlot();
    } // inventory keyboard use
}