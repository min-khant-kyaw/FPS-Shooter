using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;
using UnityEngine.UIElements;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;
    
    float countdown;

    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType
    {
        Grenade,
        Flashbang,
        Smoke,
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown) {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && !hasExploded) {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        GetThrowableEffect();
    }


    private void GetThrowableEffect()
    {
        switch (throwableType) {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
        }
    }

    private void GrenadeEffect()
    {
        // Visual Effect
        GameObject explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders) {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            // Apply damage to any Enemies in the damageRadius
        }


    }

}
