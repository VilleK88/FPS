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
        //EnemyManager.Instance.indicatorText.text = "Enemy is alerted";
        EnemyManager.Instance.indicatorImage.enabled = true;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.alertImage;
        enemy.currentState = enemy.alertState;
    }
    public void ToCombatState()
    {
        enemy.agent.speed = enemy.runningSpeed;
        //EnemyManager.Instance.indicatorText.text = "Enemy is chasing";
        EnemyManager.Instance.indicatorImage.enabled = true;
        EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.combatImage;
        enemy.currentState = enemy.combatState;
    }
    public void ToPatrolState()
    {
    }
    public void ToTrackingState()
    {
        //EnemyManager.Instance.indicatorText.text = "Enemy is tracking";
        //Debug.Log("Enemy starts tracking");
        EnemyManager.Instance.indicatorImage.enabled = true;
        if(!EnemyManager.Instance.CanAnyoneSeeThePlayer())
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.trackingImage;
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
            ToCombatState();
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
        {
            waypointCounter += Time.deltaTime;
            enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
        }
        else
        {
            enemy.agent.SetDestination(enemy.waypoints[waypointIndex].position);
            enemy.GetComponentInChildren<Animator>().SetBool("Walk", true);
        }
    }
}