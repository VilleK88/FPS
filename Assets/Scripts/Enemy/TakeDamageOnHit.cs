using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TakeDamageOnHit : MonoBehaviour
{
    public float damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}