using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance {get; set;}

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables General")]
    public float throwForce = 10f;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0f;
    public float forceMultiplierLimit = 2f;

    [Header("Lethals")]
    public int lethalsCount = 0;
    public int maxLethalCount = 2;
    public Throwable.ThrowableType equippedLethalType;
    public GameObject grenadePrefab;
    
    [Header("Tacticals")]
    public int tacticalCount = 0;
    public int maxTacticalCount = 2;
    public Throwable.ThrowableType equippedTacticalType;
    public GameObject smokeGrenadePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;
    }

    public void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots) {
            if (weaponSlot == activeWeaponSlot) {
                weaponSlot.SetActive(true);
            } else {
                weaponSlot.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SwitchActiveSlot(1);
        }


        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T)) {
            forceMultiplier += Time.deltaTime;
            if (forceMultiplier > forceMultiplierLimit) {
                forceMultiplier = forceMultiplierLimit;
            }
        }
        if (Input.GetKeyUp(KeyCode.G)) {
            if (lethalsCount > 0) {
                ThrowLethal();
            }
            forceMultiplier = 0;
        }

        
        if (Input.GetKeyUp(KeyCode.T)) {
            if (tacticalCount > 0) {
                ThrowTactical();
            }
            forceMultiplier = 0;
        }

    }

    #region || ---- Weapons ---- ||
    public void PickUpWeapon(GameObject pickedUpWeapon)
    {
        AddWeaponIntoActiveSlot(pickedUpWeapon);
        pickedUpWeapon.GetComponent<BoxCollider>().enabled = false;
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedUpWeapon)
    {
        DropCurrentWeapon(pickedUpWeapon);

        pickedUpWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedUpWeapon.GetComponent<Weapon>();

        pickedUpWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    private void DropCurrentWeapon(GameObject pickedUpWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0) {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;
            weaponToDrop.GetComponent<BoxCollider>().enabled = true;
             
            weaponToDrop.transform.SetParent(pickedUpWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedUpWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedUpWeapon.transform.localRotation;

        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0) {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }
        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0) {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }        
    }
    #endregion

    #region || ---- Ammo ---- ||
    internal void PickUpAmmo(AmmoBox ammoBox)
    {
        switch (ammoBox.ammoType) {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammoBox.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammoBox.ammoAmount;
                break;
            
        }
        
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel) {
            case Weapon.WeaponModel.M1911:
                totalPistolAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.M4:
                totalRifleAmmo -= bulletsToDecrease;
                break;
        }
    }

    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel) {
            case Weapon.WeaponModel.M1911:
                return totalPistolAmmo;
            case Weapon.WeaponModel.M4:
                return totalRifleAmmo;
            default:
                return 0;
        }
    }

    #endregion

    #region || ---- Throwables ---- ||
    public void PickUpThrowable(Throwable throwable)
    {
        
        switch (throwable.throwableType) {
            case Throwable.ThrowableType.Grenade:
                PickUpThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke_Grenade:
                PickUpThrowableAsTactical(Throwable.ThrowableType.Smoke_Grenade);
                break;
        }
    }

    private void PickUpThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None) {
            equippedLethalType = lethal;

            if (lethalsCount < maxLethalCount) {
                lethalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else {
                print("Lethals limit reached");
            }
        } else {
            // Cannot pickup different lethal
            // Option to swap lethals
        }
    }

    
    private void PickUpThrowableAsTactical(Throwable.ThrowableType tactical) 
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None) {
            equippedTacticalType = tactical;

            if (tacticalCount < maxTacticalCount) {
                tacticalCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else {
                print("Tactical limit reached");
            }
        } else {
            // Cannot pickup different tacticals
            // Option to swap tacticals
        }
    }

    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);
        
        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;
    
        lethalsCount -= 1;

        if (lethalsCount <= 0) {
            equippedLethalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);
        
        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;
    
        tacticalCount -= 1;

        if (tacticalCount <= 0) {
            equippedTacticalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType throwableType)
    {
        switch (throwableType) {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke_Grenade:
                return smokeGrenadePrefab;
        }

        return new();
    }

    #endregion

}
