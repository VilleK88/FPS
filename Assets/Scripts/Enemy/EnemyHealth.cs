using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Image healthBar;
    float targetFillAmount;
    private void Start()
    {
        currentHealth = maxHealth;
        targetFillAmount = currentHealth / maxHealth;
        healthBar.fillAmount = targetFillAmount;
    }
    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            Debug.Log("Enemy current health: " + currentHealth);
            targetFillAmount = currentHealth / maxHealth;
            healthBar.fillAmount = targetFillAmount;
        }
        else
            Die();
    }
    void Die()
    {
        Debug.Log("Enemy dead");
        StatePatternEnemy enemy = GetComponent<StatePatternEnemy>();
        enemy.enabled = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
    }
}