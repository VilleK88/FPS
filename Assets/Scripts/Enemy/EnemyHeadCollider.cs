using UnityEngine;
public class EnemyHeadCollider : MonoBehaviour
{
    [SerializeField] EnemyHealth parentEnemyHealthScript;
    private float damageMultiplier = 4;
    private float supriseDamageMultiplier = 20;
    private StatePatternEnemy enemy;
    private void Start()
    {
        parentEnemyHealthScript = GetComponentInParent<EnemyHealth>();
        enemy = GetComponentInParent<StatePatternEnemy>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet") && enemy.currentState == enemy.combatState)
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if(bullet != null)
                parentEnemyHealthScript.TakeDamage(bullet.damage * damageMultiplier);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.CompareTag("Bullet") && enemy.currentState != enemy.combatState)
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet != null)
                parentEnemyHealthScript.TakeDamage(bullet.damage * supriseDamageMultiplier);
            Destroy(collision.gameObject);
        }
    }
}