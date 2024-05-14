using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float searchTimer;
    public AlertState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        Look();
        Search();
    }
    public void OnTriggerEnter(Collider other)
    {

    }
    public void ToAlertState()
    {

    }
    public void ToChaseState()
    {
        EnemyManager.Instance.indicatorText.text = "Enemy is chasing";
        searchTimer = 0;
        enemy.currentState = enemy.chaseState;
    }
    public void ToPatrolState()
    {
        EnemyManager.Instance.StartCoroutine(EnemyManager.Instance.BackToPatrol());
        searchTimer = 0;
        enemy.currentState = enemy.patrolState;
    }
    public void ToTrackingState()
    {

    }
    void Look()
    {
        Debug.DrawRay(enemy.eye.position, enemy.eye.forward * enemy.sightRange, Color.yellow);
        RaycastHit hit;
        if (Physics.Raycast(enemy.eye.position, enemy.eye.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            ToChaseState();
        }
    }
    void Search()
    {
        //enemy.indicator.material.color = Color.yellow;
        enemy.agent.isStopped = true;
        enemy.transform.Rotate(0, enemy.searchTurnSpeed * Time.deltaTime, 0);
        searchTimer += Time.deltaTime;
        if(searchTimer >= enemy.searchDuration)
        {
            ToPatrolState();
        }
    }
}