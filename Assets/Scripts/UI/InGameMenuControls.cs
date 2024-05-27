using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
public class InGameMenuControls : MonoBehaviour
{
    #region Singleton
    public static InGameMenuControls instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    #endregion
    [SerializeField] public GameObject menuButtons;
    public UnityEvent OnToggleMenu;
    public delegate void ToggleMenuDelegate(bool isActive);
    public static event ToggleMenuDelegate OnToggleMenuStatic;
    Scene currentScene;
    public Player player;
    [SerializeField] Button saveButton;
    public GameObject settingsMenu;
    public bool settingsMenuOpen;
    private void Start()
    {
        if (settingsMenu != null)
            settingsMenu.SetActive(false);
        if (menuButtons != null)
            menuButtons.SetActive(false);
        player.GetComponent<Player>();
        currentScene = SceneManager.GetActiveScene();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !settingsMenuOpen && InventoryManager.instance.tempInventoryItem == null)
        {
            if (!InventoryManager.instance.closed)
                InventoryManager.instance.CloseInventory();
            OnToggleMenu?.Invoke();
            OnToggleMenuStatic?.Invoke(menuButtons.activeSelf);
            ToggleInGameMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && settingsMenuOpen)
            CloseSettings();
        else if (Input.GetKeyDown(KeyCode.Escape) && InventoryManager.instance.tempInventoryItem != null)
            InventoryManager.instance.CancelKeyboardItemTransfer();
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame();
        if (Input.GetKeyDown(KeyCode.F8))
            LoadGame();
    }
    void ToggleInGameMenu()
    {
        if(menuButtons != null)
        {
            menuButtons.SetActive(!menuButtons.activeSelf);
            if (menuButtons.activeSelf)
                saveButton.Select();
        }
    }
    public void SaveGame()
    {
        SaveSceneID();
        player.GetComponent<Player>().SavePlayerTransformPosition();
        InventoryManager.instance.SaveInventory();
        InventoryManager.instance.SaveHowManyBulletsLeftInMagazine();
        GameManager.instance.Save();
    }
    public void LoadGame()
    {
        ClearAndDestroyInventory();
        GameManager.instance.Load();
        LoadSceneID();
    }
    public void SaveSceneID()
    {
        if(currentScene.name == "TestScene")
            GameManager.instance.savedSceneID = 0;
        if (currentScene.name == "TestScene2")
            GameManager.instance.savedSceneID = 1;
    }
    public void LoadSceneID()
    {
        if(GameManager.instance.savedSceneID == 0)
            SceneManager.LoadScene("TestScene");
        if (GameManager.instance.savedSceneID == 1)
            SceneManager.LoadScene("TestScene2");
    }
    public void Settings()
    {
        settingsMenu.SetActive(true);
        settingsMenuOpen = true;
    }
    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        settingsMenuOpen = false;
    }
    public void QuitGame()
    {
        SceneManager.LoadScene("1 - Menu");
    }
    public void ClearAndDestroyInventory()
    {
        InventoryManager.instance.ClearInventory();
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if (inventoryCanvas != null)
            Destroy(inventoryCanvas);
    }
}