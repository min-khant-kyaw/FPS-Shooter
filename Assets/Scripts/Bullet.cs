using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;

    private void OnCollisionEnter(Collision objectHit)
    {
        if (objectHit.gameObject.CompareTag("Target")) {
            print("hit " + objectHit.gameObject.name + " !");
            CreateBulletImpactEffect(objectHit);
            Destroy(gameObject);
        }
        
        if (objectHit.gameObject.CompareTag("Wall")) {
            print("hits a wall");
            CreateBulletImpactEffect(objectHit);
            Destroy(gameObject);
        }
        
        if (objectHit.gameObject.CompareTag("Zombie")) {
            print("hits a Zombie");
            objectHit.gameObject.GetComponent<Zombie>().TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            
        );
    
        hole.transform.SetParent(objectHit.gameObject.transform);
    }

}
