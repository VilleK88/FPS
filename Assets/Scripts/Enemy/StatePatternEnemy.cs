using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class StatePatternEnemy : MonoBehaviour
{
    public float searchDuration; // AlertState searching time
    public int randomEnemyTurn;
    public Vector3 lastKnownPlayerPosition;
    [Header("Field of View")]
    public float radius = 40; // radius enemy is seeing the player if he's not sneaking
    public float sneakRadius = 20; // radius enemy is seeing the player if he's sneaking
    public float battleRadius = 50;
    [Range(0, 360)] public float angle = 140;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;
    public Color closeColor = new Color(0, 0, 0, 1f);
    public Color farColor = new Color(0, 0, 0, 0f);
    [HideInInspector] public Collider[] rangeChecks;
    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 directionToTarget;
    public float distanceToPlayer;
    [Header("Patrol")]
    public Transform[] waypoints;
    public bool randomPatrol = false;
    float callReinforcementsDistance = 30;
    [Header("Move Speed")]
    public float walkSpeed = 3.5f;
    public float runningSpeed = 5f;
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
    public float hearingPlayerShootRadius = 30f;
    public EnemyHealth enemyHealth;
    [HideInInspector] public GameObject player;
    [HideInInspector] public IEnemyState currentState; // current state is defined here
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public AlertState alertState;
    [HideInInspector] public CombatState combatState;
    [HideInInspector] public TrackingState trackingState;
    [HideInInspector] public NavMeshAgent agent;
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
        currentState.UpdateState();
    }
    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
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
        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);
        return direction + new Vector3(x, y, 0); // returning the shooting direction and spread
    }
    public IEnumerator CallReinforcements()
    {
        yield return new WaitForSeconds(0.5f);
        StatePatternEnemy[] enemies = FindObjectsOfType<StatePatternEnemy>();
        foreach (StatePatternEnemy enemy in enemies)
        {
            Transform enemyTransform = enemy.transform;
            StatePatternEnemy stateEnemy = enemy.GetComponent<StatePatternEnemy>();
            if(enemyTransform != null && stateEnemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemyTransform.position);
                if(distance < callReinforcementsDistance)
                {
                    stateEnemy.lastKnownPlayerPosition = stateEnemy.player.transform.position;
                    stateEnemy.GetComponentInChildren<Animator>().SetBool("WalkAiming", false);
                    stateEnemy.currentState = stateEnemy.combatState;
                }
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if(currentState == trackingState)
            {
                agent.isStopped = false;
                GetComponentInChildren<Animator>().SetBool("WalkAiming", true);
                agent.SetDestination(transform.position + (Random.insideUnitSphere * 5));
            }
        }
    }
}