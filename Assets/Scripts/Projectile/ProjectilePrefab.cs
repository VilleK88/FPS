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
        if (objectWeHit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            StatePatternEnemy enemy = objectWeHit.gameObject.GetComponent<StatePatternEnemy>();
            enemy.lastKnownPlayerPosition = enemy.player.transform.position;
            enemy.currentState = enemy.combatState;
            Destroy(gameObject);
        }
        if (objectWeHit.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            transform.SetParent(objectWeHit.transform);
            enemies = FindObjectsOfType<StatePatternEnemy>();
            for (int i = 0; i < enemies.Length; i++)
            {
                StatePatternEnemy enemy = enemies[i];
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < 20)
                {
                    float x = transform.position.x;
                    float y = transform.position.y;
                    float z = transform.position.z;
                    //enemy.lastKnownPlayerPosition = transform.position;
                    enemy.lastKnownPlayerPosition = new Vector3(x, y+1.5f, z);
                    enemy.alertState.checkDisturbance = true;
                    enemy.alertState.searchTimer = 0;
                    EnemyManager.Instance.indicatorText.text = "Enemy is alerted";
                    enemy.currentState = enemy.alertState;
                }
            }
            Destroy(gameObject);
        }
    }
}