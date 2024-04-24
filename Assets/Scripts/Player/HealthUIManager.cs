using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{
    #region Singleton
    public static HealthUIManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion
    public Image bgHealthbar;
    public Image fillHealthbar;
    public void UpdateHealthBar()
    {
        if (GameManager.instance.currentHealth < 0)
            GameManager.instance.currentHealth = 0;
        float targetFillAmount = GameManager.instance.currentHealth / GameManager.instance.maxHealth;
        fillHealthbar.fillAmount = targetFillAmount;
    }
}
