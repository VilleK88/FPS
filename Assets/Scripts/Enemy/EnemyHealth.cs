using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            Debug.Log("Enemy current health: " + currentHealth);
        }
            //currentHealth -= damage;
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