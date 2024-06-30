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
    public float lookAtDisturbanceTimer = 2;
    public AlertState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        HearingArea();
        if (!checkDisturbance)
            LookAround();
        else
        {
            WalkToDisturbance();
        }
        if (enemy.canSeePlayer)
        {
            DetectionTimeUI();
            if (!enemy.playerMovementScript.sneaking)
            {
                enemy.lastKnownPlayerPosition = enemy.player.transform.position;
                ToCombatState();
            }
            else if (enemy.canSeePlayerTimer < enemy.canSeePlayerMaxTime)
                enemy.canSeePlayerTimer += Time.deltaTime;
            else
            {
                enemy.lastKnownPlayerPosition = enemy.player.transform.position;
                ToCombatState();
            }
        }
        else if (!enemy.canSeePlayer && enemy.canSeePlayerTimer != 0)
        {
            enemy.canSeePlayerTimer = 0;
            if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
                PlayerManager.instance.sneakIndicatorImage.color = new Color(0f, 0f, 0f, 0f);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
    }
    public void HearingArea()
    {
        if (enemy.distanceToPlayer < 6f && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
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
                {
                    enemy.lastKnownPlayerPosition = enemy.player.transform.position;
                    ToCombatState();
                }
            }
        }
    }
    public void ToAlertState()
    {
    }
    public void ToCombatState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.combatImage;
        searchTimer = 0;
        lookAtDisturbance = false;
        checkDisturbance = false;
        lookAtDisturbanceTimer = 2;
        enemy.canSeePlayerTimer = 0;
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
        enemy.canSeePlayerTimer = 0;
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
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", true);
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.trackingImage;
        searchTimer = 0;
        lookAtDisturbanceTimer = 2;
        checkDisturbance = false;
        enemy.canSeePlayerTimer = 0;
        enemy.currentState = enemy.trackingState;
    }
    void DetectionTimeUI()
    {
        float alpha = Mathf.Clamp01(enemy.canSeePlayerTimer / enemy.canSeePlayerMaxTime);
        PlayerManager.instance.sneakIndicatorImage.color = new Color(0f + alpha, 0f + alpha, 0f + alpha, 1f);
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
        if (distanceToDisturbance > 1.5f)
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