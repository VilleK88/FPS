using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices.ComTypes;
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

    public float health;
    public float stamina;
    public float armor;

    public int currentLevel;
    public int skillPoints;

    public int cash;
    public int[] cashIDs;

    public float x;
    public float y;
    public float z;

    public int savedSceneID;
    public bool loadPlayerPosition = false;
    public bool loadInventory = false;

    public void Save()
    {
        Debug.Log("Game Saved!");

        BinaryFormatter bf = new BinaryFormatter(); // Tehd‰‰n uusi olio tai instanssi luokasta BinaryFormatter
        FileStream file = File.Create(Application.persistentDataPath + "/gameInfo.dat");
        GameData data = new GameData();

        data.health = health;
        data.stamina = stamina;
        data.armor = armor;

        data.currentLevel = currentLevel;
        data.skillPoints = skillPoints;

        data.cash = cash;
        data.cashIDs = cashIDs;

        data.x = x;
        data.y = y;
        data.z = z;

        data.savedSceneID = savedSceneID;
        data.loadPlayerPosition = false;

        InventoryManager.instance.inventory.Save();
        InventoryManager.instance.equipment.Save();

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
            //Debug.Log("Game Loaded!");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);
            // Deserialisoidaan ja muunnetaan data GameData -muotoon. Me tied‰mme, ett‰ data on GameData objektin informaatio.
            GameData data = (GameData)bf.Deserialize(file);
            // T‰rke‰. Muista sulkea tiedosto, ettei hakkerit p‰‰se k‰siksi.
            file.Close();

            // Kun tieto on ladattu data objektiin, siirret‰‰n muuttujien arvot Game Manager:in muuttujiin.

            health = data.health;
            stamina = data.stamina;
            armor = data.armor;

            currentLevel = data.currentLevel;
            skillPoints = data.skillPoints;

            cash = data.cash;
            cashIDs = data.cashIDs;

            x = data.x;
            y = data.y;
            z = data.z;

            savedSceneID = data.savedSceneID;
            loadPlayerPosition = true;
            loadInventory = true;
        }
    }
}

// Toinen luokka, joka voidaan serialisoida. Pit‰‰ sis‰ll‰‰n vaan sen datan mit‰ halutaan serialisoida ja tallentaa.

[Serializable]
class GameData
{
    public float health;
    public float stamina;
    public float armor;

    public int currentLevel;
    public int skillPoints;

    public int cash;
    public int[] cashIDs;

    public float x;
    public float y;
    public float z;

    public int savedSceneID;
    public bool loadPlayerPosition = false;
}
