using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PatrolState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.2f;
    int waypointIndex;
    float waypointCounter = 0;
    float waypointMaxTime = 4;
    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        FOVRoutine();
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        HearingArea();
        Patrol();
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
        enemy.lastKnownPlayerPosition = enemy.player.transform.position;
        EnemyManager.Instance.indicatorText.text = "Enemy is alerted";
        enemy.currentState = enemy.alertState;
    }
    public void ToChaseState()
    {
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorText.text = "Enemy is chasing";
        enemy.currentState = enemy.chaseState;
    }
    public void ToPatrolState()
    {
    }
    public void ToTrackingState()
    {
        enemy.agent.speed = enemy.walkSpeed;
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
    void Patrol()
    {
        if (Vector3.Distance(enemy.transform.position, enemy.waypoints[waypointIndex].transform.position) < 1)
        {
            if (enemy.randomPatrol)
                waypointIndex = Random.Range(0, enemy.waypoints.Length);
            else
                waypointIndex = (waypointIndex + 1) % enemy.waypoints.Length;
            waypointCounter = 0;
            if (waypointIndex >= enemy.waypoints.Length)
                waypointIndex = 0;
        }
        if (waypointCounter < waypointMaxTime)
            waypointCounter += Time.deltaTime;
        else
        {
            enemy.agent.SetDestination(enemy.waypoints[waypointIndex].position);
        }
    }
}