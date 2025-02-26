using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    string newGameScene = "MainScene";
    public AudioSource mainChannel;
    public AudioClip bgMusic;

    private void Start() 
    {
        mainChannel.PlayOneShot(bgMusic);

        // Set High Score text
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";
    }

    public void StartNewGame()
    {
        mainChannel.Stop();

        SceneManager.LoadScene(newGameScene);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

}
