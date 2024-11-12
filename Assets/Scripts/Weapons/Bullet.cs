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
            CreateBulletImpactFleshSmallEffect(objectWeHit);
            //CreateBlootSplatterEffect(objectWeHit);
        }
        if(objectWeHit.gameObject.CompareTag("EnemyHead"))
            CreateBulletImpactFleshSmallEffect(objectWeHit);
        if (objectWeHit.gameObject.CompareTag("Target"))
            CreateBulletImpactStoneEffect(objectWeHit);
        if (objectWeHit.gameObject.CompareTag("Wall"))
            CreateBulletImpactStoneEffect(objectWeHit);
        if (objectWeHit.gameObject.CompareTag("Ground"))
            CreateBulletImpactStoneEffect(objectWeHit);
        if (objectWeHit.gameObject.CompareTag("ExplodingBarrel"))
        {
            CreateBulletImpactMetalEffect(objectWeHit);
            CreateBulletImpactIgnitionFlameEffect(objectWeHit);
            ExplodingBarrel explodingBarrelScript = objectWeHit.gameObject.GetComponent<ExplodingBarrel>();
            if (explodingBarrelScript != null) explodingBarrelScript.BarrelHit();
        }
        Destroy(gameObject);
    }
    void CreateBulletImpactStoneEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactStoneEffect,
            contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
    void CreateBulletImpactMetalEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactMetalEffect,
            contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
    void CreateBulletImpactIgnitionFlameEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(GlobalReferences.Instance.ignitionFlame,
            contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectWeHit.gameObject.transform);
        ParticleSystem ps = hole.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            Debug.Log("Particle System löytyi");
        }
        else
        {
            ParticleSystem childPS = hole.GetComponentInChildren<ParticleSystem>();
            if (childPS != null)
            {
                childPS.Play();
                Debug.Log("Child Particle System löytyi");
            }
        }
    }
    void CreateBulletImpactFleshSmallEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactFleshSmallEffect,
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