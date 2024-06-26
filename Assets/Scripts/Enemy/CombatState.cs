using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CombatState : IEnemyState
{
    private StatePatternEnemy enemy;
    private float fovTimer = 0.1f;
    private float shootingTime = 2f;
    private float shootingDelay = 2f;
    private float moveTimer;
    public float wanderingRadius = 10f;
    private float startSearchingMaxTime = 3f;
    private float startSearchingTimer = 0;
    public CombatState(StatePatternEnemy statePatternEnemy)
    {
        this.enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        FOVRoutine();
        enemy.distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        Chase();
    }
    public void OnTriggerEnter(Collider other)
    {
    }
    public void HearingArea()
    {
        if (enemy.distanceToPlayer < 6f && enemy.player.GetComponent<PlayerMovement>().moving && !enemy.player.GetComponent<PlayerMovement>().sneaking)
            ToAlertState();
    }
    public void ToAlertState()
    {
        enemy.agent.isStopped = true;
        enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
        enemy.GetComponentInChildren<Animator>().SetBool("RunAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.agent.speed = enemy.walkSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
        {
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.alertImage;
            PlayerManager.instance.sneakIndicatorImage.color = new Color(0f, 0f, 0f, 0f);
        }
        startSearchingTimer = 0;
        enemy.currentState = enemy.alertState;
    }
    public void ToCombatState()
    {
        EnemyManager.Instance.indicatorImage.enabled = true;
        EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.combatImage;
        startSearchingTimer = 0;
    }
    public void ToPatrolState()
    {
    }
    public void ToTrackingState()
    {
        enemy.agent.isStopped = false;
        enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
        enemy.GetComponentInChildren<Animator>().SetBool("RunAiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
        enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
        enemy.agent.speed = enemy.runningSpeed;
        EnemyManager.Instance.indicatorImage.enabled = true;
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
        {
            EnemyManager.Instance.indicatorImage.sprite = EnemyManager.Instance.trackingImage;
            PlayerManager.instance.sneakIndicatorImage.color = new Color(0f, 0f, 0f, 0f);
        }
        enemy.lastKnownPlayerPosition = enemy.player.transform.position;
        startSearchingTimer = 0;
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
    void Scan()
    {
        enemy.rangeChecks = Physics.OverlapSphere(enemy.sensor.transform.position, enemy.distance, enemy.layers);
        GameObject player = enemy.player;
        IsInSight(player);
        if (enemy.canSeePlayer)
        {
            ToCombatState();
            enemy.StartCoroutine(enemy.CallReinforcements());
        }
        /*else
            ToTrackingState();*/
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
            if (enemy.distanceToPlayer < enemy.battleRadius)
                enemy.canSeePlayer = true;
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
                        if (enemy.distanceToPlayer < enemy.battleRadius && !enemy.playerMovementScript.sneaking)
                            enemy.canSeePlayer = true;
                        else if (enemy.distanceToPlayer < enemy.radius && enemy.playerMovementScript.sneaking)
                            enemy.canSeePlayer = true;
                    }
                }
                else
                    enemy.canSeePlayer = false;
            }
        }
        else if (enemy.canSeePlayer)
            enemy.canSeePlayer = false;
        if (enemy.canSeePlayer)
        {
            ToCombatState();
            enemy.StartCoroutine(enemy.CallReinforcements());
        }
        else
            ToTrackingState();*/
    }
    void Chase()
    {
        enemy.GetComponentInChildren<Animator>().SetBool("Walk", false);
        if (enemy.distanceToPlayer > 20)
        {
            if (enemy.canSeePlayer)
            {
                if (shootingTime > 0 && enemy.canSeePlayer)
                {
                    enemy.transform.LookAt(enemy.player.transform.position);
                    enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
                    enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
                    enemy.agent.isStopped = true;
                    shootingTime -= Time.deltaTime;
                    if (enemy.readyToShoot && !enemy.enemyHealth.takingHit)
                        enemy.Shoot();
                    else if (enemy.enemyHealth.takingHit)
                        enemy.Invoke("RecoverFromHit", 1);
                }
                else
                {
                    shootingDelay -= Time.deltaTime;
                    if (shootingDelay < 0)
                    {
                        shootingDelay = 2;
                        shootingTime = 2;
                    }
                    enemy.agent.isStopped = false;
                    enemy.agent.speed = enemy.runningSpeed;
                    enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
                    enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
                    enemy.agent.SetDestination(enemy.player.transform.position);
                }
            }
            else
            {
                enemy.agent.isStopped = false;
                enemy.agent.speed = enemy.runningSpeed;
                enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
                enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
                enemy.agent.SetDestination(enemy.player.transform.position);
                startSearchingTimer += Time.deltaTime;
                if (startSearchingTimer > startSearchingMaxTime)
                    ToTrackingState();
            }
        }
        else
        {
            if (shootingTime > 0 && enemy.canSeePlayer)
            {
                enemy.transform.LookAt(enemy.player.transform.position);
                enemy.GetComponentInChildren<Animator>().SetBool("Running", false);
                enemy.GetComponentInChildren<Animator>().SetBool("Aiming", true);
                enemy.agent.isStopped = true;
                shootingTime -= Time.deltaTime;
                if (enemy.readyToShoot && !enemy.enemyHealth.takingHit)
                    enemy.Shoot();
                else if (enemy.enemyHealth.takingHit)
                    enemy.Invoke("RecoverFromHit", 1);
            }
            else if(enemy.canSeePlayer)
            {
                shootingDelay -= Time.deltaTime;
                if (shootingDelay < 0)
                {
                    shootingDelay = 2;
                    shootingTime = 2;
                }
                moveTimer += Time.deltaTime;
                if (moveTimer > Random.Range(1, 3))
                {
                    enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
                    enemy.GetComponentInChildren<Animator>().SetBool("Running", true);

                    Vector3 newPos = RandomNavSphere(enemy.transform.position, wanderingRadius, -1);
                    enemy.agent.speed = enemy.runningSpeed;
                    enemy.agent.SetDestination(newPos);
                    enemy.agent.isStopped = false;
                    moveTimer = 0;
                }
            }
            else
            {
                enemy.agent.isStopped = false;
                enemy.agent.speed = enemy.runningSpeed;
                enemy.GetComponentInChildren<Animator>().SetBool("Aiming", false);
                enemy.GetComponentInChildren<Animator>().SetBool("Running", true);
                enemy.agent.SetDestination(enemy.player.transform.position);
                startSearchingTimer += Time.deltaTime;
                if (startSearchingTimer > startSearchingMaxTime)
                    ToTrackingState();
            }
        }
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