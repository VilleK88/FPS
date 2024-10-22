using UnityEngine;
public class KnifeHitbox : MonoBehaviour
{
    public BoxCollider boxCollider;
    public float damage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            StatePatternEnemy enemyScript = other.gameObject.GetComponent<StatePatternEnemy>();
            if(enemyHealth != null && enemyScript != null)
            {
                if (!enemyScript.canSeePlayer)
                    enemyHealth.TakeDamage(100);
                else
                    enemyHealth.TakeDamage(damage);
            }
        }
    }
}