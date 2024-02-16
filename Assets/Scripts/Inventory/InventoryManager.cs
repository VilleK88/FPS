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

    private void Start()
    {
        if(GameManager.instance.loadInventory)
        {
            //inventory.Load("inventorySave.txt");
            //equipment.Load("equipmentSave.txt");
            inventory.Load();
            equipment.Load();
            GameManager.instance.loadInventory = false;
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
        equipment.Container.Clear();
    }
}
