using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    Scene currentScene;
    //public InventoryObject inventory;
    //public InventoryObject equipment;

    private void Start()
    {
        //currentScene = SceneManager.GetActiveScene();
    }

    public void StartGame()
    {
        GameManager.instance.cashIDs = new int[0]; // clear collected currency IDs
        GameManager.instance.loadPlayerPosition = false;
        SceneManager.LoadScene("TestScene");

        //inventory.Container.Clear();
        //equipment.Container.Clear();
    }

    public void LoadGame()
    {
        GameManager.instance.loadPlayerPosition = true;
        GameManager.instance.Load();
        LoadSceneID();
    }

    public void LoadSceneID()
    {
        if (GameManager.instance.savedSceneID == 0)
        {
            SceneManager.LoadScene("TestScene");
        }
        if (GameManager.instance.savedSceneID == 1)
        {
            SceneManager.LoadScene("TestScene2");
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
