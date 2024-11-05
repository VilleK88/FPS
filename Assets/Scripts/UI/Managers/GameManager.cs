using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    [Header("Health, Stamina, Armor")]
    public float health;
    public float currentHealth;
    public float maxHealth;
    public float stamina;
    public float armor;
    [Header("Save/load scene and players position on it")]
    public float x;
    public float y;
    public float z;
    public float xRotation;
    public float yRotation;
    public float zRotation;
    public int savedSceneID;
    public bool loadPlayerPosition = false;
    public bool loadInventory = false;
    public bool changeScene = false; // check if scene is changing. not serialized.
    public bool ifSneaking;
    [Header("Save/load players inventory and check picked up items")]
    public InventorySlotData[] inventorySlotsData;
    public InventorySlotData[] equipmentSlotsData;
    public InventoryManagerData inventoryData;
    public int[] bulletsLeft; // save/load how many bullets left in weapon
    public int cash;
    public string[] cashIDs;
    public string[] itemPickUpIDs;
    [Header("Save/load enemy parameters")]
    public List<EnemyData> nearbyEnemies = new List<EnemyData>();
    public string enemyDataID;
    public float[] enemyPositionX;
    public float[] enemyPositionY;
    public float[] enemyPositionZ;
    public float[] enemyRotationX;
    public float[] enemyRotationY;
    public float[] enemyRotationZ;
    public int[] patrolWaypointIndex;
    public bool enemyDead;
    public bool enemyFoundDead;
    public bool loadEnemiesData = false;
    [Header("Multisave System")]
    public Image saveImage;
    public string saveName;
    public string timestamp;
    public void Save(int saveType, string newFilePath)
    {
        StartCoroutine(SaveGameWithScreenshot(saveType, newFilePath));
    }
    private IEnumerator SaveGameWithScreenshot(int saveType, string newFilePath)
    {
        Debug.Log("Saving game with screenshot....");
        GameObject playerObject = PlayerManager.instance.GetPlayer();
        PlayerMovement playerMovementScript = playerObject.GetComponent<PlayerMovement>();
        Texture2D screenshotTexture = null;
        yield return StartCoroutine(TakeScreenshot(texture => screenshotTexture = texture));
        string json = JsonConvert.SerializeObject(new GameData
        {
            health = this.health,
            currentHealth = this.currentHealth,
            maxHealth = this.maxHealth,
            stamina = this.stamina,
            armor = this.armor,
            x = this.x,
            y = this.y,
            z = this.z,
            xRotation = this.xRotation,
            yRotation = this.yRotation,
            zRotation = this.zRotation,
            savedSceneID = this.savedSceneID,
            ifSneaking = playerMovementScript.sneaking,
            inventorySlotsData = this.inventorySlotsData,
            equipmentSlotsData = this.equipmentSlotsData,
            inventoryData = this.inventoryData,
            bulletsLeft = this.bulletsLeft,
            cash = this.cash,
            cashIDs = this.cashIDs,
            itemPickUpIDs = this.itemPickUpIDs,
            nearbyEnemies = this.nearbyEnemies,
            enemyDataID = this.enemyDataID,
            enemyPositionX = this.enemyPositionX,
            enemyPositionY = this.enemyPositionY,
            enemyPositionZ = this.enemyPositionZ,
            enemyRotationX = this.enemyRotationX,
            enemyRotationY = this.enemyRotationY,
            enemyRotationZ = this.enemyRotationZ,
            patrolWaypointIndex = this.patrolWaypointIndex,
            enemyDead = this.enemyDead,
            enemyFoundDead = this.enemyFoundDead,
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        }, Formatting.Indented);
        if (saveType == 1) // quicksave
        {
            File.WriteAllText(Application.persistentDataPath + "/quicksave.dat", json);
            string screenshotPath = Path.Combine(Application.persistentDataPath, "quicksave.png");
            File.WriteAllBytes(screenshotPath, screenshotTexture.EncodeToPNG());
        }
        else if(saveType == 2) // manual save
        {
            File.WriteAllText(newFilePath, json);
            string screenshotPath = Path.ChangeExtension(newFilePath, ".png");
            File.WriteAllBytes(screenshotPath, screenshotTexture.EncodeToPNG());
            SaveMenu saveMenu = InGameMenuControls.instance.saveMenu.GetComponent<SaveMenu>();
            saveMenu.DisplaySaveFiles();
        }
        else if(saveType == 3) // save over
        {
            File.WriteAllText(newFilePath, json);
            string screenshotPath = Path.ChangeExtension(newFilePath, ".png");
            File.WriteAllBytes(screenshotPath, screenshotTexture.EncodeToPNG());
            SaveMenu saveMenu = InGameMenuControls.instance.saveMenu.GetComponent<SaveMenu>();
            saveMenu.DisplaySaveFiles();
            saveMenu.saveObjectsIndex = 0;
            saveMenu.SelectButton();
            saveMenu.ScrollToSelected();
            saveMenu.savePrefabMenuOpen = false;
            saveMenu.closingSavePrefabMenuCountdown = 0;
        }
        loadEnemiesData = true;
        Debug.Log("Game and screenshot saved!");
    }
    public IEnumerator TakeScreenshot(System.Action<Texture2D> callback)
    {
        yield return new WaitForEndOfFrame();
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera mainCamera = InGameMenuControls.instance.mainCamera;
        Camera weaponCamera = InGameMenuControls.instance.weaponRenderCamera;
        mainCamera.targetTexture = renderTexture;
        weaponCamera.targetTexture = renderTexture;
        mainCamera.Render();
        weaponCamera.Render();
        RenderTexture.active = renderTexture;
        Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshotTexture.Apply();
        mainCamera.targetTexture = null;
        weaponCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        callback?.Invoke(screenshotTexture);
    }
    public void Load(bool quickLoad, string newFilePath)
    {
        loadPlayerPosition = true;
        loadEnemiesData = true;
        if (quickLoad)
        {
            string filePath = Application.persistentDataPath + "/quicksave.dat";
            if (File.Exists(filePath))
            {
                Debug.Log("Quick save loaded");
                string json = File.ReadAllText(filePath);
                GameData data = JsonConvert.DeserializeObject<GameData>(json);
                health = data.health;
                currentHealth = data.currentHealth;
                maxHealth = data.maxHealth;
                stamina = data.stamina;
                armor = data.armor;
                x = data.x;
                y = data.y;
                z = data.z;
                xRotation = data.xRotation;
                yRotation = data.yRotation;
                zRotation = data.zRotation;
                savedSceneID = data.savedSceneID;
                ifSneaking = data.ifSneaking;
                loadInventory = true;
                inventorySlotsData = data.inventorySlotsData;
                equipmentSlotsData = data.equipmentSlotsData;
                inventoryData = data.inventoryData;
                bulletsLeft = data.bulletsLeft;
                cash = data.cash;
                cashIDs = data.cashIDs;
                itemPickUpIDs = data.itemPickUpIDs;
                nearbyEnemies = data.nearbyEnemies;
                enemyDataID = data.enemyDataID;
                enemyPositionX = data.enemyPositionX;
                enemyPositionY = data.enemyPositionY;
                enemyPositionZ = data.enemyPositionZ;
                enemyRotationX = data.enemyRotationX;
                enemyRotationY = data.enemyRotationY;
                enemyRotationZ = data.enemyRotationZ;
                patrolWaypointIndex = data.patrolWaypointIndex;
                enemyDead = data.enemyDead;
                enemyFoundDead = data.enemyFoundDead;
            }
        }
        else
        {
            if (File.Exists(newFilePath))
            {
                Debug.Log("Save loaded");
                string json = File.ReadAllText(newFilePath);
                GameData data = JsonConvert.DeserializeObject<GameData>(json);
                health = data.health;
                currentHealth = data.currentHealth;
                maxHealth = data.maxHealth;
                stamina = data.stamina;
                armor = data.armor;
                x = data.x;
                y = data.y;
                z = data.z;
                xRotation = data.xRotation;
                yRotation = data.yRotation;
                zRotation = data.zRotation;
                savedSceneID = data.savedSceneID;
                ifSneaking = data.ifSneaking;
                loadInventory = true;
                inventorySlotsData = data.inventorySlotsData;
                equipmentSlotsData = data.equipmentSlotsData;
                inventoryData = data.inventoryData;
                bulletsLeft = data.bulletsLeft;
                cash = data.cash;
                cashIDs = data.cashIDs;
                itemPickUpIDs = data.itemPickUpIDs;
                nearbyEnemies = data.nearbyEnemies;
                enemyDataID = data.enemyDataID;
                enemyPositionX = data.enemyPositionX;
                enemyPositionY = data.enemyPositionY;
                enemyPositionZ = data.enemyPositionZ;
                enemyRotationX = data.enemyRotationX;
                enemyRotationY = data.enemyRotationY;
                enemyRotationZ = data.enemyRotationZ;
                patrolWaypointIndex = data.patrolWaypointIndex;
                enemyDead = data.enemyDead;
                enemyFoundDead = data.enemyFoundDead;
            }
        }
    }
}
[Serializable] // Toinen luokka, joka voidaan serialisoida. Pit‰‰ sis‰ll‰‰n vaan sen datan mit‰ halutaan serialisoida ja tallentaa.
public class GameData
{
    public float health;
    public float currentHealth;
    public float maxHealth;
    public float stamina;
    public float armor;
    public float x;
    public float y;
    public float z;
    public float xRotation;
    public float yRotation;
    public float zRotation;
    public int savedSceneID;
    public bool loadPlayerPosition = false;
    public bool ifSneaking;
    public InventorySlotData[] inventorySlotsData;
    public InventorySlotData[] equipmentSlotsData;
    public InventoryManagerData inventoryData;
    public int[] bulletsLeft;
    public int cash;
    public string[] cashIDs;
    public string[] itemPickUpIDs;
    public List<EnemyData> nearbyEnemies = new List<EnemyData>();
    public string enemyDataID;
    public float[] enemyPositionX;
    public float[] enemyPositionY;
    public float[] enemyPositionZ;
    public float[] enemyRotationX;
    public float[] enemyRotationY;
    public float[] enemyRotationZ;
    public int[] patrolWaypointIndex;
    public bool enemyDead;
    public bool enemyFoundDead;
    public Image saveImage;
    public string timestamp;
}