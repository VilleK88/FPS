using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    private void Start()
    {
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if(inventoryCanvas != null)
        {
            Destroy(inventoryCanvas);
        }
    }

    public void StartGame()
    {
        GameManager.instance.cashIDs = new int[0]; // clear collected currency IDs
        GameManager.instance.loadPlayerPosition = false;
        //GameManager.instance.loadInventory = false;
        SceneManager.LoadScene("TestScene");
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
