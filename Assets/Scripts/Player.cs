using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;

    public TextMeshProUGUI playerHealthUI;
    public GameObject gameOverUI;

    public bool isDead;

    public int healingRate = 1;
    private float healTimer = 0f;
    private float healInterval = 2f;

    private void Start()
    {
        playerHealthUI.text = $"Health: {HP}";
    }

    private void Update() {
        if (!isDead) {
            healTimer += Time.deltaTime;
            if (healTimer >= healInterval) {
                PlayerHealing();
                healTimer = 0f;
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
    
        if (HP <= 0) {
            print("Player Dead");
            PlayerDead();
            isDead = true;
        } else {
            print("Player Hit");
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health: {HP}";
        }
    }

    private void PlayerHealing()
    {
        if (HUDManager.Instance != null && HUDManager.Instance.concentrationType != null) {
            if (HUDManager.Instance.concentrationType.text == "Focused") {
                healingRate = 1;
            } else if (HUDManager.Instance.concentrationType.text == "Relaxed") {
                healingRate = 3;
            }

            HP += healingRate;
            // Cap HP at 100
            if (HP > 100) {
                HP = 100;
            }

            playerHealthUI.text = $"Health: {HP}";
        }
    }

    private void PlayerDead()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDeath);

        SoundManager.Instance.playerChannel.clip = SoundManager.Instance.gameOverMusic;
        SoundManager.Instance.playerChannel.PlayDelayed(2f);
        
        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        // GetComponent<Weapon>().enabled = false;
        
        // Dying Animation
        GetComponentInChildren<Animator>().enabled = true;
        playerHealthUI.gameObject.SetActive(false);

        GetComponent<ScreenBlackout>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.gameObject.SetActive(true);

        // int waveSurvived = GlobalReferences.Instance.waveNumber - 1;

        // if (waveSurvived > SaveLoadManager.Instance.LoadHighScore()) {
        //     SaveLoadManager.Instance.SaveHighScore(waveSurvived);
        // }

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (bloodyScreen.activeInHierarchy == false) {
            bloodyScreen.SetActive(true);
        }

        // Fade out the image
        var image = bloodyScreen.GetComponentInChildren<Image>();
 
        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;
 
        float duration = 3f;
        float elapsedTime = 0f;
 
        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
 
            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
 
            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;
 
            yield return null; ; // Wait for the next frame.
        }

        if (bloodyScreen.activeInHierarchy) {
            bloodyScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieHand")) {
            if (isDead == false) {
                TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
            }
        }
    }

}
