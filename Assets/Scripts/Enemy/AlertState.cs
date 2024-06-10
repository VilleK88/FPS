using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AlertState : IEnemyState
{
    private StatePatternEnemy enemy;
    public float searchTimer;
    private float fovTimer = 0.2f;
    public float turnSpeed;
    public bool lookAtDisturbance;
    public bool checkDisturbance;
    float lookAtDisturbanceTimer = 2;
    public AlertState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        FOVRoutine();
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        HearingArea();
        if(!checkDisturbance)
            LookAround();
        else
        {
            WalkToDisturbance();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
    }
    public void HearingArea()
    {
        if (enemy.distanceToPlayer < 8.1f && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
        {
            enemy.lastKnownPlayerPosition = enemy.player.transform.position;
            lookAtDisturbance = false;
            lookAtDisturbanceTimer = 2;
            checkDisturbance = false;
            searchTimer = 0;
        }
        if (enemy.distanceToPlayer <= enemy.hearingPlayerShootRadius)
        {
            Weapon weaponScript = enemy.player.GetComponentInChildren<Weapon>();
            if (weaponScript != null)
            {
                if (weaponScript.isShooting && !weaponScript.silenced)
                    ToCombatState();
            }
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
        EnemyManager.Instance.indicatorImage.enabled = true;
        EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.combatImage;
        searchTimer = 0;
        lookAtDisturbance = false;
        checkDisturbance = false;
        lookAtDisturbanceTimer = 2;
        enemy.currentState = enemy.combatState;
    }
    public void ToPatrolState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", true);
        enemy.agent.speed = enemy.walkSpeed;
        searchTimer = 0;
        lookAtDisturbance = false;
        checkDisturbance = false;
        lookAtDisturbanceTimer = 2;
        enemy.currentState = enemy.patrolState;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
        {
            if (EnemyManager.Instance.CloseIndicatorImage())
                EnemyManager.Instance.StartCoroutine(EnemyManager.Instance.BackToPatrol());
        }
    }
    public void ToTrackingState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", true);
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.trackingImage;
        searchTimer = 0;
        lookAtDisturbanceTimer = 2;
        checkDisturbance = false;
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
            if (!lookAtDisturbance)
                ToPatrolState();
            else
            {
                checkDisturbance = true;
            }
        }
    }
    void WalkToDisturbance()
    {
        float distanceToDisturbance = Vector3.Distance(enemy.lastKnownPlayerPosition, enemy.transform.position);
        if(distanceToDisturbance > 1.5f)
        {
            enemy.agent.isStopped = false;
            enemy.GetComponentInChildren<Animator>().SetBool("Walk", true);
            enemy.agent.SetDestination(enemy.lastKnownPlayerPosition);
        }
        else
        {
            enemy.agent.isStopped = true;
            enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
            if (lookAtDisturbanceTimer > 0)
                lookAtDisturbanceTimer -= Time.deltaTime;
            else
                ToPatrolState();
        }
    }
}