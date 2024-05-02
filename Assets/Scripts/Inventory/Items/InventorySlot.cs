using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[Serializable]
public class InventorySlotData
{
    public SlotType slotType = SlotType.Default;
    public int itemId = -1;
    public string pickupItemID;
    public string itemName;
    public ItemType itemType;
    public bool stackable;
    public int stackMax;
    public int count = 0;
}
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventorySlotData slotData = new InventorySlotData();
    public void InitializeSlot() // for new input system
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }
    void OnButtonClicked() // inventory keyboard use
    {
        InventoryItem itemInThisSlot = GetComponentInChildren<InventoryItem>();
        if(!InventoryManager.instance.closed)
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
                    TransferItemToAnotherEmptySlot();
                else if(InventoryManager.instance.tempInventoryItem != null && itemInThisSlot != null)
                {
                    if (!InventoryManager.instance.tempInventoryItem.stackable && itemInThisSlot.stackable)
                    {
                        InventorySlot tempSlot = InventoryManager.instance.tempInventoryItem.GetComponentInParent<InventorySlot>();
                        if (tempSlot.slotData.slotType == SlotType.Default)
                            SwapItems(itemInThisSlot);
                    }
                    else if (InventoryManager.instance.tempInventoryItem.stackable && !itemInThisSlot.stackable)
                        SwapItems(itemInThisSlot);
                    else
                    {
                        if (InventoryManager.instance.tempInventoryItem.itemId == itemInThisSlot.itemId)
                        {
                            if (InventoryManager.instance.tempInventoryItem != itemInThisSlot)
                                AddToStack(itemInThisSlot, InventoryManager.instance.tempInventoryItem);
                        }
                        else
                            SwapItems(itemInThisSlot);
                    }
                }
            }
        }
    } // inventory keyboard use
    public void CloseTransparentBG(InventoryItem closeThisInvItemsTranspBG)
    {
        closeThisInvItemsTranspBG.transparent.SetActive(false);
    }
    void AddToStack(InventoryItem firstInventoryItem, InventoryItem secondInventoryItem)
    {
        int totalAmount = firstInventoryItem.count + secondInventoryItem.count;

        if (firstInventoryItem.maxStack >= totalAmount)
        {
            firstInventoryItem.count = totalAmount;
            secondInventoryItem.count = totalAmount;
            secondInventoryItem.RefreshCount();
            slotData.count = totalAmount;
            firstInventoryItem.slider.maxValue = firstInventoryItem.count;
            firstInventoryItem.slider.value = 0;
            firstInventoryItem.RefreshCount();
            firstInventoryItem.InitializeSlider();
            Destroy(secondInventoryItem.gameObject);
        }
        else
        {
            firstInventoryItem.count = firstInventoryItem.maxStack;
            secondInventoryItem.count = firstInventoryItem.maxStack;
            secondInventoryItem.count = totalAmount - firstInventoryItem.maxStack;
            slotData.count = firstInventoryItem.maxStack;
            firstInventoryItem.slider.maxValue = firstInventoryItem.count;
            firstInventoryItem.slider.value = 0;
            firstInventoryItem.RefreshCount();
            secondInventoryItem.RefreshCount();
            firstInventoryItem.InitializeSlider();
            secondInventoryItem.InitializeSlider();
            firstInventoryItem.GetComponentInParent<InventorySlot>().slotData.count = firstInventoryItem.count;
            secondInventoryItem.GetComponentInParent<InventorySlot>().slotData.count = secondInventoryItem.count;
            CloseTransparentBG(secondInventoryItem);
            InventoryManager.instance.tempInventoryItem = null;
        }
    } // inventory keyboard use
    void TransferItemToAnotherEmptySlot()
    {
        InventoryItem newItem = Instantiate(InventoryManager.instance.tempInventoryItem, this.transform);
        newItem.count = InventoryManager.instance.tempInventoryItem.GetComponentInParent<InventorySlot>().slotData.count;
        newItem.InitializeSlot();
        newItem.InitializeSlider();
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
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        if(dropped != null && inventoryItem != null)
        {
            if (slotData.itemId == inventoryItem.itemId && slotData.stackable)
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
                if (slotData.slotType == SlotType.Default && slotData.itemId == -1)
                    inventoryItem.parentAfterDrag = transform;
                else if (slotData.slotType == SlotType.Armor && inventoryItem.itemType == ItemType.Armor && slotData.itemId == -1)
                    inventoryItem.parentAfterDrag = transform;
                else if (slotData.slotType == SlotType.Weapon && inventoryItem.itemType == ItemType.Weapon && slotData.itemId == -1)
                    inventoryItem.parentAfterDrag = transform;
            }
        }
    } // inventory mouse use
}
public enum SlotType
{
    Default,
    Armor,
    Weapon
}