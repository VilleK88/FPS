using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
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
    [Header("Save/load players inventory and check picked up items")]
    public InventorySlotData[] inventorySlotsData;
    public InventorySlotData[] equipmentSlotsData;
    public int[] bulletsLeft; // save/load how many bullets left in weapon
    public int cash;
    public int[] cashIDs;
    public int[] itemPickUpIDs;
    public void Save()
    {
        Debug.Log("Game Saved!");
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
            inventorySlotsData = this.inventorySlotsData,
            equipmentSlotsData = this.equipmentSlotsData,
            bulletsLeft = this.bulletsLeft,
            cash = this.cash,
            cashIDs = this.cashIDs,
            itemPickUpIDs = this.itemPickUpIDs
        }, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/gameInfo.dat", json);
    }

    public void Load()
    {
        string filePath = Application.persistentDataPath + "/gameInfo.dat";
        if (File.Exists(filePath))
        {
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
            loadPlayerPosition = true;
            loadInventory = true;
            inventorySlotsData = data.inventorySlotsData;
            equipmentSlotsData = data.equipmentSlotsData;
            bulletsLeft = data.bulletsLeft;
            cash = data.cash;
            cashIDs = data.cashIDs;
            itemPickUpIDs = data.itemPickUpIDs;
        }
    }
}
[Serializable] // Toinen luokka, joka voidaan serialisoida. Pit‰‰ sis‰ll‰‰n vaan sen datan mit‰ halutaan serialisoida ja tallentaa.
class GameData
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
    public InventorySlotData[] inventorySlotsData;
    public InventorySlotData[] equipmentSlotsData;
    public int[] bulletsLeft;
    public int cash;
    public int[] cashIDs;
    public int[] itemPickUpIDs;
}