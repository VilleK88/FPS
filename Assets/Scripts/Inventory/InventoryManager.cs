using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion
    public Dictionary<int, InventoryItem> inventory = new Dictionary<int, InventoryItem>();
    public Item[] itemsDatabase; // where the inventory gets the item scriptable objects when loading the game
    public GameObject[] weaponPrefabs;
    public InventorySlot slotPrefab;
    public InventoryItem inventoryItem; // prefab
    public Transform inventoryTransform; // where the slotPrefabs are instantiated.
    public InventorySlot[] inventorySlotsUI; // table where the slotPrefabs are put.
    public EquipmentSlot[] equipmentSlotsUI;
    [SerializeField] Animator inventoryAnim; // inventory screen
    [SerializeField] Animator equipmentAnim; // equipment screen
    public bool closed = true; // inventory ui closed or not
    public GameObject player;
    public GameObject middlePoint; // crosshair
    public List<int> weaponIDsList = new List<int>();
    public Button selectSlot; // for keyboard use
    public InventoryItem tempInventoryItem; // for keyboard use
    private void Start()
    {
        inventorySlotsUI = new InventorySlot[20];
        for(int i = 0; i < 20; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, inventoryTransform);
            inventorySlotsUI[i] = slot;
            slot.InitializeSlot();
        }
        if(GameManager.instance.loadInventory)
        {
            LoadInventorySlotData();
            AddSavedInventorySlotData();
            WeaponCollected();
            LoadHowManyBulletsLeftInMagazine();
            GameManager.instance.loadInventory = false;
        }
        else
            ClearInventory();
        selectSlot = inventorySlotsUI[0].GetComponent<Button>();
        selectSlot.Select();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            if (closed)
                OpenInventory();
            else
                CloseInventory();
        }
        if (!closed)
            return;
        EquipWeapon();
    }
    public void MakeTempInventoryItemForTransfer(InventoryItem inventoryItemInTransfer)
    {
        tempInventoryItem = inventoryItemInTransfer;
    }
    public bool CheckIfArmorSlotEmpty(InventoryItem newArmorItem)
    {
        InventoryItem armorItem = equipmentSlotsUI[0].GetComponentInChildren<InventoryItem>();
        if (armorItem == null)
        {
            InventoryItem thisArmorItem = Instantiate(inventoryItem, equipmentSlotsUI[0].transform);
            thisArmorItem.item = newArmorItem.item;
            thisArmorItem.itemId = newArmorItem.itemId;
            thisArmorItem.pickupItemID = newArmorItem.pickupItemID;
            thisArmorItem.itemName = newArmorItem.itemName;
            thisArmorItem.img.sprite = newArmorItem.item.icon;
            thisArmorItem.itemType = newArmorItem.itemType;
            thisArmorItem.stackable = newArmorItem.stackable;
            thisArmorItem.maxStack = newArmorItem.maxStack;
            thisArmorItem.count = newArmorItem.count;
            thisArmorItem.ammoType = newArmorItem.ammoType;
            thisArmorItem.InitializeSlot();
            return true;
        }
        return false;
    }
    public bool CheckIfRoomInWeaponSlots(InventoryItem newWeaponItem)
    {
        for(int i = 1; i < equipmentSlotsUI.Length; i++)
        {
            InventoryItem weaponItem = equipmentSlotsUI[i].GetComponentInChildren<InventoryItem>();
            if(weaponItem == null)
            {
                InventoryItem thisItemWeapon = Instantiate(inventoryItem, equipmentSlotsUI[i].transform);
                thisItemWeapon.item = newWeaponItem.item;
                thisItemWeapon.itemId = newWeaponItem.itemId;
                thisItemWeapon.pickupItemID = newWeaponItem.pickupItemID;
                thisItemWeapon.itemName = newWeaponItem.itemName;
                thisItemWeapon.img.sprite = newWeaponItem.item.icon;
                thisItemWeapon.itemType = newWeaponItem.itemType;
                thisItemWeapon.stackable = newWeaponItem.stackable;
                thisItemWeapon.maxStack = newWeaponItem.maxStack;
                thisItemWeapon.count = newWeaponItem.count;
                thisItemWeapon.ammoType = newWeaponItem.ammoType;
                thisItemWeapon.InitializeSlot();
                return true;
            }
        }
        return false;
    }
    void EquipWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            DrawWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            DrawWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            DrawWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            DrawWeapon(4);
    }
    void DrawWeapon(int index)
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            if (equipmentSlotsUI[index].slotData.itemId > -1)
            {
                for(int i = 0; i < weaponSlots.Length; i++)
                {
                    if (weaponSlots[i].GetComponent<Weapon>().weaponId == equipmentSlotsUI[index].slotData.itemId)
                    {
                        weaponSlots[i].SetActive(!weaponSlots[i].activeSelf);
                        weaponSlots[i].GetComponent<Weapon>().UpdateTotalAmmoStatus();
                    }
                    else
                        weaponSlots[i].SetActive(false);
                }
            }
        }
    }
    void HolsterWeapons()
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            for(int i = 0; i < weaponSlots.Length; i++)
                weaponSlots[i].SetActive(false);
        }
    }
    public void SaveHowManyBulletsLeftInMagazine()
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        GameManager.instance.bulletsLeft = new int[weaponSlots.Length];
        if (player != null)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                GameManager.instance.bulletsLeft[i] = weaponSlots[i].GetComponent<Weapon>().bulletsLeft;
            }
        }
    }
    public void LoadHowManyBulletsLeftInMagazine()
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            for (int i = 0; i < GameManager.instance.bulletsLeft.Length; i++)
            {
                weaponSlots[i].SetActive(true);
                weaponSlots[i].GetComponent<Weapon>().bulletsLeft = GameManager.instance.bulletsLeft[i];
                weaponSlots[i].SetActive(false);
            }
        }
    }
    public void WeaponCollected()
    {
        player = PlayerManager.instance.GetPlayer();
        foreach (var slotData in GameManager.instance.inventorySlotsData)
            weaponIDsList.Add(slotData.itemId);
        foreach (var slotData in GameManager.instance.equipmentSlotsData)
            weaponIDsList.Add(slotData.itemId);
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        foreach(var weaponSlot in weaponSlots)
        {
            Weapon weaponScript = weaponSlot.GetComponent<Weapon>();
            if (weaponScript != null)
            {
                if(weaponIDsList.Contains(weaponScript.weaponId))
                {
                    weaponSlot.SetActive(true);
                    weaponScript.weaponCollected = true;
                    weaponSlot.SetActive(false);
                }
            }
        }
    }
    public void OpenInventory()
    {
        inventoryAnim.GetComponent<Animator>().SetBool("InventoryOn", true);
        equipmentAnim.GetComponent<Animator>().SetBool("EquipmentScreenOn", true);
        closed = false;
        HolsterWeapons();
        selectSlot.Select();
    }
    public void CloseInventory()
    {
        inventoryAnim.GetComponent<Animator>().SetBool("InventoryOn", false);
        equipmentAnim.GetComponent<Animator>().SetBool("EquipmentScreenOn", false);
        CloseInventoryItemMenus();
        closed = true;
        HolsterWeapons();
        if(tempInventoryItem != null)
        {
            tempInventoryItem.GetComponentInParent<InventorySlot>().CloseTransparentBG(tempInventoryItem);
            tempInventoryItem = null;
        }
    }
    void CloseInventoryItemMenus() // when closing inventory
    {
        for(int i = 0; i < inventorySlotsUI.Length; i++)
        {
            InventoryItem thisItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if(thisItem != null)
            {
                thisItem.sliderBG.SetActive(false);
                thisItem.itemMenuMoreThanOne.SetActive(false);
                thisItem.itemMenu.SetActive(false);
            }
        }
    }
    public bool AddInventoryItem(Item newItem, int pickupItemID, int count)
    {
        // add to stackable
        for(int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if(inventorySlotsUI[i].slotData.itemId == newItem.itemID && inventorySlotsUI[i].slotData.stackable == true)
            {
                InventoryItem itemInSlot = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                if(newItem.stackMax > inventorySlotsUI[i].slotData.count)
                {
                    int tempTotalCount = itemInSlot.count + count;
                    if(itemInSlot.maxStack >= tempTotalCount)
                    {
                        inventorySlotsUI[i].slotData.count += count;
                        itemInSlot.count += count;
                        itemInSlot.slider.maxValue += count;
                        itemInSlot.RefreshCount();
                    }
                    else
                    {
                        int decreaseCount = tempTotalCount - itemInSlot.maxStack;
                        inventorySlotsUI[i].slotData.count = newItem.stackMax;
                        itemInSlot.count = newItem.stackMax;
                        itemInSlot.slider.maxValue = newItem.stackMax;
                        itemInSlot.RefreshCount();

                        // find next empty slot
                        for (i = 0; i < inventorySlotsUI.Length; i++)
                        {
                            if (inventorySlotsUI[i].slotData.itemId == -1)
                            {
                                inventorySlotsUI[i].slotData.itemId = newItem.itemID;
                                inventorySlotsUI[i].slotData.itemName = newItem.itemName;
                                inventorySlotsUI[i].slotData.itemType = newItem.itemType;
                                inventorySlotsUI[i].slotData.stackable = newItem.stackable;
                                inventorySlotsUI[i].slotData.stackMax = newItem.stackMax;
                                inventorySlotsUI[i].slotData.count += 1;
                                inventorySlotsUI[i].slotData.ammoType = newItem.ammoType;
                                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                                thisInventoryItem.GetComponent<InventoryItem>().item = newItem;
                                thisInventoryItem.GetComponent<InventoryItem>().itemId = newItem.itemID;
                                thisInventoryItem.GetComponent<InventoryItem>().itemName = newItem.itemName;
                                thisInventoryItem.GetComponent<InventoryItem>().itemType = newItem.itemType;
                                thisInventoryItem.GetComponent<InventoryItem>().stackable = newItem.stackable;
                                thisInventoryItem.GetComponent<InventoryItem>().maxStack = newItem.stackMax;
                                thisInventoryItem.GetComponent<InventoryItem>().img.sprite = newItem.icon;
                                thisInventoryItem.GetComponent<InventoryItem>().count = decreaseCount;
                                thisInventoryItem.GetComponent<InventoryItem>().ammoType = newItem.ammoType;
                                thisInventoryItem.GetComponent<InventoryItem>().slider.maxValue = thisInventoryItem.count;
                                InventorySlot slot = thisInventoryItem.GetComponentInParent<InventorySlot>();
                                slot.slotData.count = thisInventoryItem.count;
                                thisInventoryItem.RefreshCount();
                                if(thisInventoryItem.count > 1)
                                    thisInventoryItem.InitializeSlider();
                                thisInventoryItem.InitializeAmmoStatus();
                                break;
                            }
                        }
                    }
                    if (newItem.itemType == ItemType.Ammo)
                    {
                        player = PlayerManager.instance.GetPlayer();
                        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
                        if (player != null)
                        {
                            for (i = 0; i < weaponSlots.Length; i++)
                            {
                                weaponSlots[i].GetComponent<Weapon>().UpdateTotalAmmoStatus();
                            }
                        }
                    }
                    return true;
                }
            }
        }

        // find next empty slot
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if (inventorySlotsUI[i].slotData.itemId == -1)
            {
                inventorySlotsUI[i].slotData.itemId = newItem.itemID;
                inventorySlotsUI[i].slotData.itemName = newItem.itemName;
                inventorySlotsUI[i].slotData.itemType = newItem.itemType;
                inventorySlotsUI[i].slotData.stackable = newItem.stackable;
                inventorySlotsUI[i].slotData.stackMax = newItem.stackMax;
                inventorySlotsUI[i].slotData.count += 1;
                inventorySlotsUI[i].slotData.ammoType = newItem.ammoType;
                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.GetComponent<InventoryItem>().item = newItem;
                if (newItem.itemType == ItemType.Weapon)
                {
                    inventorySlotsUI[i].slotData.pickupItemID = pickupItemID;
                    thisInventoryItem.GetComponent<InventoryItem>().pickupItemID = pickupItemID;
                }
                thisInventoryItem.GetComponent<InventoryItem>().InitializeItem(count);
                return true;
            }
        }
        return false;
    }
    public bool SplitStack(Item newItem, int pickupItemID, int count)
    {
        for (int i = 0; i < inventorySlotsUI.Length; i++) // find next empty slot
        {
            if (inventorySlotsUI[i].slotData.itemId == -1)
            {
                inventorySlotsUI[i].slotData.itemId = newItem.itemID;
                inventorySlotsUI[i].slotData.itemName = newItem.itemName;
                inventorySlotsUI[i].slotData.itemType = newItem.itemType;
                inventorySlotsUI[i].slotData.stackable = newItem.stackable;
                inventorySlotsUI[i].slotData.stackMax = newItem.stackMax;
                inventorySlotsUI[i].slotData.count = count;
                inventorySlotsUI[i].slotData.ammoType = newItem.ammoType;
                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.GetComponent<InventoryItem>().item = newItem;
                if (newItem.itemType == ItemType.Weapon)
                {
                    inventorySlotsUI[i].slotData.pickupItemID = pickupItemID;
                    thisInventoryItem.GetComponent<InventoryItem>().pickupItemID = pickupItemID;
                }
                thisInventoryItem.GetComponent<InventoryItem>().InitializeItem(count);
                return true;
            }
        }
        return false;
    }
    public void SaveInventory() // and equipment to GameManager
    {
        GameManager.instance.inventorySlotsData = new InventorySlotData[inventorySlotsUI.Length];
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            GameManager.instance.inventorySlotsData[i] = inventorySlotsUI[i].slotData;
        }
        GameManager.instance.equipmentSlotsData = new InventorySlotData[equipmentSlotsUI.Length];
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            GameManager.instance.equipmentSlotsData[i] = equipmentSlotsUI[i].slotData;
        }
    }
    public void LoadInventorySlotData() // and equipment slot data from GameManager
    {
        for(int i = 0; i < inventorySlotsUI.Length; i++)
        {
            inventorySlotsUI[i].slotData = GameManager.instance.inventorySlotsData[i];
        }
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            equipmentSlotsUI[i].slotData = GameManager.instance.equipmentSlotsData[i];
        }
    }
    public void AddSavedInventorySlotData() // to inventory
    {
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if (inventorySlotsUI[i].slotData.itemId > -1)
            {
                inventorySlotsUI[i].slotData.itemId = GameManager.instance.inventorySlotsData[i].itemId;
                inventorySlotsUI[i].slotData.itemType = GameManager.instance.inventorySlotsData[i].itemType;
                inventorySlotsUI[i].slotData.pickupItemID = GameManager.instance.inventorySlotsData[i].pickupItemID;
                inventorySlotsUI[i].slotData.stackable = GameManager.instance.inventorySlotsData[i].stackable;
                inventorySlotsUI[i].slotData.stackMax = GameManager.instance.inventorySlotsData[i].stackMax;
                InventoryItem newItemGo = Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.item = itemsDatabase[inventorySlotsUI[i].slotData.itemId];
                newItemGo.itemId = thisInventoryItem.item.itemID;
                newItemGo.pickupItemID = inventorySlotsUI[i].slotData.pickupItemID;
                newItemGo.itemName = thisInventoryItem.item.itemName;
                newItemGo.itemType = thisInventoryItem.item.itemType;
                newItemGo.stackable = thisInventoryItem.item.stackable;
                newItemGo.maxStack = thisInventoryItem.item.stackMax;
                newItemGo.count = inventorySlotsUI[i].slotData.count;
                newItemGo.img.sprite = thisInventoryItem.item.icon;
                newItemGo.ammoType = thisInventoryItem.item.ammoType;
                newItemGo.InitializeSlider();
                thisInventoryItem.GetComponent<InventoryItem>().RefreshCount();
            }
        }
        // add saved equipment slot data
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            if (equipmentSlotsUI[i].slotData.itemId > -1)
            {
                equipmentSlotsUI[i].slotData.itemId = GameManager.instance.equipmentSlotsData[i].itemId;
                equipmentSlotsUI[i].slotData.itemType = GameManager.instance.equipmentSlotsData[i].itemType;
                equipmentSlotsUI[i].slotData.pickupItemID = GameManager.instance.equipmentSlotsData[i].pickupItemID;
                equipmentSlotsUI[i].slotData.stackable = GameManager.instance.equipmentSlotsData[i].stackable;
                equipmentSlotsUI[i].slotData.stackMax = GameManager.instance.equipmentSlotsData[i].stackMax;
                InventoryItem newItemGo = Instantiate(inventoryItem, equipmentSlotsUI[i].transform);
                InventoryItem thisInventoryItem = equipmentSlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.item = itemsDatabase[equipmentSlotsUI[i].slotData.itemId];
                newItemGo.itemId = thisInventoryItem.item.itemID;
                newItemGo.itemName = thisInventoryItem.item.itemName;
                newItemGo.itemType = thisInventoryItem.item.itemType;
                newItemGo.pickupItemID = thisInventoryItem.pickupItemID;
                newItemGo.stackable = thisInventoryItem.item.stackable;
                newItemGo.maxStack = thisInventoryItem.item.stackMax;
                newItemGo.count = equipmentSlotsUI[i].slotData.count;
                newItemGo.img.sprite = thisInventoryItem.item.icon;
                newItemGo.ammoType = thisInventoryItem.item.ammoType;
                thisInventoryItem.GetComponent<InventoryItem>().RefreshCount();
            }
        }
    }
    public void ClearInventory() // and equipment
    {
        for(int i = 0; i < GameManager.instance.inventorySlotsData.Length; i++)
        {
            GameManager.instance.inventorySlotsData[i] = null;
        }
        for (int i = 0; i < GameManager.instance.equipmentSlotsData.Length; i++)
        {
            GameManager.instance.equipmentSlotsData[i] = null;
        }
    }
}