using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : StateMachineBehaviour
{
    Transform player;
    NavMeshAgent agent;
    
    public float chaseSpeed = 4f; // Default speed
    public float focusedSpeed = 3.75f; // Speed when player is Focused
    public float relaxedSpeed = 4f; // Speed when player is Relaxed

    public float stopChasingDistance = 55f;
    public float attackingDistance = 1f;
    public float chaseHeightThreshold = 20f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // --- Initialization --- //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = chaseSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SoundManager.Instance.zombieChannel.isPlaying == false) {
            SoundManager.Instance.zombieChannel.PlayOneShot(SoundManager.Instance.zombieChase);
        }

        // Get player and zombie positions
        Vector3 playerPos = player.position;
        Vector3 zombiePos = animator.transform.position;

        // Calculate horizontal distance (XZ-plane)
        float horizontalDistance = Vector2.Distance(
            new Vector2(playerPos.x, playerPos.z), 
            new Vector2(zombiePos.x, zombiePos.z)
        );

        // Calculate vertical distance (Y-axis)
        float verticalDifference = Mathf.Abs(playerPos.y - zombiePos.y);

        // Check if player is within valid range and height
        bool canDetectPlayer = horizontalDistance <= stopChasingDistance && verticalDifference <= chaseHeightThreshold;
        bool canAttackPlayer = horizontalDistance <= attackingDistance && verticalDifference <= chaseHeightThreshold;

        // Update chaseSpeed based on HUDManager's concentrationType
        if (HUDManager.Instance != null && HUDManager.Instance.concentrationType != null) {
            string concentration = HUDManager.Instance.concentrationType.text;
            if (concentration == "Focused") {
                chaseSpeed = focusedSpeed;
            } else if (concentration == "Relaxed") {
                chaseSpeed = relaxedSpeed;
            } else {
                chaseSpeed = relaxedSpeed;
                Debug.LogWarning($"Unexpected concentrationType.text: {concentration}. Using relaxedSpeed.");
            }
            agent.speed = chaseSpeed;
        }

        if (agent != null && agent.enabled && agent.isOnNavMesh) {
            agent.SetDestination(player.position);
            animator.transform.LookAt(new Vector3(playerPos.x, zombiePos.y, playerPos.z));
        }
        
        // --- Check if the player is out of range --- //
        animator.SetBool("isChasing", canDetectPlayer);

        // --- Check if the player is in attack range --- //
        animator.SetBool("isAttacking", canAttackPlayer);
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
