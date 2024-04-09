using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

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

    public int ammoAmount;
    public int maxAmmo;
    public AmmoType ammoType;

    public TextMeshProUGUI countText;

    [HideInInspector] public Transform parentAfterDrag;
    bool dragging = false;

    [SerializeField] GameObject removeItem;


    public void InitializeItem()
    {
        itemId = item.itemID;
        itemType = item.itemType;
        itemName = item.itemName;
        stackable = item.stackable;
        maxStack = item.stackMax;
        img.sprite = item.icon;
        count += 1;

        ammoAmount = item.ammoAmount;
        maxAmmo = item.maxAmmo;
        ammoType = item.ammoType;

        if(itemType == ItemType.Ammo)
        {
            InitializeAmmoStatus();
        }
    }

    void InitializeAmmoStatus()
    {
        GameObject player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i].GetComponent<Weapon>().thisWeaponModel == WeaponModel.Pistol &&
                    ammoType == AmmoType.Pistol)
                {
                    weaponSlots[i].GetComponent<Weapon>().InitializeAmmoStatus();
                }
            }
        }
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

    public void DestroyItem()
    {
        if (count <= 0)
        {
            InventorySlot slot = GetComponentInParent<InventorySlot>();
            slot.slotData.itemId = -1;
            slot.slotData.itemType = ItemType.Default;
            slot.slotData.stackable = false;
            slot.slotData.stackMax = 0;
            slot.slotData.count = 0;

            slot.slotData.ammoAmount = 0;
            slot.slotData.maxAmmo = 0;
            slot.slotData.ammoType = AmmoType.Default;

            Destroy(gameObject);
        }
        
        if(ammoAmount <= 0)
        {
            InventorySlot slot = GetComponentInParent<InventorySlot>();
            slot.slotData.itemId = -1;
            slot.slotData.itemType = ItemType.Default;
            slot.slotData.stackable = false;
            slot.slotData.stackMax = 0;
            slot.slotData.count = 0;

            slot.slotData.ammoAmount = 0;
            slot.slotData.maxAmmo = 0;
            slot.slotData.ammoType = AmmoType.Default;

            Destroy(gameObject);
        }
    }

    void CleanSlot()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = -1;
        slot.slotData.itemName = null;
        slot.slotData.itemType = ItemType.Default;
        slot.slotData.stackable = false;
        slot.slotData.stackMax = 0;
        slot.slotData.count = 0;

        slot.slotData.ammoAmount = 0;
        slot.slotData.maxAmmo = 0;
        slot.slotData.ammoType = AmmoType.Default;
    }

    void InitializeSlot()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = itemId;
        slot.slotData.itemName = itemName;
        slot.slotData.itemType = itemType;
        slot.slotData.stackable = stackable;
        slot.slotData.stackMax = maxStack;
        slot.slotData.count = count;

        slot.slotData.ammoAmount = ammoAmount;
        slot.slotData.maxAmmo = maxAmmo;
        slot.slotData.ammoType = ammoType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            removeItem.SetActive(true);
        }
    }

    public void RemoveItem()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = -1;
        slot.slotData.itemName = null;
        slot.slotData.itemType = ItemType.Default;
        slot.slotData.stackable = false;
        slot.slotData.stackMax = 0;
        slot.slotData.count = 0;

        slot.slotData.ammoAmount = 0;
        slot.slotData.maxAmmo = 0;
        slot.slotData.ammoType = AmmoType.Default;

        Destroy(gameObject);
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
            if(thisItemsSlot.slotData.slotType == SlotType.Default)
                SwapItems(inventoryItem, thisItem);
            else if(thisItemsSlot.slotData.slotType == SlotType.Armor && inventoryItem.itemType == ItemType.Armor)
                SwapItems(inventoryItem, thisItem);
            else if(thisItemsSlot.slotData.slotType == SlotType.Weapon && inventoryItem.itemType == ItemType.Weapon)
                SwapItems(inventoryItem, thisItem);
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

        int tempAmmoAmount = item1.ammoAmount;
        int tempMaxAmmo = item1.maxAmmo;
        AmmoType tempAmmoType = item1.ammoType;

        item1.item = item2.item;
        item1.itemId = item2.itemId;
        item1.itemType = item2.itemType;
        item1.itemName = item2.itemName;
        item1.stackable = item2.stackable;
        item1.maxStack = item2.maxStack;
        item1.count = item2.count;
        item1.img.sprite = item2.img.sprite;

        item1.ammoAmount = item2.ammoAmount;
        item1.maxAmmo = item2.maxAmmo;
        item1.ammoType = item2.ammoType;

        item1.RefreshCount();

        item2.item = tempItem;
        item2.itemId = tempItemId;
        item2.itemType = tempItemType;
        item2.itemName = tempItemName;
        item2.stackable = tempStackable;
        item2.maxStack = tempMaxStack;
        item2.count = tempCount;
        item2.img.sprite = tempImg;

        item2.ammoAmount = tempAmmoAmount;
        item2.maxAmmo = tempMaxAmmo;
        item2.ammoType = tempAmmoType;

        item2.RefreshCount();

        InventorySlot slot = item2.GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = item2.itemId;
        slot.slotData.itemName = item2.itemName;
        slot.slotData.itemType = item2.itemType;
        slot.slotData.stackable = item2.stackable;
        slot.slotData.stackMax = item2.maxStack;
        slot.slotData.count = item2.count;

        slot.slotData.ammoAmount = item2.ammoAmount;
        slot.slotData.maxAmmo = item2.maxAmmo;
        slot.slotData.ammoType = item2.ammoType;
    }
}
