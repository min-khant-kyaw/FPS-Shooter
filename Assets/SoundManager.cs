using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get; set;}

    public AudioSource shootingChannel;
    public AudioClip m1911Shot;
    public AudioClip m4Shot;


    // TODO: change to reloading Channel
    public AudioSource reloadingSoundM4;
    public AudioSource reloadingSoundM1911;

    public AudioSource emptyMagazineSoundM1911;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch(weapon) {
            case WeaponModel.M1911:
                shootingChannel.PlayOneShot(m1911Shot);
                break;
            case WeaponModel.M4:
                shootingChannel.PlayOneShot(m4Shot);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch(weapon) {
            case WeaponModel.M1911:
                reloadingSoundM1911.Play();
                break;
            case WeaponModel.M4:
                reloadingSoundM4.Play();
                break;
        }
    }

}
