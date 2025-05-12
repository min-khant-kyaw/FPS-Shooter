using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    string newGameScene = "MainScene";
    public AudioSource mainChannel;
    public AudioClip bgMusic;
    public Button startButton;
    public Button exitButton;

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainChannel.PlayOneShot(bgMusic);
        startButton.onClick.AddListener(StartNewGame);
        exitButton.onClick.AddListener(ExitApplication);
    }

    public void StartNewGame()
    {
        Debug.Log("StartNewGame called");
        if (mainChannel != null)
            mainChannel.Stop();

        SceneManager.LoadScene(newGameScene);
    }


    public void ExitApplication()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }

}
