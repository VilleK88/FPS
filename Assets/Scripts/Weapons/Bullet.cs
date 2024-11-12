using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float damage = 0;
    private void Start()
    {
        Destroy(gameObject, 2);
    }
    private void OnCollisionEnter(Collision objectWeHit)
    {
        EnemyHealth enemyHealth = objectWeHit.gameObject.GetComponentInParent<EnemyHealth>();
        if (objectWeHit.gameObject.CompareTag("Enemy") && enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            //CreateBlootSplatterEffect(objectWeHit);
        }
        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            CreateBulletImpactEffect(objectWeHit);
        }
        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            CreateBulletImpactEffect(objectWeHit);
        }
        if (objectWeHit.gameObject.CompareTag("Ground"))
        {
            CreateBulletImpactEffect(objectWeHit);
        }
        if(objectWeHit.gameObject.CompareTag("ExplodingBarrel"))
        {
            ExplodingBarrel explodingBarrelScript = objectWeHit.gameObject.GetComponent<ExplodingBarrel>();
            if (explodingBarrelScript != null) explodingBarrelScript.BarrelHit();
        }
        Destroy(gameObject);
    }
    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
    void CreateBlootSplatterEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(GlobalReferences.Instance.bloodSplatterEffectPrefab,
            contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}