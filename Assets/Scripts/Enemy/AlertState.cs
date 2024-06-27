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
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
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
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", true);
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
    public void FOVRoutine()
    {
        if (fovTimer > 0)
            fovTimer -= Time.deltaTime;
        else
        {
            Scan();
            fovTimer = 0;
        }
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