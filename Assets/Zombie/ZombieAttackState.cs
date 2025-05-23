using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackState : StateMachineBehaviour
{
    
    Transform player;
    NavMeshAgent agent;
    public float stopAttackingDistance = 2f;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // --- Initialization --- //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SoundManager.Instance.zombieChannel.isPlaying == false) {
            SoundManager.Instance.zombieChannel.PlayOneShot(SoundManager.Instance.zombieAttack);
        }

        LookAtPlayer();

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        // --- Check if the agent should stop Attacking --- ///
        if (distanceFromPlayer > stopAttackingDistance) {
            animator.SetBool("isAttacking", false);
        }
    }


    private void LookAtPlayer()
    {
        Vector3 direction = player.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.Instance.zombieChannel.Stop();
    }
}
