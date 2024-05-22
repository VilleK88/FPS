using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AlertState : IEnemyState
{
    private StatePatternEnemy enemy;
    public float searchTimer;
    private float fovTimer = 0.2f;
    public float turnSpeed;
    public bool checkDisturbance;
    public AlertState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        FOVRoutine();
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        HearingArea();
        LookAround();
    }
    public void OnTriggerEnter(Collider other)
    {
    }
    public void HearingArea()
    {
        if (enemy.distanceToPlayer < 8.1f && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
        {
            enemy.lastKnownPlayerPosition = enemy.player.transform.position;
            checkDisturbance = false;
            searchTimer = 0;
        }
    }
    public void ToAlertState()
    {
    }
    public void ToCombatState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("running", true);
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorText.text = "Enemy is chasing";
        searchTimer = 0;
        checkDisturbance = false;
        enemy.currentState = enemy.combatState;
    }
    public void ToPatrolState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", true);
        enemy.agent.speed = enemy.walkSpeed;
        EnemyManager.Instance.StartCoroutine(EnemyManager.Instance.BackToPatrol());
        searchTimer = 0;
        checkDisturbance = false;
        enemy.currentState = enemy.patrolState;
    }
    public void ToTrackingState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", true);
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorText.text = "Enemy is tracking";
        enemy.currentState = enemy.trackingState;
        searchTimer = 0;
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
    void LookAround()
    {
        enemy.agent.isStopped = true;
        Quaternion currentRotation = enemy.transform.rotation;
        Vector3 directionToImpact = enemy.lastKnownPlayerPosition - enemy.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToImpact);
        Quaternion newRotation = Quaternion.Slerp(currentRotation, targetRotation, 3 * Time.deltaTime);
        enemy.transform.rotation = newRotation;

        searchTimer += Time.deltaTime;
        if (searchTimer >= enemy.searchDuration)
        {
            if (!checkDisturbance)
                ToPatrolState();
            else
                ToTrackingState();
        }
    }
}