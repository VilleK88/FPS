using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private void Start()
    {
        if(!GameManager.instance.loadPlayerPosition)
            GameManager.instance.currentHealth = GameManager.instance.maxHealth;
        HealthUIManager.Instance.UpdateHealthBar();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.Q))
            CheckHealthItemStatus();
    }
    public void TakeDamage(float damage)
    {
        if(GameManager.instance.currentHealth > 0)
        {
            GameManager.instance.currentHealth -= damage;
            HealthUIManager.Instance.UpdateHealthBar();
        }
    }
    bool HealPlayer(float health)
    {
        if(GameManager.instance.maxHealth > GameManager.instance.currentHealth)
        {
            GameManager.instance.currentHealth += health;
            if (GameManager.instance.currentHealth > GameManager.instance.maxHealth)
                GameManager.instance.currentHealth = GameManager.instance.maxHealth;
            HealthUIManager.Instance.UpdateHealthBar();
            return true;
        }
        return false;
    }
    void CheckHealthItemStatus()
    {
        for(int i = 0; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
        {
            InventoryItem inventoryItem = InventoryManager.instance.inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if(inventoryItem != null && inventoryItem.itemType == ItemType.Medpack)
            {
                Item item = inventoryItem.item;
                Medpack medPack = item as Medpack;
                bool healing = HealPlayer(medPack.healthAmount);
                if(healing)
                    inventoryItem.PublicRemoveItem();
                break;
            }
        }
    }
}
