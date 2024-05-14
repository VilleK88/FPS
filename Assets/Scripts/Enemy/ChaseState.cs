using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.2f;
    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        //Look();
        FOVRoutine();
        Chase();
    }
    public void OnTriggerEnter(Collider other)
    {

    }
    public void ToAlertState()
    {
        EnemyManager.Instance.indicatorText.text = "Enemy is alerted";
        enemy.currentState = enemy.alertState;
    }
    public void ToChaseState()
    {

    }
    public void ToPatrolState()
    {

    }
    public void ToTrackingState()
    {
        enemy.currentState = enemy.trackingState;
    }
    void Look()
    {
        Vector3 enemyToTarget = enemy.chaseTarget.position - enemy.eye.position;
        Debug.DrawRay(enemy.eye.position, enemyToTarget, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(enemy.eye.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
        }
        else
        {
            enemy.lastKnownPlayerPosition = enemy.chaseTarget.position;
            ToTrackingState();
        }
    }
    public void FOVRoutine()
    {
        if (fovTimer > 0)
            fovTimer -= Time.deltaTime;
        else
        {
            FieldOfViewCheck();
            Debug.Log("FOV routine");
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
                    enemy.currentState = enemy.chaseState;
            }
        }
    }
    void Chase()
    {
        //enemy.indicator.material.color = Color.red;
        //enemy.agent.destination = enemy.chaseTarget.position;
        enemy.agent.SetDestination(enemy.player.transform.position);
        enemy.agent.isStopped = false;
    }
}