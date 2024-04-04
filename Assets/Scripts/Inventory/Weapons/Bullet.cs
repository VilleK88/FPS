using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float maxTime = 2;
    float count = 0;

    private void Update()
    {
        if (maxTime > count)
            count += Time.deltaTime;
        else
            Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("hit " + collision.gameObject.name + " !");
            Destroy(gameObject);
        }
    }
}
