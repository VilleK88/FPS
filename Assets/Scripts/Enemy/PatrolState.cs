using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.RuleTile.TilingRuleOutput;
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
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        FOVRoutine();
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
        EnemyManager.Instance.indicatorImage.enabled = true;
        enemy.canSeePlayerTimer = 0;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.trackingImage;
        enemy.currentState = enemy.trackingState;
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