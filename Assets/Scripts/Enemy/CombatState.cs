using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CombatState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.1f;
    private float shootingTime = 2f;
    private float shootingDelay = 2f;
    private float moveTimer;
    public float wanderingRadius = 10f;
    private float startSearchingMaxTime = 3f;
    private float startSearchingTimer = 0;
    public CombatState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        Chase();
    }
    public void OnTriggerEnter(Collider other)
    {
    }
    public void HearingArea()
    {
        if (enemy.distanceToPlayer < 6f && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
            ToAlertState();
    }
    public void ToAlertState()
    {
        enemy.agent.isStopped = true;
        enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
        enemy.GetComponentInChildren<Animator>().SetBool("RunAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.speed = enemy.walkSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
        {
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.alertImage;
            PlayerManager.instance.sneakIndicatorImage.color = new Color(0f, 0f, 0f, 0f);
        }
        startSearchingTimer = 0;
        enemy.currentState = enemy.alertState;
    }
    public void ToCombatState()
    {
        enemy.lastKnownPlayerPosition = enemy.player.transform.position;
        EnemyManager.Instance.indicatorImage.enabled = true;
        EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.combatImage;
        startSearchingTimer = 0;
    }
    public void ToPatrolState()
    {
    }
    public void ToTrackingState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", true);
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
        {
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.trackingImage;
            PlayerManager.instance.sneakIndicatorImage.color = new Color(0f, 0f, 0f, 0f);
        }
        startSearchingTimer = 0;
        enemy.currentState = enemy.trackingState;
    }
    void Chase()
    {
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
        if (enemy.distanceToPlayer > 20)
        {
            if (enemy.canSeePlayer)
            {
                ToCombatState();
                if (shootingTime > 0 && enemy.canSeePlayer)
                {
                    enemy.transform.LookAt(enemy.player.transform.position);
                    enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
                    enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
                    enemy.agent.isStopped = true;
                    shootingTime -= Time.deltaTime;
                    if (enemy.readyToShoot && !enemy.enemyHealth.takingHit)
                        enemy.Shoot();
                    else if (enemy.enemyHealth.takingHit)
                        enemy.Invoke("RecoverFromHit", 1);
                }
                else
                {
                    shootingDelay -= Time.deltaTime;
                    if (shootingDelay < 0)
                    {
                        shootingDelay = 2;
                        shootingTime = 2;
                    }
                    enemy.agent.isStopped = false;
                    enemy.agent.speed = enemy.runningSpeed;
                    enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
                    enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
                    enemy.agent.SetDestination(enemy.player.transform.position);
                }
            }
            else
            {
                enemy.agent.isStopped = false;
                enemy.agent.speed = enemy.runningSpeed;
                enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
                enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
                enemy.agent.SetDestination(enemy.player.transform.position);
                startSearchingTimer += Time.deltaTime;
                if (startSearchingTimer > startSearchingMaxTime)
                    ToTrackingState();
            }
        }
        else
        {
            if (shootingTime > 0 && enemy.canSeePlayer)
            {
                ToCombatState();
                enemy.transform.LookAt(enemy.player.transform.position);
                enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
                enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
                enemy.agent.isStopped = true;
                shootingTime -= Time.deltaTime;
                if (enemy.readyToShoot && !enemy.enemyHealth.takingHit)
                    enemy.Shoot();
                else if (enemy.enemyHealth.takingHit)
                    enemy.Invoke("RecoverFromHit", 1);
            }
            else if(enemy.canSeePlayer)
            {
                ToCombatState();
                shootingDelay -= Time.deltaTime;
                if (shootingDelay < 0)
                {
                    shootingDelay = 2;
                    shootingTime = 2;
                }
                moveTimer += Time.deltaTime;
                if (moveTimer > Random.Range(1, 3))
                {
                    enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
                    enemy.GetComponentInChildren<Animator>().SetBool("Running", true);

                    Vector3 newPos = RandomNavSphere(enemy.transform.position, wanderingRadius, -1);
                    enemy.agent.speed = enemy.runningSpeed;
                    enemy.agent.SetDestination(newPos);
                    enemy.agent.isStopped = false;
                    moveTimer = 0;
                }
            }
            else
            {
                enemy.agent.isStopped = false;
                enemy.agent.speed = enemy.runningSpeed;
                enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
                enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
                enemy.agent.SetDestination(enemy.player.transform.position);
                startSearchingTimer += Time.deltaTime;
                if (startSearchingTimer > startSearchingMaxTime)
                    ToTrackingState();
            }
        }
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection;
        do
        {
            randDirection = Random.insideUnitSphere * dist;
        }
        while (randDirection.magnitude < 5.0f);
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}