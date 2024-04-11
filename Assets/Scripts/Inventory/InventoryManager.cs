using System;
using System.Collections;
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

    public Item[] itemsDatabase;
    public GameObject[] weaponPrefabs;
    public InventorySlot slotPrefab;
    public InventoryItem inventoryItem;
    public Transform inventoryTransform; // where the slotPrefabs are instantiated.
    public InventorySlot[] inventorySlotsUI; // table where the slotPrefabs are put.
    public EquipmentSlot[] equipmentSlotsUI;

    [SerializeField] Animator inventoryAnim; // inventory screen
    [SerializeField] Animator equipmentAnim; // equipment screen
    public bool closed = true;

    public GameObject player;
    public GameObject middlePoint; // crosshair

    private void Start()
    {
        inventorySlotsUI = new InventorySlot[20];
        for(int i = 0; i < 20; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, inventoryTransform);
            inventorySlotsUI[i] = slot;
        }

        if(GameManager.instance.loadInventory)
        {
            LoadInventorySlotData();
            AddSavedInventorySlotData();
            LoadHowManyBulletsLeftInMagazine();
            GameManager.instance.loadInventory = false;
        }
        else
        {
            ClearInventory();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (closed)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }

        if (!closed)
            return;

        EquipWeapon();
    }

    void EquipWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Item equipped: " + equipmentSlotsUI[1]);
            DrawWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Item equipped: " + equipmentSlotsUI[2]);
            DrawWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Item equipped: " + equipmentSlotsUI[3]);
            DrawWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Item equipped: " + equipmentSlotsUI[4]);
            DrawWeapon(4);
        }
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
                        weaponSlots[i].GetComponent<Weapon>().InitializeAmmoStatus();
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

    public void OpenInventory()
    {
        inventoryAnim.GetComponent<Animator>().SetBool("InventoryOn", true);
        equipmentAnim.GetComponent<Animator>().SetBool("EquipmentScreenOn", true);
        closed = false;
        HolsterWeapons();
    }

    public void CloseInventory()
    {
        inventoryAnim.GetComponent<Animator>().SetBool("InventoryOn", false);
        equipmentAnim.GetComponent<Animator>().SetBool("EquipmentScreenOn", false);
        closed = true;
        HolsterWeapons();
    }

    public bool AddInventoryItem(Item newItem)
    {
        // add to stackable
        for(int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if(inventorySlotsUI[i].slotData.itemId == newItem.itemID && inventorySlotsUI[i].slotData.stackable == true)
            {
                InventoryItem itemInSlot = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                if(newItem.stackMax > inventorySlotsUI[i].slotData.count)
                {
                    int tempTotalCount = itemInSlot.count + newItem.count;
                    if(itemInSlot.maxStack >= tempTotalCount)
                    {
                        inventorySlotsUI[i].slotData.count += newItem.count;
                        itemInSlot.count += newItem.count;
                        itemInSlot.RefreshCount();
                    }
                    else
                    {
                        int decreaseCount = tempTotalCount - itemInSlot.maxStack;
                        inventorySlotsUI[i].slotData.count = newItem.stackMax;
                        itemInSlot.count = newItem.stackMax;
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
                                InventoryItem newItemGo = Instantiate(inventoryItem, inventorySlotsUI[i].transform);
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
                                InventorySlot slot = thisInventoryItem.GetComponentInParent<InventorySlot>();
                                slot.slotData.count = thisInventoryItem.count;
                                thisInventoryItem.RefreshCount();
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
                                weaponSlots[i].GetComponent<Weapon>().InitializeAmmoStatus();
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
                InventoryItem newItemGo = Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.GetComponent<InventoryItem>().item = newItem;
                thisInventoryItem.GetComponent<InventoryItem>().InitializeItem();
                return true;
            }
        }

        return false;
    }

    // save to GameManager
    public void SaveInventory()
    {
        // saving inventory
        GameManager.instance.inventorySlotsData = new InventorySlotData[inventorySlotsUI.Length];
        for(int i = 0; i < inventorySlotsUI.Length; i++)
        {
            GameManager.instance.inventorySlotsData[i] = inventorySlotsUI[i].slotData;
        }

        // saving equipment
        GameManager.instance.equipmentSlotsData = new InventorySlotData[equipmentSlotsUI.Length];
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            GameManager.instance.equipmentSlotsData[i] = equipmentSlotsUI[i].slotData;
        }
    }

    // load from GameManager
    public void LoadInventorySlotData()
    {
        // loading inventory slots data
        for(int i = 0; i < inventorySlotsUI.Length; i++)
        {
            inventorySlotsUI[i].slotData = GameManager.instance.inventorySlotsData[i];
        }

        // loading equipment slots data
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            equipmentSlotsUI[i].slotData = GameManager.instance.equipmentSlotsData[i];
        }
    }

    // to inventory
    public void AddSavedInventorySlotData()
    {
        // add saved inventory slot data
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if (inventorySlotsUI[i].slotData.itemId > -1)
            {
                inventorySlotsUI[i].slotData.itemId = GameManager.instance.inventorySlotsData[i].itemId;
                inventorySlotsUI[i].slotData.itemType = GameManager.instance.inventorySlotsData[i].itemType;
                inventorySlotsUI[i].slotData.stackable = GameManager.instance.inventorySlotsData[i].stackable;
                inventorySlotsUI[i].slotData.stackMax = GameManager.instance.inventorySlotsData[i].stackMax;
                InventoryItem newItemGo = Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.item = itemsDatabase[inventorySlotsUI[i].slotData.itemId];
                newItemGo.itemId = thisInventoryItem.item.itemID;
                newItemGo.itemName = thisInventoryItem.item.itemName;
                newItemGo.itemType = thisInventoryItem.item.itemType;
                newItemGo.stackable = thisInventoryItem.item.stackable;
                newItemGo.maxStack = thisInventoryItem.item.stackMax;
                newItemGo.count = inventorySlotsUI[i].slotData.count;
                newItemGo.img.sprite = thisInventoryItem.item.icon;

                newItemGo.ammoType = thisInventoryItem.item.ammoType;

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
                equipmentSlotsUI[i].slotData.stackable = GameManager.instance.equipmentSlotsData[i].stackable;
                equipmentSlotsUI[i].slotData.stackMax = GameManager.instance.equipmentSlotsData[i].stackMax;
                InventoryItem newItemGo = Instantiate(inventoryItem, equipmentSlotsUI[i].transform);
                InventoryItem thisInventoryItem = equipmentSlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.item = itemsDatabase[equipmentSlotsUI[i].slotData.itemId];
                newItemGo.itemId = thisInventoryItem.item.itemID;
                newItemGo.itemName = thisInventoryItem.item.itemName;
                newItemGo.itemType = thisInventoryItem.item.itemType;
                newItemGo.stackable = thisInventoryItem.item.stackable;
                newItemGo.maxStack = thisInventoryItem.item.stackMax;
                newItemGo.count = equipmentSlotsUI[i].slotData.count;
                newItemGo.img.sprite = thisInventoryItem.item.icon;

                newItemGo.ammoType = thisInventoryItem.item.ammoType;

                thisInventoryItem.GetComponent<InventoryItem>().RefreshCount();
            }
        }
    }

    public void ClearInventory()
    {
        // clear inventory
        for(int i = 0; i < GameManager.instance.inventorySlotsData.Length; i++)
        {
            GameManager.instance.inventorySlotsData[i] = null;
        }

        // clear equipment
        for (int i = 0; i < GameManager.instance.equipmentSlotsData.Length; i++)
        {
            GameManager.instance.equipmentSlotsData[i] = null;
        }
    }
}
