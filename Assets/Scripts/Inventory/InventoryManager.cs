using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

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
    public InventorySlot slotPrefab;
    public InventoryItem inventoryItem;
    public Transform inventoryTransform; // where the slotPrefabs are instantiated.
    public InventorySlot[] inventorySlotsUI; // table where the slotPrefabs are put.
    public EquipmentSlot[] equipmentSlotsUI;

    [SerializeField] Animator inventoryAnim; // inventory screen
    [SerializeField] Animator equipmentAnim; // equipment screen
    public bool closed = true;

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
    }

    public void OpenInventory()
    {
        inventoryAnim.GetComponent<Animator>().SetBool("InventoryOn", true);
        equipmentAnim.GetComponent<Animator>().SetBool("EquipmentScreenOn", true);
        closed = false;
    }

    public void CloseInventory()
    {
        inventoryAnim.GetComponent<Animator>().SetBool("InventoryOn", false);
        equipmentAnim.GetComponent<Animator>().SetBool("EquipmentScreenOn", false);
        closed = true;
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
                    inventorySlotsUI[i].slotData.count++;
                    itemInSlot.count++;
                    itemInSlot.RefreshCount();
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
                inventorySlotsUI[i].slotData.itemType = newItem.itemType;
                inventorySlotsUI[i].slotData.stackable = newItem.stackable;
                inventorySlotsUI[i].slotData.stackMax = newItem.stackMax;
                inventorySlotsUI[i].slotData.count += 1;

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
                newItemGo.itemType = thisInventoryItem.item.itemType;
                newItemGo.stackable = thisInventoryItem.item.stackable;
                newItemGo.maxStack = thisInventoryItem.item.stackMax;
                newItemGo.count = inventorySlotsUI[i].slotData.count;
                newItemGo.img.sprite = thisInventoryItem.item.icon;
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
                newItemGo.itemType = thisInventoryItem.item.itemType;
                newItemGo.stackable = thisInventoryItem.item.stackable;
                newItemGo.maxStack = thisInventoryItem.item.stackMax;
                newItemGo.count = equipmentSlotsUI[i].slotData.count;
                newItemGo.img.sprite = thisInventoryItem.item.icon;
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
