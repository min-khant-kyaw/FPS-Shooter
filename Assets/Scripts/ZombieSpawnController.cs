using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiePerWave = 5;
    public int currentZombiePerWave;
    public int increaseRatePerWave = 5;

    public float spawnDelay = 0.5f; // Delay between each Zombie spawn

    public int currentWave = 0;
    public int finalWave = 2;

    public float waveCooldown = 10.0f; // Time in seconds between Zombie Waves
    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI cooldownCounterUI;
    public TextMeshProUGUI currentWaveUI;

    public GameObject gameCompleteUI;
    
    public Transform[] spawnPoints; // Array of spawn points

    private void Start() {
        currentZombiePerWave = initialZombiePerWave;
        GlobalReferences.Instance.waveNumber = currentWave;
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();
        currentWave++;

        GlobalReferences.Instance.waveNumber = currentWave;

        currentWaveUI.text = "Wave: " + currentWave.ToString();
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiePerWave; i++) {
            
            // Select a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Generate a random offset for zombie spawn location
            Vector3 spawnOffset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f,2f));
            Vector3 spawnPosition = spawnPoint.position + spawnOffset;

            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            Enemy enemyScript = zombie.GetComponent<Enemy>();

            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update() {
        // Check if all zombies are dead
        bool allZombiesDead = true;
        foreach (Enemy zombie in currentZombiesAlive) {
            if (!zombie.isDead) {
                allZombiesDead = false;
                break; // No need to check further
            }
        }

        // If all zombies are dead and we're not in cooldown, start cooldown
        if (allZombiesDead && !inCooldown) {
            if (currentWave == finalWave) {
                Debug.Log("Trigger Game Completed");
                GameCompleted();
            } else {
                // Start cooldown for waves 1-9
                StartCoroutine(WaveCooldown());
            }
        }

        // Run the cooldown counter
        if (inCooldown) {
            cooldownCounter -= Time.deltaTime;
        } else {
            cooldownCounter = waveCooldown;
        }

        cooldownCounterUI.text = cooldownCounter.ToString("F0");
    }


    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        waveOverUI.gameObject.SetActive(true);

        // Stop all zombie-related sounds before destroying
        SoundManager.Instance.zombieChannel.Stop();
        SoundManager.Instance.zombieChannel2.Stop();

        // Destroy all zombies
        foreach (Enemy zombie in currentZombiesAlive) {
            Destroy(zombie.gameObject);
        }

        currentZombiesAlive.Clear(); // Clear the list after destroying

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;
        waveOverUI.gameObject.SetActive(false);

        currentZombiePerWave += increaseRatePerWave;
        StartNextWave();
    }

    private void GameCompleted()
    {
        SoundManager.Instance.playerChannel.clip = SoundManager.Instance.gameOverMusic;
        SoundManager.Instance.playerChannel.PlayDelayed(2f);
        // Stop all zombie-related sounds
        SoundManager.Instance.zombieChannel.Stop();
        SoundManager.Instance.zombieChannel2.Stop();

        // Destroy all zombies
        foreach (Enemy zombie in currentZombiesAlive) {
            Destroy(zombie.gameObject);
        }
        currentZombiesAlive.Clear();
        
        GetComponent<ScreenBlackout>().StartFade();
        StartCoroutine(GameCompletedUI());
    }

    private IEnumerator GameCompletedUI()
    {
        yield return new WaitForSeconds(2f);
        gameCompleteUI.gameObject.SetActive(true);

        StartCoroutine(ReturnToMainMenu());
    }
    
    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }

}
