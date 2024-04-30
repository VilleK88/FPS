using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ThrowImpactEffect : MonoBehaviour
{
    [SerializeField] Enemy[] enemies;
    private void Start()
    {
        enemies = FindObjectsOfType<Enemy>();
        for(int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i];
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < 20)
            {
                //enemy.DisturbanceOver();
                enemy.disturbance = false;
                enemy.Disturbance();
                enemy.disturbanceTimes++;
            }
        }
        StartCoroutine(DestroyObject());
    }
    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(25);
        Destroy(gameObject);
    }
}