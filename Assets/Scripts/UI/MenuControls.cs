using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
    #region Singleton
    public static MenuControls Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    #endregion
    public GameObject SettingsMenu;
    public GameObject loadMenu;
    public bool loadMenuOpen = false;
    [SerializeField] private Scrollbar loadMenuScrollbar;
    [SerializeField] Button[] uiButtons;
    private void Start()
    {
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if (inventoryCanvas != null)
            Destroy(inventoryCanvas);
        if (AccountManager.Instance != null)
            AccountManager.Instance.ShowContainer();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingsMenu.activeSelf)
            {
                SettingsMenu.SetActive(false);
                uiButtons[2].Select();
            }
            else if (loadMenu.activeSelf) CloseLoadMenu();
        }
    }
    public void StartGame()
    {
        ClearTablesInGameManager();
        GameManager.instance.loadPlayerPosition = false;
        SceneManager.LoadScene("2 - Prison");
        Time.timeScale = 1f;
    }
    public void ClearTablesInGameManager()
    {
        GameManager.instance.cashIDs = new string[0]; // clear collected currency IDs
        GameManager.instance.itemPickUpIDs = new string[0]; // clear colleted item IDs
        GameManager.instance.explodingBarrelIDs = new string[0]; // clear destroyed barrel IDs
        GameManager.instance.bulletsLeft = new int[0]; // clear saved bullets left in magazines
    }
    public void LoadGame(string filePath)
    {
        GameManager.instance.Load(false, filePath);
        LoadSceneID();
        Time.timeScale = 1f;
    }
    public void OpenLoadMenu()
    {
        loadMenu.SetActive(true);
        LoadMenu loadMenuScript = loadMenu.GetComponent<LoadMenu>();
        loadMenuScript.DisplaySaveFiles();
        loadMenuOpen = true;
        loadMenuScrollbar.value = 1f;
    }
    public void CloseLoadMenu()
    {
        loadMenu.SetActive(false);
        loadMenuOpen = false;
        uiButtons[1].Select();
    }
    public void LoadSceneID()
    {
        if (GameManager.instance.savedSceneID == 0) SceneManager.LoadScene("2 - Prison");
        else if (GameManager.instance.savedSceneID == 1) SceneManager.LoadScene("TestScene2");
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