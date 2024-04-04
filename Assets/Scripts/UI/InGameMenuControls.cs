using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class InGameMenuControls : MonoBehaviour
{
    #region Singleton
    public static InGameMenuControls instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    [SerializeField] public GameObject menuButtons;
    public UnityEvent OnToggleMenu;
    public delegate void ToggleMenuDelegate(bool isActive);
    public static event ToggleMenuDelegate OnToggleMenuStatic;
    Scene currentScene;
    public Player player;

    private void Start()
    {
        if(menuButtons != null)
        {
            menuButtons.SetActive(false);
        }
        player.GetComponent<Player>();
        //currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnToggleMenu?.Invoke();
            OnToggleMenuStatic?.Invoke(menuButtons.activeSelf);
            ToggleInGameMenu();
        }
    }

    void ToggleInGameMenu()
    {
        if(menuButtons != null)
        {
            menuButtons.SetActive(!menuButtons.activeSelf);
        }
    }

    public void SaveGame()
    {
        SaveSceneID();
        player.GetComponent<Player>().SavePlayerTransformPosition();
        InventoryManager.instance.SaveInventory();
        GameManager.instance.Save();
    }

    public void LoadGame()
    {
        InventoryManager.instance.ClearInventory();
        GameManager.instance.Load();
        LoadSceneID();
    }

    public void SaveSceneID()
    {
        if(currentScene.name == "TestScene")
        {
            GameManager.instance.savedSceneID = 0;
        }
        if (currentScene.name == "TestScene2")
        {
            GameManager.instance.savedSceneID = 1;
        }
    }

    public void LoadSceneID()
    {
        if(GameManager.instance.savedSceneID == 0)
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
        SceneManager.LoadScene("1 - Menu");
    }
}
