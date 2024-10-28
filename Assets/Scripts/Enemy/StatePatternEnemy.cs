using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[Serializable]
public class EnemyData
{
    public string enemyDataID;
    public float enemyPositionX;
    public float enemyPositionY;
    public float enemyPositionZ;
    public float enemyRotationX;
    public float enemyRotationY;
    public float enemyRotationZ;
    public int waypointIndexData;
    public bool dead;
    public bool alreadyFoundDead;
    public RagdollData ragdollData;
}
[Serializable]
public class BoneData
{
    public string boneName;
    public float bonePositionX;
    public float bonePositionY;
    public float bonePositionZ;
    public float boneRotationX;
    public float boneRotationY;
    public float boneRotationZ;
}
[Serializable]
public class RagdollData
{
    public List<BoneData> bones = new List<BoneData>();
}
public class StatePatternEnemy : MonoBehaviour
{
    public float searchDuration; // AlertState searching time
    public int randomEnemyTurn;
    public Vector3 lastKnownPlayerPosition;
    [Header("Field of View")]
    public GameObject sensor;
    public GameObject eyes;
    public List<GameObject> Objects = new List<GameObject>();
    private float fovTimer = 0.2f;
    public float radius = 50; // radius enemy is seeing the player if he's not sneaking
    public float sneakRadius = 20; // radius enemy is seeing the player if he's sneaking
    public float battleRadius = 60;
    public bool canSeePlayer;
    public Color closeColor = new Color(0, 0, 0, 1f);
    public Color farColor = new Color(0, 0, 0, 0f);
    public float distance = 10;
    public float angle = 30;
    public float height = 1.0f;
    public Color meshColor = Color.red;
    public LayerMask playerLayer;
    public LayerMask obstructionLayer;
    public LayerMask enemyLayer;
    Collider[] colliders = new Collider[50];
    [HideInInspector] public Mesh mesh;
    [HideInInspector] public int count;
    [HideInInspector] public Collider[] rangeChecks;
    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 directionToPlayer;
    public float distanceToPlayer;
    public float canSeePlayerTimer = 0;
    public float canSeePlayerMaxTime = 2f; // not alerted
    public float canSeePlayerAlertedMaxTime = 1f; // alerted
    public PlayerMovement playerMovementScript;
    [Header("Patrol")]
    public Transform[] waypoints;
    public bool randomPatrol = false;
    float callReinforcementsDistance = 60;
    [Header("Move Speed")]
    public float walkSpeed = 3.5f;
    public float runningSpeed = 6f;
    [Header("Shooting")]
    public Transform shootingPoint;
    public GameObject enemyBulletPrefab;
    public GameObject muzzleFlash;
    public float bulletVelocity = 300;
    public float spreadIntensity = 2f;
    public float bulletDamage = 1f;
    public string bulletTarget = "Player";
    public bool readyToShoot = true;
    public float shootingDelay = 0.3f;
    public float hearingPlayerShootRadius = 40f;
    public EnemyHealth enemyHealth;
    [HideInInspector] public GameObject player;
    [HideInInspector] public IEnemyState currentState; // current state is defined here
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public AlertState alertState;
    [HideInInspector] public CombatState combatState;
    [HideInInspector] public TrackingState trackingState;
    [HideInInspector] public NavMeshAgent agent;
    public string enemyID;
    private void Awake()
    {
        patrolState = new PatrolState(this);
        alertState = new AlertState(this);
        combatState = new CombatState(this);
        trackingState = new TrackingState(this);
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        player = PlayerManager.instance.GetPlayer();
        currentState = patrolState;
        enemyHealth = GetComponent<EnemyHealth>();
    }
    private void Update()
    {
        directionToPlayer = player.transform.position - transform.position;
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        FOVRoutine();
        Debug.DrawRay(eyes.transform.position, directionToPlayer, Color.green);
        currentState.UpdateState();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!enemyHealth.dead)
            currentState.OnTriggerEnter(other);
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
        rangeChecks = Physics.OverlapSphere(sensor.transform.position, distance, playerLayer);
        GameObject playerObj = player;
        IsPlayerInSight(playerObj);
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, enemyLayer, QueryTriggerInteraction.Collide);
        Objects.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsDeadEnemyInSight(obj))
            {
                Objects.Add(obj);
            }
        }
        if (canSeePlayer && currentState == combatState)
            StartCoroutine(CallReinforcementsToCombat());
    }
    public bool IsPlayerInSight(GameObject obj)
    {
        Vector3 origin = sensor.transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if (direction.y < 0 || direction.y > height)
        {
            canSeePlayer = false;
            return false;
        }
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, sensor.transform.forward);
        if (deltaAngle > angle)
        {
            canSeePlayer = false;
            return false;
        }
        origin.y += height / 2;
        dest.y = origin.y;
        if(!Physics.Raycast(eyes.transform.position, directionToPlayer, distanceToPlayer, obstructionLayer))
        {
            playerMovementScript = player.GetComponent<PlayerMovement>();
            if (playerMovementScript != null)
            {
                if (distanceToPlayer < radius && !playerMovementScript.sneaking)
                    canSeePlayer = true;
                else if (distanceToPlayer < sneakRadius && playerMovementScript.sneaking)
                    canSeePlayer = true;
                return true;
            }
        }
        canSeePlayer = false;
        return false;
    }
    public bool IsDeadEnemyInSight(GameObject obj)
    {
        Vector3 origin = sensor.transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        float distanceToEnemy = Vector3.Distance(transform.position, dest);
        if (direction.y < 0 || direction.y > height)
        {
            return false;
        }
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, sensor.transform.forward);
        if (deltaAngle > angle)
        {
            return false;
        }
        origin.y += height / 2;
        dest.y = origin.y;
        if (!Physics.Raycast(eyes.transform.position, direction, distanceToEnemy, obstructionLayer))
        {
            EnemyHealth enemyHealthScript = obj.GetComponent<EnemyHealth>();
            if(enemyHealthScript != null)
            {
                if(distanceToEnemy < 30 && !enemyHealthScript.alreadyFoundDead && enemyHealthScript.dead)
                {
                    enemyHealthScript.alreadyFoundDead = true;
                    enemyHealthScript.StartCoroutine(enemyHealthScript.Vanish());
                    if(currentState != combatState && !canSeePlayer)
                    {
                        lastKnownPlayerPosition = enemyHealthScript.transform.position;
                        Debug.Log("Dead enemy found");
                        if (currentState == patrolState)
                            patrolState.ToTrackingState();
                        else if (currentState == alertState)
                            patrolState.ToTrackingState();
                        //StartCoroutine(CallReinforcements());
                    }
                    return true;
                }
            }
        }
        return false;
    }
    public void Shoot()
    {
        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        GameObject bullet = Instantiate(enemyBulletPrefab, shootingPoint.position, shootingPoint.rotation);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        bullet.GetComponent<EnemyBullet>().target = bulletTarget;
        bullet.GetComponent<EnemyBullet>().damage = bulletDamage;
        muzzleFlash.GetComponent<ParticleSystem>().Play();
        Invoke("ResetShot", shootingDelay);
    }
    public void ResetShot()
    {
        readyToShoot = true;
    }
    public void RecoverFromHit()
    {
        enemyHealth.takingHit = false;
    }
    public Vector3 CalculateDirectionAndSpread()
    {
        Vector3 direction = player.transform.position - shootingPoint.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        return direction + new Vector3(x, y, 0); // returning the shooting direction and spread
    }
    public IEnumerator CallReinforcementsToCombat() // to combat
    {
        yield return new WaitForSeconds(0.5f);
        StatePatternEnemy[] enemies = FindObjectsOfType<StatePatternEnemy>();
        foreach (StatePatternEnemy enemy in enemies)
        {
            Transform enemyTransform = enemy.transform;
            StatePatternEnemy stateEnemy = enemy.GetComponent<StatePatternEnemy>();
            EnemyHealth enemyHealthScript = enemy.GetComponent<EnemyHealth>();
            if (enemyTransform != null && stateEnemy != null && enemyHealthScript != null)
            {
                if (!enemyHealthScript.dead)
                {
                    float distance = Vector3.Distance(transform.position, enemyTransform.position);
                    if (distance < callReinforcementsDistance && stateEnemy.currentState != stateEnemy.combatState)
                    {
                        stateEnemy.lastKnownPlayerPosition = stateEnemy.player.transform.position;
                        if (stateEnemy.currentState == stateEnemy.patrolState)
                            stateEnemy.patrolState.ToCombatState();
                        else if (stateEnemy.currentState == stateEnemy.alertState)
                            stateEnemy.alertState.ToCombatState();
                        else if (stateEnemy.currentState == stateEnemy.trackingState)
                            stateEnemy.trackingState.ToCombatState();
                    }
                }
            }
        }
    }
    public IEnumerator CallReinforcements() // when dead body found
    {
        yield return new WaitForSeconds(0.5f);
        StatePatternEnemy[] enemies = FindObjectsOfType<StatePatternEnemy>();
        foreach (StatePatternEnemy enemy in enemies)
        {
            Transform enemyTransform = enemy.transform;
            StatePatternEnemy stateEnemy = enemy.GetComponent<StatePatternEnemy>();
            EnemyHealth enemyHealthScript = enemy.GetComponent<EnemyHealth>();
            if (enemyTransform != null && stateEnemy != null && enemyHealthScript != null)
            {
                if (!enemyHealthScript.dead)
                {
                    float distance = Vector3.Distance(transform.position, enemyTransform.position);
                    if (distance < callReinforcementsDistance)
                    {
                        if(stateEnemy.currentState != combatState && stateEnemy.currentState != trackingState)
                        {
                            stateEnemy.lastKnownPlayerPosition = transform.position;
                            if (stateEnemy.currentState == stateEnemy.patrolState)
                                stateEnemy.patrolState.ToCombatState();
                            else if (stateEnemy.currentState == stateEnemy.alertState)
                                stateEnemy.alertState.ToCombatState();
                        }
                    }
                }
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (currentState == trackingState)
            {
                agent.isStopped = false;
                GetComponentInChildren<Animator>().SetBool("WalkAiming", true);
                agent.SetDestination(transform.position + (UnityEngine.Random.insideUnitSphere * 5));
            }
        }
    }
    Mesh CreateWedgeMesh()
    {
        mesh = new Mesh();
        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        int vert = 0;
        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;
        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;
        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;
        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;
        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;
            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;
            currentAngle += deltaAngle;
            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;
            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
            // top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
            // bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;
        }
        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
    }
    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, sensor.transform.position, sensor.transform.rotation);
        }
        Gizmos.DrawWireSphere(sensor.transform.position, distance);
        for (int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);
        }
    }
    [ContextMenu("Generate GUID FOR ID")]
    public void GenerateID()
    {
        enemyID = System.Guid.NewGuid().ToString();
    }
}