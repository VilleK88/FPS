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
    [HideInInspector] public GameObject player;

    private void Start()
    {
        if(menuButtons != null)
        {
            menuButtons.SetActive(false);
        }
        player = PlayerManager.instance.GetPlayer();
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
        GameManager.instance.Save();
    }

    public void LoadGame()
    {
        LoadSceneID();
        GameManager.instance.Load();
    }

    public void SaveSceneID()
    {
        if(currentScene.name == "TestScene")
        {
            GameManager.instance.savedSceneID = 0;
        }
    }

    public void LoadSceneID()
    {
        if(GameManager.instance.savedSceneID == 0)
        {
            SceneManager.LoadScene("TestScene");
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
