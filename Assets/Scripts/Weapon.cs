using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Shooting related configs
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 0.2f;

    // Weapon Burst mode
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Weapon Recoil/Spread
    public float spreadIntensity = 0.01f;

    // Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 500;
    public float bulletPrefabLifeTime = 3f;
    public GameObject muzzleEffect;

    // Animator
    private Animator animator;

    // Reloading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public enum WeaponModel
    {
        M1911,
        M4
    }

    public WeaponModel thisWeaponModel;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    public void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        
        bulletsLeft = magazineSize;
    }

    void Update()
    {
        if (bulletsLeft == 0 && isShooting) {
            SoundManager.Instance.emptyMagazineSoundM1911.Play();
        }

        // Automatic mode only works when Holding down Left Mouse Button
        if (currentShootingMode == ShootingMode.Auto) {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        // These shooting mode works when Clicking Left Mouse Button
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst) {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false) {
            Reload();
        }

        if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0) {
            // Reload();
        }

        if (readyToShoot && isShooting && bulletsLeft > 0) {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

        if (AmmoManager.Instance.ammoDisplay != null) {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        }

    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);
        
        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        
        // Point the bullet towards Shooting Direction
        bullet.transform.forward = shootingDirection;
        
        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        
        // Destroy the bullet
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    
        // Check if we are done with shooting
        if (allowReset) {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }

    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        
        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

     private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // Ray comes out from middle of our screen to check where we are pointing at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit)) {
            // Hitting something
            targetPoint = hit.point;
        }
        else {
            // Shooting the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Returning the shooting direction and spread
        return direction + new Vector3(x, y, 0);
    }


    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
