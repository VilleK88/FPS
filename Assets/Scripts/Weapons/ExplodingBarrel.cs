using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ExplodingBarrel : MonoBehaviour
{
    public BoxCollider boxCollider;
    private float explosionRadius = 5f;
    private float explosionForce = 3000f;
    private float delayBeforeExplosion = 3f;
    private int hitCount = 0;
    private Coroutine explosionCoroutine;
    public LayerMask obstructionLayer;
    public AudioClip explosionSound;
    public string explodingBarrelID;
    public void BarrelHit()
    {
        hitCount++;
        if (hitCount == 1)
            explosionCoroutine = StartCoroutine(DelayedExplosion());
        else if (hitCount > 1)
        {
            if (explosionCoroutine != null)
                StopCoroutine(explosionCoroutine);
            Explode();
        }
    }
    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);
        Explode();
    }
    private void Explode()
    {
        AudioManager.instance.PlayExplosionSound(explosionSound);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        GameObject explosionEffect = GlobalReferences.Instance.explosionEffect;
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        foreach(Collider hitCollider in hitColliders)
        {
            if(hitCollider.CompareTag("Enemy"))
            {
                Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                if (!Physics.Raycast(transform.position, directionToTarget.normalized, explosionRadius, obstructionLayer))
                {
                    EnemyHealth enemyHealthScript = hitCollider.GetComponent<EnemyHealth>();
                    if (enemyHealthScript != null)
                    {
                        enemyHealthScript.TakeDamage(100);
                    }
                    DamageEnemy(hitCollider.gameObject);
                }
            }
            if (hitCollider.CompareTag("Player"))
            {
                Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                if (!Physics.Raycast(transform.position, directionToTarget.normalized, explosionRadius, obstructionLayer))
                {
                    PlayerHealth playerHealthScript = hitCollider.GetComponent<PlayerHealth>();
                    if (playerHealthScript != null)
                    {
                        playerHealthScript.TakeDamage(50);
                    }
                }
            }
        }
        AddToExplodingBarrelIDsArray(explodingBarrelID);
        Destroy(gameObject);
    }
    private void DamageEnemy(GameObject enemy)
    {
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if(rb != null)
        {
            Vector3 explosionDirection = (enemy.transform.position - transform.position).normalized;
            rb.AddForce(explosionDirection * explosionForce);
        }
    }
    void AddToExplodingBarrelIDsArray(string newExplodingBarrelID)
    {
        string[] newExplodingBarrelIDs = new string[GameManager.instance.explodingBarrelIDs.Length + 1];
        for(int i = 0; i < GameManager.instance.explodingBarrelIDs.Length; i++)
        {
            newExplodingBarrelIDs[i] = GameManager.instance.explodingBarrelIDs[i];
        }
        newExplodingBarrelIDs[GameManager.instance.explodingBarrelIDs.Length] = newExplodingBarrelID;
        GameManager.instance.explodingBarrelIDs = newExplodingBarrelIDs;
    }
    [ContextMenu("Generate GUID FOR ID")]
    public void GenerateID()
    {
        explodingBarrelID = System.Guid.NewGuid().ToString();
    }
}