using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.currentHealth = GameManager.instance.maxHealth;
        HealthUIManager.Instance.UpdateHealthBar();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TakeDamage(10);
    }
    public void TakeDamage(float damage)
    {
        if(GameManager.instance.currentHealth > 0)
        {
            GameManager.instance.currentHealth -= damage;
            HealthUIManager.Instance.UpdateHealthBar();
        }
    }
}
