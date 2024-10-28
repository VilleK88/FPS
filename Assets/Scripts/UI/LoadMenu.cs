using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
public class LoadMenu : MonoBehaviour
{
    [SerializeField] public Transform content;
    [SerializeField] private SavePrefab savePrefab;
    public List<string> GetSaveFiles()
    {
        string directory = Application.persistentDataPath;
        string filePrefix = "gameInfo";
        string fileExtension = ".dat";
        string[] files = Directory.GetFiles(directory, filePrefix + "*" + fileExtension);
        Dictionary<int, string> sortedFiles = new Dictionary<int, string>();
        Regex regex = new Regex(@"gameInfo(\d+)\.dat");
        foreach (string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            Match match = regex.Match(fileName);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int saveNumber))
            {
                sortedFiles[saveNumber] = filePath;
            }
        }
        List<string> orderedSaveFiles = sortedFiles
            .OrderBy(pair => pair.Key)
            .Select(pair => pair.Value)
            .ToList();
        foreach (string file in orderedSaveFiles)
            Debug.Log($"Save file: {file}");
        return orderedSaveFiles;
    }
    public void DisplaySaveFiles()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
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
}