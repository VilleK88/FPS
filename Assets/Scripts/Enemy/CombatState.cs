using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CombatState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.1f;
    private float shootingTime = 3f;
    private float shootingDelay = 2f;
    public CombatState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        FOVRoutine();
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        Chase();
    }
    public void OnTriggerEnter(Collider other)
    {
    }
    public void HearingArea()
    {
        if (enemy.distanceToPlayer < 8.1f && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
            ToAlertState();
    }
    public void ToAlertState()
    {
        enemy.agent.isStopped = true;
        enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.speed = enemy.walkSpeed;
        EnemyManager.Instance.indicatorText.text = "Enemy is alerted";
        enemy.currentState = enemy.alertState;
    }
    public void ToCombatState()
    {
    }
    public void ToPatrolState()
    {
    }
    public void ToTrackingState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.speed = enemy.walkSpeed;
        EnemyManager.Instance.indicatorText.text = "Enemy is tracking";
        enemy.lastKnownPlayerPosition = enemy.player.transform.position;
        enemy.currentState = enemy.trackingState;
    }
    public void FOVRoutine()
    {
        if (fovTimer > 0)
            fovTimer -= Time.deltaTime;
        else
        {
            FieldOfViewCheck();
            fovTimer = 0;
        }
    }
    void FieldOfViewCheck()
    {
        enemy.rangeChecks = Physics.OverlapSphere(enemy.transform.position, enemy.radius, enemy.targetMask);
        if (enemy.rangeChecks.Length != 0)
        {
            enemy.target = enemy.rangeChecks[0].transform;
            enemy.directionToTarget = (enemy.target.position - enemy.transform.position).normalized;
            if (Vector3.Angle(enemy.transform.forward, enemy.directionToTarget) < enemy.angle / 2)
            {
                if (!Physics.Raycast(enemy.transform.position, enemy.directionToTarget, enemy.distanceToPlayer, enemy.obstructionMask))
                    enemy.canSeePlayer = true;
                else
                    enemy.canSeePlayer = false;
            }
        }
        else if (enemy.canSeePlayer)
            enemy.canSeePlayer = false;
        if (enemy.canSeePlayer)
        {
            ToCombatState();
            enemy.CallHelp();
            Debug.Log("Call Help from CombatState");
        }
        else
            ToTrackingState();
    }
    void Chase()
    {
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
        if (enemy.distanceToPlayer > 10)
        {
            enemy.agent.isStopped = false;
            enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
            enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
            enemy.agent.SetDestination(enemy.player.transform.position);
        }
        else
        {
            enemy.agent.isStopped = true;
            enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
            enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
            enemy.transform.LookAt(enemy.player.transform.position);
            if (shootingTime > 0)
            {
                shootingTime -= Time.deltaTime;
                if (enemy.readyToShoot && !enemy.enemyHealth.takingHit)
                    enemy.Shoot();
                else if(enemy.enemyHealth.takingHit)
                    enemy.Invoke("RecoverFromHit", 1);
            }
            else
            {
                shootingDelay -= Time.deltaTime;
                if(shootingDelay < 0)
                {
                    shootingDelay = 2;
                    shootingTime = 3;
                }
            }
        }
    }
}