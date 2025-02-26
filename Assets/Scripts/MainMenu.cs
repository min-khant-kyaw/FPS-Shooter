using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource main_channel;
    public AudioClip bg_Music;
    public TMP_Text highScoreUI;
    string newGameScene = "MainScene";


    void Start() 
    {
        main_channel.PlayOneShot(bg_Music);

        // Set High Score text
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";
    }

    public void StartNewGame()
    {
        main_channel.Stop();

        SceneManager.LoadScene(newGameScene);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

}
