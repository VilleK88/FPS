using UnityEngine;
public class Bullet : MonoBehaviour
{
    public string target = "";
    public float damage = 0;
    private void Start()
    {
        Destroy(gameObject, 2);
    }
    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (target != "" && objectWeHit.gameObject.CompareTag(target))
        {
            if (target == "Player")
                objectWeHit.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            Debug.Log("hit " + objectWeHit.gameObject.name + " !");
            CreateBulletImpactEffect(objectWeHit);
        }
        if(objectWeHit.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit a wall");
            CreateBulletImpactEffect(objectWeHit);
        }
        if (objectWeHit.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Hit a ground");
            CreateBulletImpactEffect(objectWeHit);
        }
        Destroy(gameObject);
    }
    void CreateBulletImpactEffect(Collision objectWeHit) // and destroy gameobject
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}