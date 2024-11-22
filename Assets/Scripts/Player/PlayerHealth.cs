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
    [HideInInspector] Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        if (Input.GetKeyDown(KeyCode.Q) && !PlayerManager.instance.dead)
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
                Die();
        }
    }
    public bool HealPlayer(float health)
    {
        if (GameManager.instance.maxHealth > GameManager.instance.currentHealth)
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
        for (int i = 0; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
        {
            InventoryItem inventoryItem = InventoryManager.instance.inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.itemType == ItemType.Medpack)
            {
                Item item = inventoryItem.item;
                Medpack medPack = item as Medpack;
                bool healing = HealPlayer(medPack.healthAmount);
                if (healing)
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
        ChangeVignetteColorToRed();
        ChangeVignetteSmoothness(1);
        intensity = 0.4f;
        vignette.enabled.Override(true);
        vignette.intensity.Override(0.4f); // 0.4f original
        yield return new WaitForSeconds(0.1f); // 0.4f original
        while (intensity > 0)
        {
            intensity -= 0.02f; // 0.01f original
            if (intensity < 0)
                intensity = 0;
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.1f); // 0.1f original
        }
        vignette.enabled.Override(false);
        ChangeVignetteColorToBlack();
        ChangeVignetteSmoothness(0.4f);
        yield break;
    }
    void ChangeVignetteColorToRed()
    {
        vignette.color.Override(new Color32(172, 0, 0, 255));
    }
    void ChangeVignetteColorToBlack()
    {
        vignette.color.Override(Color.black);
    }
    void ChangeVignetteSmoothness(float smoothness)
    {
        vignette.smoothness.Override(smoothness);
    }
    void Die()
    {
        PlayerManager.instance.dead = true;
        PlayerMovement playerMovementScript = GetComponent<PlayerMovement>();
        playerMovementScript.controller.Move(Vector3.zero);
        playerMovementScript.enabled = false;
        MouseLook mouseLookScript = GetComponentInChildren<MouseLook>();
        if(mouseLookScript != null) mouseLookScript.enabled = false;
        rb.isKinematic = true;
        GameOverScreen.instance.StartCoroutine(GameOverScreen.instance.GameOver());
    }
}