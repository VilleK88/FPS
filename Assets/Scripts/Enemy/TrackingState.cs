using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrackingState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.2f;
    public TrackingState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        FOVRoutine();
        Hunt();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player triggers hearing area");
            ToAlertState();
        }
    }
    public void ToAlertState()
    {
        EnemyManager.Instance.indicatorText.text = "Enemy is alerted";
        enemy.currentState = enemy.alertState;
    }
    public void ToChaseState()
    {
        EnemyManager.Instance.indicatorText.text = "Enemy is chasing";
        enemy.currentState = enemy.chaseState;
    }
    public void ToPatrolState()
    {

    }
    public void ToTrackingState()
    {

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
    void Hunt()
    {
        //enemy.indicator.material.color = Color.cyan;
        enemy.agent.destination = enemy.lastKnownPlayerPosition;
        enemy.agent.isStopped = false;
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance && !enemy.agent.pathPending)
        {
            ToAlertState();
        }
    }
}