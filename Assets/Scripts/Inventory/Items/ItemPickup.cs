using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemPickup : Interactable
{
    public Item item, secondItem;
    public string itemName;
    public int pickUpItemID;
    public int count = 1;
    [SerializeField] bool collectOnTouch;
    public List<int> weaponIDsList = new List<int>();
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
        bool wasPickedUp = InventoryManager.instance.AddInventoryItem(item, pickUpItemID, count);
        if(wasPickedUp)
        {
            AddItemPickupIDsToArray(pickUpItemID);
            gameObject.SetActive(false);
        }
    }
    bool CheckIfWeaponAlreadyCollected(int thisItemId)
    {
        foreach(var slotData in InventoryManager.instance.inventorySlotsUI)
            weaponIDsList.Add(slotData.slotData.itemId);

        foreach (var slotData in InventoryManager.instance.equipmentSlotsUI)
            weaponIDsList.Add(slotData.slotData.itemId);

        if (weaponIDsList.Contains(thisItemId))
            return true;

        return false;
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
                if(item.itemType != ItemType.Weapon)
                    PickUp();
                else
                {
                    if(CheckIfWeaponAlreadyCollected(item.itemID))
                    {
                        item = secondItem;
                        itemName = item.itemName;
                        item.itemType = ItemType.Ammo;
                        if (item.ammoType == AmmoType.Pistol)
                            count = 7;
                        if (item.ammoType == AmmoType.AssaultRifle)
                            count = 30;
                        if (item.ammoType == AmmoType.Shotgun)
                            count = 7;
                        PickUp();
                    }
                    else
                    {
                        PickUp();
                    }
                }
            }
        }
    }
}
