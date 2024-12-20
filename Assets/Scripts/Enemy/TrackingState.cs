using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TrackingState : IEnemyState
{
    private StatePatternEnemy enemy;
    public float fovTimer = 0.2f;
    public float searchTimer;
    public float moveTimer;
    public bool startSearchTimer;
    public float wanderingRadius = 20f;
    public TrackingState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        HearingArea();
        Tracking();
        if (enemy.canSeePlayer)
        {
            DetectionTimeUI();
            if (!enemy.playerMovementScript.sneaking || enemy.distanceToPlayer < 3)
            {
                enemy.lastKnownPlayerPosition = enemy.player.transform.position;
                ToCombatState();
            }
            else if (enemy.canSeePlayerTimer < enemy.canSeePlayerAlertedMaxTime)
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
            ToAlertState();
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
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.isStopped = true;
        enemy.agent.speed = enemy.walkSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.alertImage;
        searchTimer = 0;
        startSearchTimer = false;
        enemy.canSeePlayerTimer = 0;
        enemy.currentState = enemy.alertState;
    }
    public void ToCombatState()
    {
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.combatImage;
        searchTimer = 0;
        startSearchTimer = false;
        enemy.canSeePlayerTimer = 0;
        enemy.currentState = enemy.combatState;
    }
    public void ToPatrolState()
    {
        enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", true);
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.walkSpeed;
        searchTimer = 0;
        moveTimer = 0;
        startSearchTimer = false;
        enemy.canSeePlayerTimer = 0;
        enemy.currentState = enemy.patrolState;
        if (EnemyManager.Instance.CloseIndicatorImage())
            EnemyManager.Instance.StartCoroutine(EnemyManager.Instance.BackToPatrol());
    }
    public void ToTrackingState()
    {
    }
    void DetectionTimeUI()
    {
        float alpha = Mathf.Clamp01(enemy.canSeePlayerTimer / enemy.canSeePlayerMaxTime);
        PlayerManager.instance.sneakIndicatorImage.color = new Color(0f + alpha, 0f + alpha, 0f + alpha, 1f);
    }
    void Tracking()
    {
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            enemy.agent.isStopped = true;
            enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
            enemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
            enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
            startSearchTimer = true;
        }
        if (startSearchTimer)
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
        Vector3 randDirection;
        do
        {
            randDirection = Random.insideUnitSphere * dist;
        }
        while (randDirection.magnitude < 5.0f);
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}