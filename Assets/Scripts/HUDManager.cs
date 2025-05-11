using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    [Header("EEG")]
    public Slider concentrationLevel;
    public TextMeshProUGUI concentrationType;
    private Image sliderFillImage;

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image inactiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;
    public Sprite greySlot;

    public GameObject crosshair;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        
        if (concentrationLevel != null && concentrationLevel.fillRect != null) {
            sliderFillImage = concentrationLevel.fillRect.GetComponent<Image>();
        }
    }

    private void Update()
    {
        UpdateConcentrationUI();

        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon inactiveWeapon = GetInactiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon) {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft/activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);
            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (inactiveWeapon) {
                inactiveWeaponUI.sprite = GetWeaponSprite(inactiveWeapon.thisWeaponModel);
            }
        } else {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";
        
            ammoTypeUI.sprite = emptySlot;
            activeWeaponUI.sprite = emptySlot;
            inactiveWeaponUI.sprite = emptySlot;
        }

        if (WeaponManager.Instance.lethalsCount <= 0) {
            lethalUI.sprite = greySlot;
        }

        if (WeaponManager.Instance.tacticalCount <= 0) {
            tacticalUI.sprite = greySlot;
        }
    }

    private void UpdateConcentrationUI()
    {
        if (sliderFillImage != null && concentrationType != null && concentrationLevel != null)
        {
            concentrationLevel.value = EEGManager.Instance.focus_score;
            Debug.Log("focus score: " + concentrationLevel.value);
            if (concentrationLevel.value < 0.5f) {
                    concentrationType.text = "Focused";
                    concentrationType.color = Color.red;
                    sliderFillImage.color = Color.red;
                } else {
                    concentrationType.text = "Relaxed";
                    concentrationType.color = Color.green;
                    sliderFillImage.color = Color.green;
                }
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model) {
            case Weapon.WeaponModel.M1911:
                return Resources.Load<GameObject>("M1911_Weapon").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.M4:
                return Resources.Load<GameObject>("M4_Weapon").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }
    
    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model) {
            case Weapon.WeaponModel.M1911:
                return Resources.Load<GameObject>("Pistol_Ammo").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.M4:
                return Resources.Load<GameObject>("Rifle_Ammo").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }
    
    private GameObject GetInactiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots) {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot) {
                return weaponSlot;
            }
        }
        return null;
    }

    internal void UpdateThrowablesUI()
    {
        lethalAmountUI.text = $"{WeaponManager.Instance.lethalsCount}";
        tacticalAmountUI.text = $"{WeaponManager.Instance.tacticalCount}";

        switch (WeaponManager.Instance.equippedLethalType) {
            case Throwable.ThrowableType.Grenade:
                lethalUI.sprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;
                break;
        }
        
        switch (WeaponManager.Instance.equippedTacticalType) {
            case Throwable.ThrowableType.Smoke_Grenade:
                tacticalUI.sprite = Resources.Load<GameObject>("Smoke_Grenade").GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }

}

