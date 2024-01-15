using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;
    public string itemName;
    public int pickUpItemID;

    public override void Interact()
    {
        base.Interact();
        PickUp();
    }

    void PickUp()
    {
        InventoryManager.instance.AddItem(item);
        gameObject.SetActive(false);
    }

    public void GenerateID()
    {
        pickUpItemID = UnityEngine.Random.Range(0, 1000000);
    }
}
