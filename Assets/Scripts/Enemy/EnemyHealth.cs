using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public GameObject healthbar;
    public Image healthBarFill;
    float targetFillAmount;
    public float showHealthCounter = 2f;
    Rigidbody[] rigidBodies;
    public bool takingHit; // from the players bullets
    bool dead;
    StatePatternEnemy enemy;
    private void Start()
    {
        currentHealth = maxHealth;
        targetFillAmount = currentHealth / maxHealth;
        healthBarFill.fillAmount = targetFillAmount;
        HideHealth();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        DeactivateRagdoll();
        enemy = GetComponent<StatePatternEnemy>();
    }
    public void ShowHealth()
    {
        if(!dead)
            healthbar.active = true;
    }
    public void HideHealth()
    {
        healthbar.active = false;
    }
    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            targetFillAmount = currentHealth / maxHealth;
            healthBarFill.fillAmount = targetFillAmount;
            takingHit = true;
            enemy.lastKnownPlayerPosition = enemy.player.transform.position;
            enemy.currentState = enemy.combatState;
            if (currentHealth <= 0)
                Die();
        }
        else
            Die();
    }
    void Die()
    {
        StatePatternEnemy enemy = GetComponent<StatePatternEnemy>();
        enemy.enabled = false;
        enemy.GetComponentInChildren<Animator>().enabled = false;
        ActivateRagdoll();
        dead = true;
        healthbar.active = false;
    }
    void DeactivateRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
            rigidBody.isKinematic = true;
    }
    void ActivateRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
            rigidBody.isKinematic = false;
    }
}