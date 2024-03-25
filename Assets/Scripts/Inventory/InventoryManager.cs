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

    public Item[] items;
    public InventorySlot slotPrefab;
    public Transform inventoryTransform; // where the slotPrefabs are instantiated.
    public InventorySlot[] inventorySlotsUI; // table where the slotPrefabs are put.

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

    public void AddItem(Item item)
    {
        for(int i = 0; i < items.Length; i++)
        {
            if(items[i] == null)
            {
                items[i] = item;
                break;
            }
        }
    }

    public bool AddInventoryItem(Item newItem)
    {
        for(int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if(inventorySlotsUI[i].itemId == newItem.itemID && inventorySlotsUI[i].stackable == true)
            {
                if(inventorySlotsUI[i].maxStack > inventorySlotsUI[i].count)
                {
                    inventorySlotsUI[i].count++;
                    inventorySlotsUI[i].RefreshCount();
                    return true;
                }
            }
        }

        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if (inventorySlotsUI[i].itemId == -1)
            {
                inventorySlotsUI[i].item = newItem;
                inventorySlotsUI[i].transform.GetChild(0).GetComponentInChildren<Image>().sprite = newItem.icon;
                inventorySlotsUI[i].transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                inventorySlotsUI[i].itemId = newItem.itemID;
                inventorySlotsUI[i].itemType = newItem.itemType;
                inventorySlotsUI[i].stackable = newItem.stackable;
                inventorySlotsUI[i].maxStack = newItem.stackMax;
                inventorySlotsUI[i].count += 1;
                return true;
            }
        }

        return false;
    }
}
