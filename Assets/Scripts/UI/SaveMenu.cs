using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class SaveMenu : MonoBehaviour
{
    [SerializeField] public Transform content;
    [SerializeField] private SavePrefab savePrefab;
    [SerializeField] private NewSave newSave;
    [SerializeField] private TMP_InputField inputField;
    public bool inputFieldOn;
    public bool savePrefabMenuOpen;
    public Button[] savePrefabMenuButtons;
    public List<GameObject> saveObjects = new List<GameObject>();
    public int saveObjectsIndex = 0;
    [SerializeField] private Scrollbar scrollbar;
    public float closingSavePrefabMenuCountdown;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && saveObjectsIndex == 0)
            CheckInputField();
        SaveMenuNavigation();
        if (closingSavePrefabMenuCountdown > 0) closingSavePrefabMenuCountdown -= Time.unscaledDeltaTime;
    }
    void SaveMenuNavigation()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
        {
            if (!inputFieldOn && !savePrefabMenuOpen)
                if (closingSavePrefabMenuCountdown <= 0) OpenSaveOverMenu();
        }
        if (Input.GetKeyDown(KeyCode.W))
            if (!inputFieldOn && !savePrefabMenuOpen) NavigateUp();
        if (Input.GetKeyDown(KeyCode.S))
            if (!inputFieldOn && !savePrefabMenuOpen) NavigateDown();
        if (Input.GetKeyDown(KeyCode.D))
            if (!inputFieldOn && savePrefabMenuOpen) NavigateRight();
        if (Input.GetKeyDown(KeyCode.A))
            if (!inputFieldOn && savePrefabMenuOpen) NavigateLeft();
    }
    void PressButton()
    {

    }
    void NavigateRight()
    {
        savePrefabMenuButtons[1].Select();
    }
    void NavigateLeft()
    {
        savePrefabMenuButtons[0].Select();
    }
    void NavigateUp() // keyboard navigation
    {
        if (saveObjectsIndex - 1 >= 0)
        {
            saveObjectsIndex -= 1;
            SelectButton();
            ScrollToSelected();
        }
        else SelectButton();
    }
    void NavigateDown() // keyboard navigation
    {
        if (saveObjectsIndex + 1 < saveObjects.Count)
        {
            saveObjectsIndex += 1;
            SelectButton();
            ScrollToSelected();
        }
        else SelectButton();
    }
    public void SelectButton() // keyboard navigation
    {
        saveObjects[saveObjectsIndex].GetComponent<Button>().Select();
    }
    public void OpenSaveOverMenu()
    {
        savePrefabMenuOpen = true;
        SavePrefab savePrefabScript = saveObjects[saveObjectsIndex].GetComponent<SavePrefab>();
        savePrefabMenuButtons = savePrefabScript.saveOverButtons;
        savePrefabScript.saveOverBG.SetActive(true);
        savePrefabMenuButtons[0].Select();
        Debug.Log("Open SaveOver Menu");
    }
    public void CloseSaveOverMenu()
    {
        savePrefabMenuOpen = false;
    }
    public void OpenRemoveSaveMenu()
    {

    }
    public void ScrollToSelected()
    {
        float scrollbarPosition = 1 - (float)saveObjectsIndex / (saveObjects.Count - 1);
        scrollbar.value = Mathf.Clamp01(scrollbarPosition);
    }
    public List<string> GetSaveFiles()
    {
        string directory = Application.persistentDataPath;
        string[] files = Directory.GetFiles(directory, "*.dat");
        List<string> sortedDatFiles = files
            .Select(filePath => new FileInfo(filePath))
            .OrderByDescending(fileInfo => fileInfo.LastWriteTime)
            .Select(fileInfo => fileInfo.FullName)
            .ToList();
        return sortedDatFiles;
    }
    public void DisplaySaveFiles()
    {
        saveObjects.Clear();
        saveObjects.Add(newSave.gameObject);
        foreach (Transform child in content)
        {
            NewSave dontDestroySave = child.GetComponent<NewSave>();
            if (dontDestroySave == null) Destroy(child.gameObject);
        }
        List<string> saveFiles = GetSaveFiles();
        foreach (string saveFilePath in saveFiles)
            CreateSavePrefab(saveFilePath);
        saveObjects[0].GetComponent<Button>().Select();
    }
    void CreateSavePrefab(string saveFilePath)
    {
        if (!File.Exists(saveFilePath) || new FileInfo(saveFilePath).Length == 0)
        {
            Debug.LogWarning("Tallennustiedosto no tyhjä tai sitä ei löydy: " + saveFilePath);
            return;
        }
        string json = File.ReadAllText(saveFilePath);
        GameData gameData = JsonUtility.FromJson<GameData>(json);
        SavePrefab saveObject = Instantiate(savePrefab, content);
        saveObject.saveName.text = $"{Path.GetFileNameWithoutExtension(saveFilePath)}";
        saveObject.timeDate.text = gameData.timestamp;
        string imagePath = Path.Combine(Path.GetDirectoryName(saveFilePath), Path.GetFileNameWithoutExtension(saveFilePath) + ".png");
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                saveObject.saveImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                saveObject.saveImage.preserveAspect = true;
            }
            saveObjects.Add(saveObject.gameObject);
        }
    }
    public void InitializeInputField()
    {
        inputFieldOn = true;
        inputField.text = "";
        inputField.Select();
    }
    public void CheckInputField()
    {
        if (saveObjectsIndex != 0) return;
        string fileName = inputField.text;
        if(string.IsNullOrEmpty(fileName))
        {
            Debug.Log("File name is empty. Name the file so it can be saved.");
            return;
        }
        if(hasInvalidFileNameChars(fileName))
        {
            Debug.Log("Invalid characters used in the file name.");
            return;
        }
        if (!fileName.EndsWith(".dat"))
            fileName += ".dat";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if(File.Exists(filePath))
        {
            Debug.Log("File name " + fileName + " has already been taken.");
        }
        else
        {
            Debug.Log("File name is free to be saved.");
            InGameMenuControls.instance.SaveGame(2, filePath);
            InGameMenuControls.instance.CloseNewSaveInputMenu();
        }
    }
    bool hasInvalidFileNameChars(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach(char c in fileName)
        {
            if (invalidChars.Contains(c))
                return true;
        }
        return false;
    }
}