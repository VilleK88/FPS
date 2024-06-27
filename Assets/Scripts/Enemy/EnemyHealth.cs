using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
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
    public bool dead;
    public bool alreadyFoundDead;
    StatePatternEnemy enemy;
    NavMeshAgent agent;
    [SerializeField] private GameObject body;
    private CapsuleCollider collider;
    private void Start()
    {
        currentHealth = maxHealth;
        targetFillAmount = currentHealth / maxHealth;
        healthBarFill.fillAmount = targetFillAmount;
        HideHealth();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        DeactivateRagdoll();
        enemy = GetComponent<StatePatternEnemy>();
        agent = GetComponent<NavMeshAgent>();
        collider = GetComponent<CapsuleCollider>();
    }
    public void ShowHealth()
    {
        if (!dead)
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
            //takingHit = true;
            //enemy.lastKnownPlayerPosition = enemy.player.transform.position;
            //enemy.currentState = enemy.combatState;
            if (currentHealth <= 0)
                Die();
            if (!dead)
            {
                takingHit = true;
                enemy.lastKnownPlayerPosition = enemy.player.transform.position;
                enemy.currentState = enemy.combatState;
            }
        }
        else
            Die();
    }
    void Die()
    {
        StatePatternEnemy enemy = GetComponent<StatePatternEnemy>();
        enemy.canSeePlayer = false;
        enemy.enabled = false;
        enemy.GetComponentInChildren<Animator>().enabled = false;
        ActivateRagdoll();
        if (AccountManager.Instance != null && !dead)
        {
            if (AccountManager.Instance.loggedIn)
            {
                AccountManager.Instance.OnEnemyKilled();
            }
        }
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
        {
            PlayerManager.instance.sneakIndicatorImage.color = new Color(0f, 0f, 0f, 0f);
            EnemyManager.Instance.indicatorImage.enabled = false;
        }
        dead = true;
        healthbar.active = false;
        agent.isStopped = true;
        //StartCoroutine(Vanish());
    }
    public IEnumerator Vanish()
    {
        yield return new WaitForSeconds(30f);
        body.SetActive(false);
        collider.enabled = false;
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