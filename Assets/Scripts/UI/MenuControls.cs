using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    public GameObject SettingsMenu;

    private void Start()
    {
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if(inventoryCanvas != null)
            Destroy(inventoryCanvas);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SettingsMenu.SetActive(false);
    }

    public void StartGame()
    {
        ClearTablesInGameManager();
        GameManager.instance.loadPlayerPosition = false;
        //GameManager.instance.loadInventory = false;
        SceneManager.LoadScene("TestScene");
    }

    public void ClearTablesInGameManager()
    {
        GameManager.instance.cashIDs = new string[0]; // clear collected currency IDs
        GameManager.instance.itemPickUpIDs = new string[0]; // clear colleted item IDs
        GameManager.instance.bulletsLeft = new int[0]; // clear saved bullets left in magazines
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
            SceneManager.LoadScene("TestScene");
        if (GameManager.instance.savedSceneID == 1)
            SceneManager.LoadScene("TestScene2");
    }
    public void Settings()
    {
        SettingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}