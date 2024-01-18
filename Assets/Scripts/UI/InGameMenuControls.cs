using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class InGameMenuControls : MonoBehaviour
{
    [SerializeField] GameObject menuButtons;
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
            UpdateCursorLockState();
            UpdateMouseLook();
        }
    }

    void ToggleInGameMenu()
    {
        if(menuButtons != null)
        {
            menuButtons.SetActive(!menuButtons.activeSelf);
        }
    }

    void UpdateCursorLockState()
    {
        //player.GetComponentInChildren<MouseLook>();
        Cursor.lockState = menuButtons.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void UpdateMouseLook()
    {
        if(menuButtons.activeSelf)
        {
            player.GetComponentInChildren<MouseLook>().enabled = false;
            Cursor.visible = true;
        }
        else
        {
            player.GetComponentInChildren<MouseLook>().enabled = true;
            Cursor.visible = false;
        }
    }

    public void SaveGame()
    {
        SaveSceneID();
        player.GetComponent<Player>().SavePlayerTransformPosition();
        //PlayerManager.instance.SavePlayerTransformPosition();
        GameManager.instance.Save();
    }

    public void LoadGame()
    {
        GameManager.instance.Load();
        LoadSceneID();
        //GameManager.instance.loadPlayerPosition = true;
        //PlayerManager.instance.LoadPlayerTransformPosition();
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
