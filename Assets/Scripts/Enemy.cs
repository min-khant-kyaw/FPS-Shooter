using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;

    private NavMeshAgent navAgent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        
        if (HP <= 0) {
            int randomValue = Random.Range(0, 2);
            if (randomValue == 0) {
                animator.SetTrigger("DIE1");
            } else {            
                animator.SetTrigger("DIE2");
            }
            GetComponent<CapsuleCollider>().enabled = false;
        } else {
            animator.SetTrigger("DAMAGE");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f); // Attacking // Stop Attacking
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f); // Detection (Start Chasing)

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f); // Stop Chasing
    }
}
