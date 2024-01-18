using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    Scene currentScene;

    private void Start()
    {
        //currentScene = SceneManager.GetActiveScene();
    }

    public void StartGame()
    {
        GameManager.instance.cashIDs = new int[0]; // clear collected currency IDs
        SceneManager.LoadScene("TestScene");
    }

    public void LoadGame()
    {
        LoadSceneID();
        GameManager.instance.Load();
    }

    public void LoadSceneID()
    {
        if (GameManager.instance.savedSceneID == 0)
        {
            SceneManager.LoadScene("TestScene");
        }
    }

    public void Settings()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
