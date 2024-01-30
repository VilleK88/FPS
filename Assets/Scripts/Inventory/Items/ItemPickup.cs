using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;
    public string itemName;
    public int pickUpItemID;

    [SerializeField] bool collectOnTouch;

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
        //InventoryManager.instance.AddItem(item);
        //gameObject.SetActive(false);
    }

    public void GenerateID()
    {
        pickUpItemID = UnityEngine.Random.Range(0, 1000000);
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
