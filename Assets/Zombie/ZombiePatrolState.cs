using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolState : StateMachineBehaviour
{
    float timer;
    public float patrollingTime = 200f;

    Transform player;
    NavMeshAgent agent;

    public float detectionAreaRadius = 20f;
    public float patrolSpeed = 1f;
    public float detectionHeightThreshold  = 5f;


    List<Transform> waypointsList = new List<Transform>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // --- Initialization --- //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;
        timer = 0;

        // --- Get all waypoints and Move to First Waypoint --- //
        GameObject waypointCluster = GameObject.FindGameObjectWithTag("Waypoints");
        foreach (Transform t in waypointCluster.transform) {
            waypointsList.Add(t);
        }

        if (agent != null && agent.enabled && agent.isOnNavMesh) {
            Vector3 nextPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
            agent.SetDestination(nextPosition);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SoundManager.Instance.zombieChannel.isPlaying == false) {
            SoundManager.Instance.zombieChannel.clip = SoundManager.Instance.zombieWalk;
            SoundManager.Instance.zombieChannel.PlayDelayed(1f);
        }

        // --- Check if agent arrived at selected waypoint, then move to another waypoint --- //
        if (agent != null && agent.enabled && agent.isOnNavMesh && agent.remainingDistance <= agent.stoppingDistance) {
            agent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
        }

        // --- Transition back to Idle state once patrol time is over --- //
        timer += Time.deltaTime;
        if (timer > patrollingTime) {
            animator.SetBool("isPatrolling", false);
        }

        // --- Transition to Chase State (but only if within valid height range) --- //
        Vector3 playerPos = player.position;
        Vector3 zombiePos = animator.transform.position;

        // Calculate horizontal (XZ) distance
        float horizontalDistance = Vector2.Distance(
            new Vector2(playerPos.x, playerPos.z), 
            new Vector2(zombiePos.x, zombiePos.z)
        );

        // Calculate vertical (Y-axis) difference
        float verticalDifference = Mathf.Abs(playerPos.y - zombiePos.y);

        // Only start chasing if within detection range AND within valid height difference
        if (horizontalDistance < detectionAreaRadius && verticalDifference <= detectionHeightThreshold) {
            animator.SetBool("isChasing", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh) {
            // Stop the agent
            agent.SetDestination(animator.transform.position);
            SoundManager.Instance.zombieChannel.Stop();
        }
    }
}
