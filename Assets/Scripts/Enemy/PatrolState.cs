using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PatrolState : IEnemyState
{
    private StatePatternEnemy enemy;
    private int nextWaypoint;
    private float fovTimer = 0.2f;
    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        FOVRoutine();
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        Patrol();
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
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
        // can't be used because we are already in the patrol state
    }
    public void ToTrackingState()
    {
        EnemyManager.Instance.indicatorText.text = "Enemy is tracking";
        Debug.Log("Enemy starts tracking");
        enemy.currentState = enemy.trackingState;
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
                    ToChaseState();
            }
        }
    }
    void Patrol()
    {
        //enemy.indicator.material.color = Color.green;
        enemy.agent.destination = enemy.waypoints[nextWaypoint].position;
        enemy.agent.isStopped = false;
        if(enemy.agent.remainingDistance <= enemy.agent.stoppingDistance && !enemy.agent.pathPending)
        {
            nextWaypoint = (nextWaypoint + 1) % enemy.waypoints.Length;
        }
    }
}