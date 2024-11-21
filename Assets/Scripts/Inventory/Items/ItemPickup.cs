using System.Collections.Generic;
using UnityEngine;
public class ItemPickup : Interactable
{
    public Item item, secondItem;
    public string itemName;
    public string pickUpItemID;
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
    void AddItemPickupIDsToArray(string newPickupItemID)
    {
        string[] newPickupItemIDs = new string[GameManager.instance.itemPickUpIDs.Length + 1];
        for (int i = 0; i < GameManager.instance.itemPickUpIDs.Length; i++)
            newPickupItemIDs[i] = GameManager.instance.itemPickUpIDs[i];
        newPickupItemIDs[GameManager.instance.itemPickUpIDs.Length] = newPickupItemID;
        GameManager.instance.itemPickUpIDs = newPickupItemIDs;
    }
    [ContextMenu("Generate GUID FOR ID")]
    public void GenerateID()
    {
        pickUpItemID = System.Guid.NewGuid().ToString();
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
                        Ammo ammo = item as Ammo;
                        if (ammo.ammoType == AmmoType.Pistol)
                            count = 7;
                        if (ammo.ammoType == AmmoType.AssaultRifle)
                            count = 30;
                        if (ammo.ammoType == AmmoType.Shotgun)
                            count = 7;
                        PickUp();
                    }
                    else
                        PickUp();
                }
            }
        }
    }
}