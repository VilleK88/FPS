using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ExplodingBarrel : MonoBehaviour
{
    public GameObject explosionEffect;
    public BoxCollider boxCollider;
    private float explosionRadius = 5f;
    private float explosionForce = 3000f;
    private float delayBeforeExplosion = 3f;
    private int hitCount = 0;
    private Coroutine explosionCoroutine;
    public LayerMask obstructionLayer;
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hitCollider in hitColliders)
        {
            if(hitCollider.CompareTag("Enemy"))
            {
                //EnemyHealth enemyHealthScript = hitCollider.GetComponent<EnemyHealth>();
                //if(enemyHealthScript != null)
                //{
                    //enemyHealthScript.TakeDamage(100);
                //}
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
        }
        Destroy(gameObject);
    }
    private void DamageEnemy(GameObject enemy)
    {
        Debug.Log("Enemy damaged: " + enemy.name);
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if(rb != null)
        {
            Vector3 explosionDirection = (enemy.transform.position - transform.position).normalized;
            rb.AddForce(explosionDirection * explosionForce);
        }
    }
}