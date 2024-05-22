using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TrackingState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.2f;
    private float searchTimer;
    private float moveTimer;
    private bool startSearchTimer;
    private float wanderingRadius = 20f;
    public TrackingState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        FOVRoutine();
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        HearingArea();
        Hunt();
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
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.isStopped = true;
        enemy.agent.speed = enemy.walkSpeed;
        EnemyManager.Instance.indicatorText.text = "Enemy is alerted";
        searchTimer = 0;
        startSearchTimer = false;
        enemy.currentState = enemy.alertState;
    }
    public void ToCombatState()
    {
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorText.text = "Enemy is chasing";
        searchTimer = 0;
        startSearchTimer = false;
        enemy.currentState = enemy.combatState;
    }
    public void ToPatrolState()
    {
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.walkSpeed;
        EnemyManager.Instance.StartCoroutine(EnemyManager.Instance.BackToPatrol());
        searchTimer = 0;
        moveTimer = 0;
        startSearchTimer = false;
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
            ToCombatState();
    }
    void Hunt()
    {
        if(enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            enemy.agent.isStopped = true;
            enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
            enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
            enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
            startSearchTimer = true;
        }
        if(startSearchTimer)
        {
            searchTimer += Time.deltaTime;
            moveTimer += Time.deltaTime;
        }
        if (moveTimer > Random.Range(3, 8))
        {
            enemy.agent.isStopped = false;
            enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", true);
            Vector3 newPos = RandomNavSphere(enemy.transform.position, wanderingRadius, -1);
            enemy.agent.SetDestination(newPos);
            moveTimer = 0;
        }
        if (searchTimer > 20)
            ToPatrolState();
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}