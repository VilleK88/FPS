using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider capsuleCollider;
    [SerializeField] Animator anim;
    [Header("Field of View")]
    public float radius = 10;
    [Range(0, 360)] public float angle = 140;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;
    Collider[] rangeChecks;
    Transform target;
    Vector3 directionToTarget;
    [Header("Chase, Attack and Agro")]
    public bool isAgro = false;
    float maxAgroCounter = 5;
    public float agroCounter = 0;
    NavMeshAgent agent;
    float originalSpeed;
    public float distanceToPlayer;
    public float stoppingDistance = 5;
    [Header("Patrol")]
    public Transform[] waypoints;
    int waypointIndex;
    public float waypointCounter = 0;
    float waypointMaxTime = 4;
    public bool randomPatrol = false;
    [Header("Disturbance Parameters")]
    public bool disturbance;
    public ThrowImpactEffect throwImpactEffect;
    public float lookAtDisturbanceCounter = 0;
    public float lookAroundCounter = 0;
    public Quaternion startRotation;
    bool checkStartRotation;
    public Quaternion currentRotation;
    public float startRotationY;
    public float currentRotationY;
    float rotationSpeed = 12;
    public int disturbanceTimes;
    public float disturbanceTimesCount = 10;
    [Header("Player")]
    GameObject player;
    [Header("Death booleans")]
    bool dead; // fetch from EnemyHealth -script
    bool playerDead; // fetch from PlayerHealth -script
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = PlayerManager.instance.GetPlayer();
        StartCoroutine(FOVRoutine());
        agent.SetDestination(waypoints[waypointIndex].position);
        originalSpeed = agent.speed;
        anim.GetComponent<Animator>().SetBool("Walk", true);
    }
    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(!dead)
        {
            if(canSeePlayer && !playerDead)
            {
                isAgro = true;
                agroCounter = 0;
                DisturbanceOver();
            }
            else
            {
                if(isAgro)
                {
                    if (agroCounter < maxAgroCounter)
                        agroCounter += Time.deltaTime;
                    else
                    {
                        agroCounter = 0;
                        isAgro = false;
                    }
                }
            }
            if(isAgro)
            {
                if (distanceToPlayer > stoppingDistance)
                {
                    Chase();
                }
                else if(distanceToPlayer < stoppingDistance)
                {
                    anim.GetComponent<Animator>().SetBool("Walk", false);
                    transform.LookAt(player.transform.position);
                    Attack();
                }
            }
            else
            {
                if(disturbance)
                {
                    if (lookAtDisturbanceCounter < 4)
                    {
                        lookAtDisturbanceCounter += Time.deltaTime;
                        LookAtDisturbance();
                    }
                    else
                        CheckDisturbance();
                }
                else
                {
                    Patrol();
                    if (waypointCounter < waypointMaxTime)
                        waypointCounter += Time.deltaTime;
                    else
                    {
                        agent.SetDestination(waypoints[waypointIndex].position);
                        anim.GetComponent<Animator>().SetBool("Walk", true);
                    }
                }
            }
        }
        if (playerDead)
            isAgro = false;
        DisturbanceAlertState();
    }
    void DisturbanceAlertState()
    {
        if(disturbance)
        {
            if (disturbanceTimes > 2)
            {
                isAgro = true;
                DisturbanceOver();
            }
        }
    }
    public void Disturbance() // this is called from the ThrowImpactEffect -script
    {
        disturbanceTimesCount = 10;
        lookAtDisturbanceCounter = 0;
        if (!canSeePlayer && !isAgro)
        {
            disturbance = true;
            agent.ResetPath();
            anim.GetComponent<Animator>().SetBool("Walk", false);
            throwImpactEffect = FindObjectOfType<ThrowImpactEffect>();
        }
    }
    void LookAtDisturbance()
    {
        if (throwImpactEffect != null)
        {
            Quaternion currentRotation = transform.rotation;
            Vector3 directionToImpact = throwImpactEffect.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToImpact);
            Quaternion newRotation = Quaternion.Slerp(currentRotation, targetRotation, 3 * Time.deltaTime);
            transform.rotation = newRotation;
        }
    }
    void CheckDisturbance()
    {
        if (throwImpactEffect != null)
        {
            agent.SetDestination(throwImpactEffect.transform.position);
            float distanceToImpactEffect = Vector3.Distance(transform.position, throwImpactEffect.transform.position);
            if (distanceToImpactEffect > 2.2f)
                anim.GetComponent<Animator>().SetBool("Walk", true);
            else
            {
                anim.GetComponent<Animator>().SetBool("Walk", false);
                if (!checkStartRotation)
                    StartLookingAround();
                if (lookAroundCounter < 10)
                {
                    lookAroundCounter += Time.deltaTime;
                    LookAround();
                }
                else
                    DisturbanceOver();
            }
        }
        else
            DisturbanceOver();
    }
    void StartLookingAround()
    {
        startRotation = transform.rotation;
        startRotationY = startRotation.eulerAngles.y;
        checkStartRotation = true;
    }
    void LookAround()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        currentRotation = transform.rotation;
        currentRotationY = currentRotation.eulerAngles.y;
        currentRotationY = (currentRotationY - startRotationY + 360) % 360;
        if (currentRotationY > 180)
            currentRotationY -= 360;
        if (currentRotationY > 40)
            rotationSpeed = -Mathf.Abs(rotationSpeed);
        else if (currentRotationY < -40)
            rotationSpeed = Mathf.Abs(rotationSpeed);
    }
    public void DisturbanceOver()
    {
        disturbance = false;
        checkStartRotation = false;
        lookAroundCounter = 0;
        lookAtDisturbanceCounter = 0;
        disturbanceTimes = 0;
        disturbanceTimesCount = 0;
    }
    void Patrol()
    {
        agent.speed = originalSpeed;
        if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 1.5f)
        {
            if (randomPatrol)
                waypointIndex = Random.Range(0, 5);
            else
                waypointIndex++;
            waypointCounter = 0;
            anim.GetComponent<Animator>().SetBool("Walk", false);
            if (waypointIndex >= waypoints.Length)
                waypointIndex = 0;
        }
    }
    void Attack()
    {
        agent.isStopped = true;
    }
    void Chase()
    {
        anim.GetComponent<Animator>().SetBool("Walk", true);
        agent.isStopped = false;
        agent.speed = 3.8f;
        agent.SetDestination(player.transform.position);
    }
    IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while(true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }
    void FieldOfViewCheck()
    {
        rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToPlayer, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isAgro = true;
            agroCounter = 0;
        }
    }
}