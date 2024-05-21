using UnityEngine;
public class EnemyHeadCollider : MonoBehaviour
{
    [SerializeField] EnemyHealth parentEnemyHealthScript;
    public float damageMultiplier = 3;
    private void Start()
    {
        parentEnemyHealthScript = GetComponentInParent<EnemyHealth>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if(bullet != null)
                parentEnemyHealthScript.TakeDamage(bullet.damage * damageMultiplier);
            Destroy(collision.gameObject);
        }
    }
}