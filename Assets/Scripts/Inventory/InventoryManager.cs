using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UserInterface;

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

    public InventoryObject inventory;
    public InventoryObject equipment;
    [SerializeField] Animator inventoryAnim; // inventory screen
    [SerializeField] Animator equipmentAnim; // equipment screen
    public bool closed = true;

    private void Start()
    {
        if(GameManager.instance.loadInventory)
        {
            inventory.Load();
            equipment.Load();
            GameManager.instance.loadInventory = false;
        }
        else
        {
            inventory.Container.Clear();
            equipment.Container.Clear();
            Debug.Log("Clear inventory");
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
        equipment.Container.Clear();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
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

        if(Input.GetKeyDown(KeyCode.P))
        {
            inventory.Container.Clear();
            equipment.Container.Clear();
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
}