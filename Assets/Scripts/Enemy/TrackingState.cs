using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrackingState : IEnemyState
{
    private StatePatternEnemy enemy;
    public TrackingState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
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
    void Look()
    {
        Debug.DrawRay(enemy.eye.position, enemy.eye.forward * enemy.sightRange, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(enemy.eye.position, enemy.eye.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            ToChaseState();
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