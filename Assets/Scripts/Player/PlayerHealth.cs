using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
    public GameObject postProcessGO;
    PostProcessVolume volume;
    Vignette vignette;
    public float intensity = 0;
    public float armorMultiplier;
    private void Start()
    {
        if(!GameManager.instance.loadPlayerPosition)
            GameManager.instance.currentHealth = GameManager.instance.maxHealth;
        HealthUIManager.Instance.UpdateHealthBar();
        volume = postProcessGO.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<Vignette>(out vignette);
        if (!vignette)
            print("Error, vignette empty");
        else
            vignette.enabled.Override(false);
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
        if (GameManager.instance.currentHealth > 0)
        {
            GameManager.instance.currentHealth -= damage - armorMultiplier;
            HealthUIManager.Instance.UpdateHealthBar();
            StartCoroutine(DamageEffect());
            if (GameManager.instance.currentHealth <= 0)
                StartCoroutine(Die());
        }
    }
    public bool HealPlayer(float health)
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
                {
                    inventoryItem.count--;
                    inventoryItem.GetComponentInParent<InventorySlot>().slotData.count--;
                    if (inventoryItem.count <= 0)
                        inventoryItem.PublicRemoveItem();
                    else
                        inventoryItem.RefreshCount();
                }
                break;
            }
        }
    }
    IEnumerator DamageEffect()
    {
        intensity = 0.4f;
        vignette.enabled.Override(true);
        vignette.intensity.Override(0.1f); // 0.4f original
        yield return new WaitForSeconds(0.1f); // 0.4f original
        while(intensity > 0)
        {
            intensity -= 0.02f; // 0.01f original
            if (intensity < 0)
                intensity = 0;
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.1f); // 0.1f original
        }
        vignette.enabled.Override(false);
        yield break;
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("1 - Menu");
    }
}