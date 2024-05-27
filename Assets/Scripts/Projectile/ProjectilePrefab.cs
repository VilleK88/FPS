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
            StatePatternEnemy enemy = objectWeHit.gameObject.GetComponentInParent<StatePatternEnemy>();
            enemy.lastKnownPlayerPosition = enemy.player.transform.position;
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.combatImage;
            enemy.currentState = enemy.combatState;
            Destroy(gameObject);
        }
        if (objectWeHit.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
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
                    enemy.lastKnownPlayerPosition = new Vector3(x, y + 1.5f, z);
                    enemy.alertState.lookAtDisturbance = true;
                    enemy.alertState.searchTimer = 0;
                    if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
                    {
                        EnemyManager.Instance.indicatorImage.enabled = true;
                        EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.alertImage;
                    }
                    enemy.currentState = enemy.alertState;
                    enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
                }
            }
            Destroy(gameObject);
        }
    }
}