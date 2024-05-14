using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PatrolState : IEnemyState
{
    private StatePatternEnemy enemy;
    private int nextWaypoint;
    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        Look();
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
        enemy.currentState = enemy.trackingState;
    }
    void Look()
    {
        Debug.DrawRay(enemy.eye.position, enemy.eye.forward * enemy.sightRange, Color.green);
        RaycastHit hit;
        if(Physics.Raycast(enemy.eye.position, enemy.eye.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            ToChaseState();
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