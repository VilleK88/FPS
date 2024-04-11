using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;
    public string itemName;
    public int pickUpItemID;
    public int pickUpItemCount;

    [SerializeField] bool collectOnTouch;

    private void Start()
    {
        if (!GameManager.instance.loadInventory)
            pickUpItemCount = item.count;
    }

    public override void Interact()
    {
        if(!collectOnTouch)
        {
            base.Interact();
            PickUp();
        }
    }

    void PickUp()
    {
        bool wasPickedUp = InventoryManager.instance.AddInventoryItem(item, pickUpItemID);
        if(wasPickedUp)
        {
            AddItemPickupIDsToArray(pickUpItemID);
            gameObject.SetActive(false);
        }
    }

    void AddItemPickupIDsToArray(int newPickupItemID)
    {
        int[] newPickupItemIDs = new int[GameManager.instance.itemPickUpIDs.Length + 1];
        for(int i = 0; i < GameManager.instance.itemPickUpIDs.Length; i++)
        {
            newPickupItemIDs[i] = GameManager.instance.itemPickUpIDs[i];
        }
        newPickupItemIDs[GameManager.instance.itemPickUpIDs.Length] = newPickupItemID;
        GameManager.instance.itemPickUpIDs = newPickupItemIDs;
    }

    public void GenerateID()
    {
        pickUpItemID = UnityEngine.Random.Range(0, 1000000000);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(collectOnTouch)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PickUp();
            }
        }
    }
}
