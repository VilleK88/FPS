using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProjectilePrefab : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] StatePatternEnemy[] enemies;
    private void Start()
    {
        Destroy(gameObject, 5);
    }
    private void OnCollisionEnter(Collision objectWeHit)
    {
        if(objectWeHit.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            transform.SetParent(objectWeHit.transform);
            //CreateThrowImpactEffect(objectWeHit);
            enemies = FindObjectsOfType<StatePatternEnemy>();
            for (int i = 0; i < enemies.Length; i++)
            {
                StatePatternEnemy enemy = enemies[i];
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < 20)
                {
                    //enemy.disturbance = false;
                    //enemy.Disturbance();
                    //enemy.disturbanceTimes++;
                }
            }
            Destroy(gameObject);
        }
    }
    void CreateThrowImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hitPoint = Instantiate(GlobalReferences.Instance.throwImpactEffectPrefab,
            contact.point, Quaternion.LookRotation(contact.normal));
        hitPoint.transform.SetParent(objectWeHit.gameObject.transform);
    }
}