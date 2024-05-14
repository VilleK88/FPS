using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StatePatternEnemy : MonoBehaviour
{
    public float searchDuration; // AlertState searching time
    public float searchTurnSpeed; // AlertState turn speed
    public float sightRange; // line of sight
    public Transform[] waypoints;
    public Transform eye; // Eye where the raycast is
    public MeshRenderer indicator;
    public Vector3 lastKnownPlayerPosition;
    [HideInInspector] public Transform chaseTarget; // target that is chased
    [HideInInspector] public IEnemyState currentState; // current state is defined here
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public AlertState alertState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public TrackingState trackingState;
    [HideInInspector] public NavMeshAgent agent;
    private void Awake()
    {
        patrolState = new PatrolState(this);
        alertState = new AlertState(this);
        chaseState = new ChaseState(this);
        trackingState = new TrackingState(this);
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        currentState = patrolState;
    }
    private void Update()
    {
        currentState.UpdateState();
    }
    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }
}