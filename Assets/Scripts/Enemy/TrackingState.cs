using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrackingState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.2f;
    private float searchTimer;
    public TrackingState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        FOVRoutine();
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        Hunt();
    }
    public void OnTriggerEnter(Collider other)
    {
        /*if (other.CompareTag("Player"))
        {
            Debug.Log("Player triggers hearing area");
            ToAlertState();
        }*/
    }
    public void HearingArea()
    {
        if (enemy.distanceToPlayer < 8.1f && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
            ToAlertState();
    }
    public void ToAlertState()
    {
        EnemyManager.Instance.indicatorText.text = "Enemy is alerted";
        searchTimer = 0;
        enemy.currentState = enemy.alertState;
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
            ToChaseState();
    }
    void Hunt()
    {
        enemy.agent.SetDestination(enemy.lastKnownPlayerPosition);
        enemy.agent.isStopped = false;
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance && !enemy.agent.pathPending)
        {
            if(!enemy.alertState.checkDisturbance)
                ToAlertState();
            else
            {
                EnemyManager.Instance.indicatorText.text = "Enemy is checking disturbance";
                searchTimer += Time.deltaTime;
                if (searchTimer >= enemy.searchDuration)
                {
                    ToPatrolState();
                }
            }
        }
    }
}