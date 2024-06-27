using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PatrolState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.2f;
    public int waypointIndex;
    float waypointCounter = 0;
    float waypointMaxTime = 4;
    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        HearingArea();
        Patrol();
        if (enemy.canSeePlayer)
        {
            DetectionTimeUI();
            if (!enemy.playerMovementScript.sneaking)
                ToCombatState();
            else if (enemy.canSeePlayerTimer < enemy.canSeePlayerMaxTime)
                enemy.canSeePlayerTimer += Time.deltaTime;
            else
                ToCombatState();
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
        if (enemy.distanceToPlayer < 6 && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
            ToAlertState();
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
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
        enemy.lastKnownPlayerPosition = enemy.player.transform.position;
        EnemyManager.Instance.indicatorImage.enabled = true;
        enemy.canSeePlayerTimer = 0;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.alertImage;
        enemy.currentState = enemy.alertState;
    }
    public void ToCombatState()
    {
        enemy.lastKnownPlayerPosition = enemy.player.transform.position;
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.combatImage;
        enemy.canSeePlayerTimer = 0;
        enemy.currentState = enemy.combatState;
    }
    public void ToPatrolState()
    {
    }
    public void ToTrackingState()
    {
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", true);
        EnemyManager.Instance.indicatorImage.enabled = true;
        enemy.canSeePlayerTimer = 0;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.trackingImage;
        enemy.currentState = enemy.trackingState;
    }
    void DetectionTimeUI()
    {
        float alpha = Mathf.Clamp01(enemy.canSeePlayerTimer / enemy.canSeePlayerMaxTime);
        PlayerManager.instance.sneakIndicatorImage.color = new Color(0f + alpha, 0f + alpha, 0f + alpha, 1f);
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