using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;

    private NavMeshAgent navAgent;

    public bool isDead;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        
        if (HP <= 0) {
            Die();
        } else {
            animator.SetTrigger("DAMAGE");
            SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieHurt);
        }
    }

    
    private void Die()
    {
        isDead = true;

        // Play a random death animation
        int randomValue = Random.Range(0, 2);
        animator.SetTrigger(randomValue == 0 ? "DIE1" : "DIE2");

        // Play death sound
        SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieDeath);

        // Disable AI movement
        if (navAgent != null) 
        {
            navAgent.isStopped = true;
            navAgent.enabled = false;
        }

        // Disable colliders to prevent interaction with the dead body
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Remove Rigidbody physics (if present)
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Remove zombie from tracking list (if using ZombieSpawnController)
        ZombieSpawnController controller = FindObjectOfType<ZombieSpawnController>();
        if (controller != null)
        {
            controller.currentZombiesAlive.Remove(this);
        }

        // Destroy the body after a few seconds
        StartCoroutine(RemoveBodyAfterDelay(5f));
    }

    private IEnumerator RemoveBodyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f); // Attacking // Stop Attacking
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 20f); // Detection (Start Chasing)

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 22f); // Stop Chasing
    }
}
