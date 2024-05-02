using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class InventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Item Info")]
    public Item item;
    public int itemId;
    public string pickupItemID;
    public Image img;
    public ItemType itemType;
    public string itemName;
    public bool stackable;
    public int maxStack;
    public int count;
    public TextMeshProUGUI countText;
    [HideInInspector] public Transform parentAfterDrag;
    public bool dragging = false;
    [Header("Inventory Item Menu")]
    public GameObject itemMenu;
    public GameObject itemMenuMoreThanOne;
    public GameObject sliderBG;
    public GameObject transparent; // when transfering item to another slot with keyboard
    public Button menuCloseButton;
    public Button menuCloseMoreThanOneButton;
    public Button splitOkButton;
    public Slider slider;
    [SerializeField] TextMeshProUGUI sliderText;
    public void InitializeItem(int newItemCount)
    {
        itemId = item.itemID;
        itemType = item.itemType;
        itemName = item.itemName;
        stackable = item.stackable;
        maxStack = item.stackMax;
        img.sprite = item.icon;
        count += newItemCount;
        if (itemType == ItemType.Ammo)
        {
            InventorySlot slot = GetComponentInParent<InventorySlot>();
            slot.slotData.count = count;
            InitializeAmmoStatus();
        }
        RefreshCount();
    }
    public void InitializeSlider()
    {
        slider.minValue = 1;
        slider.maxValue = count;
        slider.value = 0;
        slider.onValueChanged.AddListener((v) =>
        {
            UpdateSliderText(v);
        });
        UpdateSliderText(slider.value);
    }
    void UpdateSliderText(float value)
    {
        sliderText.text = $"{slider.value - 1}/{slider.maxValue}";
    }
    public void InitializeAmmoStatus() // when initializing ammo item
    {
        GameObject player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                Ammo ammo = item as Ammo;
                if (weaponSlots[i].GetComponent<Weapon>().thisWeaponModel == WeaponModel.Pistol && ammo.ammoType == AmmoType.Pistol)
                    weaponSlots[i].GetComponent<Weapon>().UpdateTotalAmmoStatus();

                if (weaponSlots[i].GetComponent<Weapon>().thisWeaponModel == WeaponModel.AssaultRifle && ammo.ammoType == AmmoType.AssaultRifle)
                    weaponSlots[i].GetComponent<Weapon>().UpdateTotalAmmoStatus();

                if (weaponSlots[i].GetComponent<Weapon>().thisWeaponModel == WeaponModel.Shotgun && ammo.ammoType == AmmoType.Shotgun)
                    weaponSlots[i].GetComponent<Weapon>().UpdateTotalAmmoStatus();
            }
        }
    }
    public void RefreshCount()
    {
        bool textActive = count > 1;
        if (textActive)
            countText.text = count.ToString();
        else
            countText.text = "";
        InitializeSlider();
    }
    public void UseItem()
    {
        if (item != null)
        {
            if (!dragging)
            {
                if (stackable)
                {
                    if (itemType == ItemType.Medpack)
                    {
                        PlayerHealth playerHealth = PlayerManager.instance.player.GetComponent<PlayerHealth>();
                        Medpack medpack = item as Medpack;
                        bool ifHealing = playerHealth.HealPlayer(medpack.healthAmount);
                        if(ifHealing)
                        {
                            count--;
                            GetComponentInParent<InventorySlot>().slotData.count--;
                            RefreshCount();
                            if (count <= 0)
                                RemoveItem();
                        }
                    }
                }
                else
                {
                    if(itemType == ItemType.Weapon)
                    {
                        InventorySlot slot = GetComponentInParent<InventorySlot>();
                        if (slot.slotData.slotType != SlotType.Weapon)
                        {
                            bool addWeaponToWeaponSlot = InventoryManager.instance.CheckIfRoomInWeaponSlots(this);
                            if (addWeaponToWeaponSlot)
                                RemoveItem();
                        }
                        else
                        {
                            bool addWeaponToInventory = InventoryManager.instance.AddInventoryItem(item, pickupItemID, count);
                            if (addWeaponToInventory)
                                RemoveItem();
                        }
                    }
                    if(itemType == ItemType.Armor)
                    {
                        InventorySlot slot = GetComponentInParent<InventorySlot>();
                        if(slot.slotData.slotType != SlotType.Armor)
                        {
                            bool addArmorToArmorSlot = InventoryManager.instance.CheckIfArmorSlotEmpty(this);
                            if (addArmorToArmorSlot)
                                RemoveItem();
                        }
                        else
                        {
                            bool addArmorToInventory = InventoryManager.instance.AddInventoryItem(item, pickupItemID, count);
                            if (addArmorToInventory)
                                RemoveItem();
                        }
                    }
                }
            }
        }
    }
    void CleanSlot()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.pickupItemID = null;
        slot.slotData.itemId = -1;
        slot.slotData.itemName = null;
        slot.slotData.itemType = ItemType.Default;
        slot.slotData.stackable = false;
        slot.slotData.stackMax = 0;
        slot.slotData.count = 0;
    }
    public void PublicRemoveItem() // removes inventoryItem and cleans the inventorySlot
    {
        CleanSlot();
        Destroy(gameObject);
    }
    void RemoveItem() // removes inventoryItem and cleans the inventorySlot and Selects that slot again
    {
        CleanSlot();
        SelectThisSlot();
        Destroy(gameObject);
    }
    public void InitializeSlot()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = itemId;
        slot.slotData.itemName = itemName;
        slot.slotData.itemType = itemType;
        slot.slotData.pickupItemID = pickupItemID;
        slot.slotData.stackable = stackable;
        slot.slotData.stackMax = maxStack;
        slot.slotData.count = count;
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
    public void SelectSplitButtons()
    {
        sliderBG.SetActive(true);
        splitOkButton.Select();
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

        if (inventoryItem != null)
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
        string tempPickupItemID = item1.pickupItemID;
        string tempItemName = item1.itemName;
        bool tempStackable = item1.stackable;
        int tempMaxStack = item1.maxStack;
        int tempCount = item1.count;
        Sprite tempImg = item1.img.sprite;
        float tempSliderValue = item1.count;
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
        item2.RefreshCount();
        InventorySlot slot = item2.GetComponentInParent<InventorySlot>();
        slot.slotData.itemId = item2.itemId;
        slot.slotData.itemName = item2.itemName;
        slot.slotData.itemType = item2.itemType;
        slot.slotData.pickupItemID = item2.pickupItemID;
        slot.slotData.stackable = item2.stackable;
        slot.slotData.stackMax = item2.maxStack;
        slot.slotData.count = item2.count;
    }
    public void CloseItemMenus()
    {
        sliderBG.SetActive(false);
        itemMenuMoreThanOne.SetActive(false);
        itemMenu.SetActive(false);
        SelectThisSlot();
    }
    public void SliderOK()
    {
        Item newItem = item;
        newItem.itemID = itemId;
        newItem.icon = img.sprite;
        newItem.itemType = itemType;
        newItem.stackable = stackable;
        newItem.stackMax = maxStack;
        int tempCount = Mathf.RoundToInt(slider.value - 1);
        if (count > 0)
        {
            bool addItem = InventoryManager.instance.SplitStack(newItem, pickupItemID, tempCount);
            if (addItem)
            {
                count = Mathf.RoundToInt(slider.maxValue - slider.value + 1);
                RefreshCount();
                GetComponentInParent<InventorySlot>().slotData.count = count;
            }
        }
        CloseItemMenus();
    }
    void SelectThisSlot()
    {
        InventorySlot thisSlot = GetComponentInParent<InventorySlot>();
        thisSlot.GetComponent<Button>().Select();
    }
}