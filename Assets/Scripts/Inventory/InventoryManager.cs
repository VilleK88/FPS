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
            slot.InitializeButton();
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
            thisArmorItem.InitializeItem(newArmorItem.item, newArmorItem.count);
            thisArmorItem.pickupItemID = newArmorItem.pickupItemID;
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
                thisItemWeapon.InitializeItem(newWeaponItem.item, newWeaponItem.count);
                thisItemWeapon.pickupItemID = newWeaponItem.pickupItemID;
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
                GameManager.instance.bulletsLeft[i] = weaponSlots[i].GetComponent<Weapon>().bulletsLeft;
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
    public bool AddInventoryItem(Item newItem, string pickupItemID, int count)
    {
        for(int i = 0; i < inventorySlotsUI.Length; i++) // add to stackable
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
                        int decreaseCount = tempTotalCount - itemInSlot.maxStack; // decrease items count
                        inventorySlotsUI[i].slotData.count = newItem.stackMax;
                        itemInSlot.count = newItem.stackMax;
                        itemInSlot.slider.maxValue = newItem.stackMax;
                        itemInSlot.RefreshCount();
                        for (i = 0; i < inventorySlotsUI.Length; i++) // find next empty slot
                        {
                            if (inventorySlotsUI[i].slotData.itemId == -1)
                            {
                                inventorySlotsUI[i].InitializeSlot(newItem);
                                inventorySlotsUI[i].slotData.count += 1;
                                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                                thisInventoryItem.item = newItem;
                                thisInventoryItem.itemName = newItem.itemName;
                                thisInventoryItem.itemType = newItem.itemType;
                                thisInventoryItem.stackable = newItem.stackable;
                                thisInventoryItem.maxStack = newItem.stackMax;
                                thisInventoryItem.img.sprite = newItem.icon;
                                thisInventoryItem.count = decreaseCount;
                                thisInventoryItem.slider.maxValue = thisInventoryItem.count;
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
                                weaponSlots[i].GetComponent<Weapon>().UpdateTotalAmmoStatus();
                        }
                    }
                    return true;
                }
            }
        }
        for (int i = 0; i < inventorySlotsUI.Length; i++) // find next empty slot
        {
            if (inventorySlotsUI[i].slotData.itemId == -1)
            {
                inventorySlotsUI[i].InitializeSlot(newItem);
                inventorySlotsUI[i].slotData.count += 1;
                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.GetComponent<InventoryItem>().InitializeItem(newItem, count);
                if (newItem.itemType == ItemType.Weapon || newItem.itemType == ItemType.Armor)
                {
                    inventorySlotsUI[i].slotData.pickupItemID = pickupItemID;
                    thisInventoryItem.pickupItemID = pickupItemID;
                }
                return true;
            }
        }
        return false;
    }
    public bool SplitStack(Item newItem, string pickupItemID, int count)
    {
        for (int i = 0; i < inventorySlotsUI.Length; i++) // find next empty slot
        {
            if (inventorySlotsUI[i].slotData.itemId == -1)
            {
                inventorySlotsUI[i].InitializeSlot(newItem);
                inventorySlotsUI[i].slotData.count = count;
                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.GetComponent<InventoryItem>().InitializeItem(thisInventoryItem.item, count);
                if (newItem.itemType == ItemType.Weapon)
                {
                    inventorySlotsUI[i].slotData.pickupItemID = pickupItemID;
                    thisInventoryItem.pickupItemID = pickupItemID;
                }
                return true;
            }
        }
        return false;
    }
    public void SaveInventory() // and equipment to GameManager
    {
        GameManager.instance.inventorySlotsData = new InventorySlotData[inventorySlotsUI.Length];
        for (int i = 0; i < inventorySlotsUI.Length; i++)
            GameManager.instance.inventorySlotsData[i] = inventorySlotsUI[i].slotData;
        GameManager.instance.equipmentSlotsData = new InventorySlotData[equipmentSlotsUI.Length];
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
            GameManager.instance.equipmentSlotsData[i] = equipmentSlotsUI[i].slotData;
    }
    public void LoadInventorySlotData() // and equipment slot data from GameManager
    {
        for(int i = 0; i < inventorySlotsUI.Length; i++)
            inventorySlotsUI[i].slotData = GameManager.instance.inventorySlotsData[i];
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
            equipmentSlotsUI[i].slotData = GameManager.instance.equipmentSlotsData[i];
    }
    public void AddSavedInventorySlotData() // to inventory and equipments
    {
        for (int i = 0; i < inventorySlotsUI.Length; i++) // add saved inventory slot data
        {
            if (inventorySlotsUI[i].slotData.itemId > -1)
            {
                InitializeSavedSlot(GameManager.instance.inventorySlotsData[i], inventorySlotsUI[i]);
                InventoryItem newItemGo = Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.item = itemsDatabase[inventorySlotsUI[i].slotData.itemId];
                InitializeSavedItem(thisInventoryItem, newItemGo);
            }
        }
        for (int i = 0; i < equipmentSlotsUI.Length; i++) // add saved equipment slot data
        {
            if (equipmentSlotsUI[i].slotData.itemId > -1)
            {
                InitializeSavedSlot(GameManager.instance.inventorySlotsData[i], inventorySlotsUI[i]);
                InventoryItem newItemGo = Instantiate(inventoryItem, equipmentSlotsUI[i].transform);
                InventoryItem thisInventoryItem = equipmentSlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.item = itemsDatabase[equipmentSlotsUI[i].slotData.itemId];
                InitializeSavedItem(thisInventoryItem, newItemGo);
            }
        }
    }
    void InitializeSavedItem(InventoryItem savedInventoryItem, InventoryItem newItem)
    {
        newItem.itemId = savedInventoryItem.item.itemID;
        newItem.pickupItemID = savedInventoryItem.GetComponentInParent<InventorySlot>().slotData.pickupItemID;
        newItem.itemName = savedInventoryItem.item.itemName;
        newItem.itemType = savedInventoryItem.item.itemType;
        newItem.stackable = savedInventoryItem.item.stackable;
        newItem.maxStack = savedInventoryItem.item.stackMax;
        newItem.count = savedInventoryItem.GetComponentInParent<InventorySlot>().slotData.count;
        newItem.img.sprite = savedInventoryItem.item.icon;
        newItem.RefreshCount();
    }
    void InitializeSavedSlot(InventorySlotData savedSlotData, InventorySlot newSlot)
    {
        newSlot.slotData.itemId = savedSlotData.itemId;
        newSlot.slotData.itemType = savedSlotData.itemType;
        newSlot.slotData.pickupItemID = savedSlotData.pickupItemID;
        newSlot.slotData.stackable = savedSlotData.stackable;
        newSlot.slotData.stackMax = savedSlotData.stackMax;
    }
    public void ClearInventory() // and equipment
    {
        for(int i = 0; i < GameManager.instance.inventorySlotsData.Length; i++)
            GameManager.instance.inventorySlotsData[i] = null;
        for (int i = 0; i < GameManager.instance.equipmentSlotsData.Length; i++)
            GameManager.instance.equipmentSlotsData[i] = null;
    }
}