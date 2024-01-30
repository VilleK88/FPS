using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        if(GameManager.instance.loadInventory)
        {
            inventory.Load();
            GameManager.instance.loadInventory = false;
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }

    /*public List<Item> items = new List<Item>();

    public bool AddItem(Item item)
    {
        items.Add(item);
        return true;
    }*/
}
