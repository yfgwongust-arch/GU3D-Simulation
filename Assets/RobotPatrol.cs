using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotPatrol : MonoBehaviour
{
    [Header("Patrol Setup")]
    [Tooltip("Drag your scene waypoint GameObjects here.")]
    public Transform[] waypoints;

    [Tooltip("Pause duration (seconds) at each waypoint before moving to the next.")]
    public float waitTimeAtWaypoint = 1.0f;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            MoveToCurrentWaypoint();
        }
        else
        {
            Debug.LogWarning("[RobotPatrol] Please assign waypoints in the Inspector!");
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // Check if the agent has reached its target destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                // Move to the next waypoint, looping back to 0 at the end
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                MoveToCurrentWaypoint();
            }
        }
    }

    void MoveToCurrentWaypoint()
    {
        if (waypoints[currentWaypointIndex] != null)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}