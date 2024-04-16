using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Item Info")]
    public Item item;
    public int itemId;
    public int pickupItemID;
    public Image img;
    public ItemType itemType;
    public string itemName;
    public bool stackable;
    public int maxStack;
    public int count;
    public AmmoType ammoType;
    public TextMeshProUGUI countText;
    [HideInInspector] public Transform parentAfterDrag;
    bool dragging = false;
    [Header("Inventory Item Menu Info")]
    public GameObject itemMenu;
    public GameObject itemMenuMoreThanOne;
    public GameObject sliderBG;
    public Slider slider;
    [SerializeField] TextMeshProUGUI sliderText;


    public void InitializeItem()
    {
        itemId = item.itemID;
        itemType = item.itemType;
        itemName = item.itemName;
        stackable = item.stackable;
        maxStack = item.stackMax;
        img.sprite = item.icon;
        count += item.count;
        ammoType = item.ammoType;

        if(itemType == ItemType.Ammo)
        {
            InventorySlot slot = GetComponentInParent<InventorySlot>();
            slot.slotData.count = count;
            RefreshCount();
            InitializeAmmoStatus();
        }
        InitializeSlider();
    }

    public void InitializeSlider()
    {
        slider.minValue = 1;
        slider.maxValue = count;
        slider.value = 0;
        slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = $"{slider.value - 1}/{slider.maxValue}";
        });
    }

    public void InitializeAmmoStatus()
    {
        GameObject player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i].GetComponent<Weapon>().thisWeaponModel == WeaponModel.Pistol &&
                    ammoType == AmmoType.Pistol)
                    weaponSlots[i].GetComponent<Weapon>().InitializeAmmoStatus();

                if (weaponSlots[i].GetComponent<Weapon>().thisWeaponModel == WeaponModel.AssaultRifle &&
                    ammoType == AmmoType.AssaultRifle)
                    weaponSlots[i].GetComponent<Weapon>().InitializeAmmoStatus();

                if (weaponSlots[i].GetComponent<Weapon>().thisWeaponModel == WeaponModel.Shotgun &&
                    ammoType == AmmoType.Shotgun)
                    weaponSlots[i].GetComponent<Weapon>().InitializeAmmoStatus();
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
                    if(itemType == ItemType.Medpack)
                    {
                        count--;
                        GetComponentInParent<InventorySlot>().slotData.count--;
                        RefreshCount();
                        if (count <= 0)
                            RemoveItem();
                    }
                }
                else
                {
                    if(itemType == ItemType.Medpack)
                    {
                        count--;
                        if (count <= 0)
                            RemoveItem();
                    }
                }
            }
        }
    }

    void CleanSlot()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = -1;
        slot.slotData.itemName = null;
        slot.slotData.itemType = ItemType.Default;
        slot.slotData.pickupItemID = 0;
        slot.slotData.stackable = false;
        slot.slotData.stackMax = 0;
        slot.slotData.count = 0;
        slot.slotData.ammoType = AmmoType.Default;
    }

    void InitializeSlot()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = itemId;
        slot.slotData.itemName = itemName;
        slot.slotData.itemType = itemType;
        slot.slotData.pickupItemID = pickupItemID;
        slot.slotData.stackable = stackable;
        slot.slotData.stackMax = maxStack;
        slot.slotData.count = count;
        slot.slotData.ammoType = ammoType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            UseItem();
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (count <= 1)
                itemMenu.SetActive(!itemMenu.activeSelf);
            else
                itemMenuMoreThanOne.SetActive(!itemMenuMoreThanOne.activeSelf);
        }
    }

    public void RemoveItem()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = -1;
        slot.slotData.itemName = null;
        slot.slotData.itemType = ItemType.Default;
        slot.slotData.pickupItemID = pickupItemID;
        slot.slotData.stackable = false;
        slot.slotData.stackMax = 0;
        slot.slotData.count = 0;
        slot.slotData.ammoType = AmmoType.Default;
        Destroy(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemMenuMoreThanOne.activeSelf || itemMenu.activeSelf)
            return;
        CleanSlot();
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        img.raycastTarget = false;
        countText.raycastTarget = false;
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemMenuMoreThanOne.activeSelf || itemMenu.activeSelf)
            return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemMenuMoreThanOne.activeSelf || itemMenu.activeSelf)
            return;
        transform.SetParent(parentAfterDrag);
        img.raycastTarget = true;
        countText.raycastTarget = true;
        dragging = false;
        InitializeSlot();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (itemMenuMoreThanOne.activeSelf || itemMenu.activeSelf)
            return;
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        InventorySlot thisItemsSlot = GetComponentInParent<InventorySlot>();
        InventoryItem thisItem = GetComponent<InventoryItem>();

        if(inventoryItem != null)
        {
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
                    slider.maxValue = count;
                    slider.value = 0;
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
                    slider.maxValue = count;
                    slider.value = 0;
                }
            }
            else
            {
                if (thisItemsSlot.slotData.slotType == SlotType.Default)
                    SwapItems(inventoryItem, thisItem);
                else if (thisItemsSlot.slotData.slotType == SlotType.Armor && inventoryItem.itemType == ItemType.Armor)
                    SwapItems(inventoryItem, thisItem);
                else if (thisItemsSlot.slotData.slotType == SlotType.Weapon && inventoryItem.itemType == ItemType.Weapon)
                    SwapItems(inventoryItem, thisItem);
            }
        }
    }

    void SwapItems(InventoryItem item1, InventoryItem item2)
    {
        Item tempItem = item1.item;
        int tempItemId = item1.itemId;
        ItemType tempItemType = item1.itemType;
        int tempPickupItemID = item1.pickupItemID;
        string tempItemName = item1.itemName;
        bool tempStackable = item1.stackable;
        int tempMaxStack = item1.maxStack;
        int tempCount = item1.count;
        Sprite tempImg = item1.img.sprite;
        float tempSliderValue = item1.count;
        AmmoType tempAmmoType = item1.ammoType;

        item1.item = item2.item;
        item1.itemId = item2.itemId;
        item1.itemType = item2.itemType;
        item1.pickupItemID = item2.pickupItemID;
        item1.itemName = item2.itemName;
        item1.stackable = item2.stackable;
        item1.maxStack = item2.maxStack;
        item1.count = item2.count;
        item1.img.sprite = item2.img.sprite;
        item1.slider.maxValue = item2.count;
        item1.ammoType = item2.ammoType;
        item1.RefreshCount();

        item2.item = tempItem;
        item2.itemId = tempItemId;
        item2.itemType = tempItemType;
        item2.pickupItemID = tempPickupItemID;
        item2.itemName = tempItemName;
        item2.stackable = tempStackable;
        item2.maxStack = tempMaxStack;
        item2.count = tempCount;
        item2.img.sprite = tempImg;
        item2.slider.maxValue = tempSliderValue;
        item2.ammoType = tempAmmoType;
        item2.RefreshCount();

        InventorySlot slot = item2.GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = item2.itemId;
        slot.slotData.itemName = item2.itemName;
        slot.slotData.itemType = item2.itemType;
        slot.slotData.pickupItemID = item2.pickupItemID;
        slot.slotData.stackable = item2.stackable;
        slot.slotData.stackMax = item2.maxStack;
        slot.slotData.count = item2.count;
        slot.slotData.ammoType = item2.ammoType;
    }

    public void SliderOK()
    {
        Item newItem = item;
        newItem.itemID = itemId;
        newItem.icon = img.sprite;
        newItem.itemType = itemType;
        newItem.stackable = stackable;
        newItem.stackMax = maxStack;
        newItem.count = Mathf.RoundToInt(slider.value -1);
        newItem.ammoType = ammoType;
        bool addItem = InventoryManager.instance.SplitStack(newItem, pickupItemID);
        if(addItem)
        {
            count = Mathf.RoundToInt(slider.maxValue - slider.value + 1);
            RefreshCount();
        }
        sliderBG.SetActive(false);
        itemMenuMoreThanOne.SetActive(false);
    }
}
