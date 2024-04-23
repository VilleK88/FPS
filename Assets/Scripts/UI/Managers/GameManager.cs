using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
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
    public int savedSceneID;
    public bool loadPlayerPosition = false;
    public bool loadInventory = false;
    public bool changeScene = false;
    [Header("Save/load players inventory and check picked up items")]
    public InventorySlotData[] inventorySlotsData;
    public InventorySlotData[] equipmentSlotsData;
    public int[] bulletsLeft;
    public int cash;
    public int[] cashIDs;
    public int[] itemPickUpIDs;
    public void Save()
    {
        Debug.Log("Game Saved!");
        BinaryFormatter bf = new BinaryFormatter(); // Tehd‰‰n uusi olio tai instanssi luokasta BinaryFormatter
        FileStream file = File.Create(Application.persistentDataPath + "/gameInfo.dat");
        GameData data = new GameData();
        data.health = health;
        data.currentHealth = currentHealth;
        data.maxHealth = maxHealth;
        data.stamina = stamina;
        data.armor = armor;
        data.x = x;
        data.y = y;
        data.z = z;
        data.savedSceneID = savedSceneID;
        data.loadPlayerPosition = false;
        data.inventorySlotsData = inventorySlotsData;
        data.equipmentSlotsData = equipmentSlotsData;
        data.bulletsLeft = bulletsLeft;
        data.cash = cash;
        data.cashIDs = cashIDs;
        data.itemPickUpIDs = itemPickUpIDs;
        // Serialisoidaan GameData objekti, joka tallennetaan samalla tiedostoon.
        bf.Serialize(file, data);
        file.Close(); // Suljetaan tieodosto, ettei kukaan hakkeri p‰‰se siihen k‰siksi.
    }

    public void Load()
    {
        // Muista aina kun lataat tiedostoa mist‰ tahansa, tarkista ett‰ se on edes olemassa.
        // Jos on, niin sitten vasta jatka prosessia.
        if(File.Exists(Application.persistentDataPath + "/gameInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);
            // Deserialisoidaan ja muunnetaan data GameData -muotoon. Me tied‰mme, ett‰ data on GameData objektin informaatio.
            GameData data = (GameData)bf.Deserialize(file);
            // T‰rke‰. Muista sulkea tiedosto, ettei hakkerit p‰‰se k‰siksi.
            file.Close();
            // Kun tieto on ladattu data objektiin, siirret‰‰n muuttujien arvot Game Manager:in muuttujiin.
            health = data.health;
            currentHealth = data.currentHealth;
            maxHealth = data.maxHealth;
            stamina = data.stamina;
            armor = data.armor;
            x = data.x;
            y = data.y;
            z = data.z;
            loadPlayerPosition = true;
            savedSceneID = data.savedSceneID;
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
// Toinen luokka, joka voidaan serialisoida. Pit‰‰ sis‰ll‰‰n vaan sen datan mit‰ halutaan serialisoida ja tallentaa.
[Serializable]
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
    public int savedSceneID;
    public bool loadPlayerPosition = false;
    public InventorySlotData[] inventorySlotsData;
    public InventorySlotData[] equipmentSlotsData;
    public int[] bulletsLeft;
    public int cash;
    public int[] cashIDs;
    public int[] itemPickUpIDs;
}