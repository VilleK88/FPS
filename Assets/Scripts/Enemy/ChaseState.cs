using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private StatePatternEnemy enemy;
    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        Look();
        Chase();
    }
    public void OnTriggerEnter(Collider other)
    {

    }
    public void ToAlertState()
    {
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
    void Chase()
    {
        enemy.indicator.material.color = Color.red;
        enemy.agent.destination = enemy.chaseTarget.position;
        enemy.agent.isStopped = false;
    }
}