using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public ItemObject item;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            InventoryManager.instance.inventory.AddItem(new Item(item), 1);
            gameObject.SetActive(false);
        }
    }
}