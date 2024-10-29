using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SaveMenu : MonoBehaviour
{
    [SerializeField] public Transform content;
    [SerializeField] private SavePrefab savePrefab;
    [SerializeField] private NewSave newSave;
    [SerializeField] private TMP_InputField inputField;
    public bool inputFieldOn;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            CheckInputField();
    }
    public List<string> GetSaveFiles()
    {
        string directory = Application.persistentDataPath;
        string filePrefix = "gameInfo";
        string fileExtension = ".dat";
        string[] files = Directory.GetFiles(directory, filePrefix + "*" + fileExtension);
        Dictionary<int, string> sortedFiles = new Dictionary<int, string>();
        Regex regex = new Regex(@"gameInfo(\d+)\.dat");
        foreach(string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            Match match = regex.Match(fileName);
            if(match.Success && int.TryParse(match.Groups[1].Value, out int saveNumber))
            {
                sortedFiles[saveNumber] = filePath;
            }
        }
        List<string> orderedSaveFiles = sortedFiles
            .OrderBy(pair => pair.Key)
            .Select(pair => pair.Value)
            .ToList();
        foreach(string file in orderedSaveFiles)
            Debug.Log($"Save file: {file}");
        return orderedSaveFiles;
    }
    public void DisplaySaveFiles()
    {
        foreach (Transform child in content)
        {
            NewSave dontDestroySave = child.GetComponent<NewSave>();
            if (dontDestroySave == null) Destroy(child.gameObject);
        }
        List<string> saveFiles = GetSaveFiles();
        foreach (string saveFilePath in saveFiles)
            CreateSavePrefab(saveFilePath);
    }
    void CreateSavePrefab(string saveFilePath)
    {
        string json = File.ReadAllText(saveFilePath);
        GameData gameData = JsonUtility.FromJson<GameData>(json);
        SavePrefab saveObject = Instantiate(savePrefab, content);
        savePrefab.saveName.text = $"Save {Path.GetFileName(saveFilePath)}";
        savePrefab.timeDate.text = gameData.timestamp;
        savePrefab.gameData = gameData;
    }
    public void InitializeInputField()
    {
        inputFieldOn = true;
        inputField.text = "";
        inputField.Select();
    }
    public int GetSaveCount()
    {
        return GetSaveFiles().Count;
    }
    public void DebugLogSaveFiles()
    {
        int saveCount = GetSaveCount();
        List<string> saveFiles = GetSaveFiles();

        Debug.Log($"Total saves: {saveCount}");
        foreach (string file in saveFiles)
        {
            Debug.Log($"Save file path: {file}");
        }
    }
    public void CheckInputField()
    {
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
            inputFieldOn = false;
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