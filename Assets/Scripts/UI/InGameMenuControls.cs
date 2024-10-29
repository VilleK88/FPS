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
    public GameObject saveMenu;
    public bool saveMenuOpen;
    public GameObject loadMenu;
    public bool loadMenuOpen;
    public GameObject newSaveInputMenu;
    public bool newSaveInputMenuOpen;
    public Camera mainCamera;
    public Camera weaponRenderCamera;
    private void Start()
    {
        if (settingsMenu != null)
            settingsMenu.SetActive(false);
        if (menuButtons != null)
            menuButtons.SetActive(false);
        player.GetComponent<Player>();
        currentScene = SceneManager.GetActiveScene();
        if (AccountManager.Instance != null)
            AccountManager.Instance.HideContainer();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!settingsMenuOpen && !saveMenuOpen && !loadMenuOpen && InventoryManager.instance.tempInventoryItem == null)
            {
                if (!InventoryManager.instance.closed)
                    InventoryManager.instance.CloseInventory(true);
                OnToggleMenu?.Invoke();
                OnToggleMenuStatic?.Invoke(menuButtons.activeSelf);
                ToggleInGameMenu();
            }
            else if (settingsMenuOpen && !saveMenuOpen && !loadMenuOpen && !newSaveInputMenuOpen)
                CloseSettings();
            else if (!settingsMenuOpen && saveMenuOpen && !loadMenuOpen && !newSaveInputMenuOpen)
                CloseSaveMenu();
            else if (!settingsMenuOpen && !saveMenuOpen && loadMenuOpen && !newSaveInputMenuOpen)
                CloseLoadMenu();
            else if (!settingsMenuOpen && saveMenuOpen && !loadMenuOpen && newSaveInputMenuOpen)
                CloseNewSaveInputMenu();
            else if (InventoryManager.instance.tempInventoryItem != null)
                InventoryManager.instance.CancelKeyboardItemTransfer();
        }
        if (Input.GetKeyDown(KeyCode.F5))
            QuickSave();
        if (Input.GetKeyDown(KeyCode.F8))
            LoadGame();
        if (Input.GetKeyDown(KeyCode.F9))
        {
            saveMenu.GetComponent<SaveMenu>().GetSaveFiles();
            //saveMenu.GetComponent<SaveMenu>().DebugLogSaveFiles();
        }
    }
    void ToggleInGameMenu()
    {
        if (menuButtons != null)
        {
            InventoryManager.instance.HolsterWeapons();
            menuButtons.SetActive(!menuButtons.activeSelf);
            if (menuButtons.activeSelf)
            {
                saveButton.Select();
                InventoryManager.instance.PauseGame();
            }
            else
            {
                InventoryManager.instance.DrawActiveWeapon();
                InventoryManager.instance.StopPause();
            }
        }
    }
    public void SaveGame()
    {
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
        {
            SaveSceneID();
            player.GetComponent<Player>().SavePlayerTransformPosition();
            InventoryManager.instance.SaveInventory();
            InventoryManager.instance.SaveHowManyBulletsLeftInMagazine();
            EnemyManager.Instance.SaveEnemiesData();
            GameManager.instance.Save(false, "yes"); // muista lisätä filename
        }
    }
    public void OpenSaveMenu()
    {
        saveMenu.SetActive(true);
        Debug.Log("Open save menu");
        SaveMenu saveMenuScript = saveMenu.GetComponent<SaveMenu>();
        saveMenuScript.GetSaveFiles();
        saveMenuScript.DisplaySaveFiles();
        saveMenuOpen = true;
    }
    public void OpenLoadMenu()
    {
        loadMenu.SetActive(true);
        Debug.Log("Open load menu");
        LoadMenu loadMenuScript = loadMenu.GetComponent<LoadMenu>();
        loadMenuScript.GetSaveFiles();
        loadMenuScript.DisplaySaveFiles();
        loadMenuOpen = true;
    }
    public void QuickSave()
    {
        if (!EnemyManager.Instance.CanAnyoneSeeThePlayer())
        {
            SaveSceneID();
            player.GetComponent<Player>().SavePlayerTransformPosition();
            InventoryManager.instance.SaveInventory();
            InventoryManager.instance.SaveHowManyBulletsLeftInMagazine();
            EnemyManager.Instance.SaveEnemiesData();
            GameManager.instance.Save(true, "no");
        }
    }
    public void LoadGame()
    {
        ClearAndDestroyInventory();
        GameManager.instance.Load(true);
        LoadSceneID();
        Time.timeScale = 1f;
    }
    public void SaveSceneID()
    {
        if (currentScene.name == "2 - Prison")
            GameManager.instance.savedSceneID = 0;
        if (currentScene.name == "TestScene2")
            GameManager.instance.savedSceneID = 1;
    }
    public void LoadSceneID()
    {
        if (GameManager.instance.savedSceneID == 0)
            SceneManager.LoadScene("2 - Prison");
        if (GameManager.instance.savedSceneID == 1)
            SceneManager.LoadScene("TestScene2");
    }
    public void OpenNewSaveInputMenu()
    {
        newSaveInputMenu.SetActive(true);
        newSaveInputMenuOpen = true;
        saveMenu.GetComponent<SaveMenu>().InitializeInputField();
    }
    public void CloseNewSaveInputMenu()
    {
        newSaveInputMenu.SetActive(false);
        newSaveInputMenuOpen = false;
        saveMenu.GetComponent<SaveMenu>().inputFieldOn = false;
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
    public void CloseSaveMenu()
    {
        saveMenu.SetActive(false);
        saveMenuOpen = false;
    }
    public void CloseLoadMenu()
    {
        loadMenu.SetActive(false);
        loadMenuOpen = false;
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