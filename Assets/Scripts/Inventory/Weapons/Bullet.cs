using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 2);
    }

    private void OnCollisionEnter(Collision objectWeHit)
    {
        if(objectWeHit.gameObject.CompareTag("Target"))
        {
            Debug.Log("hit " + objectWeHit.gameObject.name + " !");
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }

        if(objectWeHit.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit a wall");
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Hit a ground");
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
