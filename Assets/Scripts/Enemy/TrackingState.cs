using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TrackingState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.2f;
    private float searchTimer;
    public float moveTimer;
    private bool startSearchTimer;
    public float wanderingRadius = 20f;
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
        if (enemy.canSeePlayer)
        {
            DetectionTimeUI();
            if (!enemy.playerMovementScript.sneaking)
                ToCombatState();
            else if (enemy.canSeePlayerTimer < enemy.canSeePlayerAlertedMaxTime)
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
        if (enemy.distanceToPlayer < 6f && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
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
    public void FOVRoutine()
    {
        if (fovTimer > 0)
            fovTimer -= Time.deltaTime;
        else
        {
            // FieldOfViewCheck();
            Scan();
            fovTimer = 0;
        }
        /*enemy.scanTimer -= Time.deltaTime;
        if (enemy.scanTimer < 0)
        {
            enemy.scanTimer += enemy.scanInterval;
            Scan();
        }*/
    }
    void Scan()
    {
        enemy.rangeChecks = Physics.OverlapSphere(enemy.sensor.transform.position, enemy.distance, enemy.layers);
        GameObject player = enemy.player;
        IsInSight(player);
    }
    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = enemy.sensor.transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if (direction.y < 0 || direction.y > enemy.height)
        {
            enemy.canSeePlayer = false;
            return false;
        }
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, enemy.sensor.transform.forward);
        if (deltaAngle > enemy.angle)
        {
            enemy.canSeePlayer = false;
            return false;
        }
        origin.y += enemy.height / 2;
        dest.y = origin.y;
        if (!Physics.Linecast(origin, dest, enemy.occlusionLayers))
        {
            enemy.playerMovementScript = enemy.player.GetComponent<PlayerMovement>();
            if (enemy.playerMovementScript != null)
            {
                if (enemy.distanceToPlayer < enemy.radius && !enemy.playerMovementScript.sneaking)
                    enemy.canSeePlayer = true;
                else if (enemy.distanceToPlayer < enemy.sneakRadius && enemy.playerMovementScript.sneaking)
                    enemy.canSeePlayer = true;
            }
        }
        else
        {
            enemy.canSeePlayer = false;
            return false;
        }
        return true;
    }
    void FieldOfViewCheck()
    {
        /*enemy.rangeChecks = Physics.OverlapSphere(enemy.transform.position, enemy.radius, enemy.targetMask);
        if (enemy.rangeChecks.Length != 0)
        {
            enemy.target = enemy.rangeChecks[0].transform;
            enemy.directionToTarget = (enemy.target.position - enemy.transform.position).normalized;
            if (Vector3.Angle(enemy.transform.forward, enemy.directionToTarget) < enemy.angle / 2)
            {
                if (!Physics.Raycast(enemy.transform.position, enemy.directionToTarget, enemy.distanceToPlayer, enemy.obstructionMask))
                {
                    enemy.playerMovementScript = enemy.player.GetComponent<PlayerMovement>();
                    if (enemy.playerMovementScript != null)
                    {
                        if (enemy.distanceToPlayer < enemy.radius && !enemy.playerMovementScript.sneaking)
                            enemy.canSeePlayer = true;
                        else if (enemy.distanceToPlayer < enemy.sneakRadius && enemy.playerMovementScript.sneaking)
                            enemy.canSeePlayer = true;
                    }
                }
                else
                    enemy.canSeePlayer = false;
            }
        }
        else if (enemy.canSeePlayer)
            enemy.canSeePlayer = false;*/
    }
    void DetectionTimeUI()
    {
        float alpha = Mathf.Clamp01(enemy.canSeePlayerTimer / enemy.canSeePlayerMaxTime);
        PlayerManager.instance.sneakIndicatorImage.color = new Color(0f + alpha, 0f + alpha, 0f + alpha, 1f);
    }
    void Hunt()
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