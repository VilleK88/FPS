using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadPrefab : MonoBehaviour, IPointerClickHandler
{
    public Image saveImage;
    public TextMeshProUGUI saveName;
    public TextMeshProUGUI timeDate;
    public string filePath;
    public GameData gameData; // don't remove
    public GameObject removeBG;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) removeBG.SetActive(true);
    }
    public void LoadGame()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) InGameMenuControls.instance.LoadGame(filePath);
        else MenuControls.Instance.LoadGame(filePath);
    }
    public void RemoveSave()
    {
        string fileName = saveName.text;
        Debug.Log($"Savename: {fileName}");
        if (!fileName.EndsWith(".dat"))
            fileName += ".dat";
        string saveFilePath = Path.Combine(Application.persistentDataPath, fileName);
        string screenshotPath = Path.Combine(Application.persistentDataPath, fileName.Replace(".dat", ".png"));
        if (File.Exists(saveFilePath))
            File.Delete(saveFilePath);
        else
            Debug.Log("Tiedostoa ei löytynyt");
        if (File.Exists(screenshotPath))
            File.Delete(screenshotPath);
        else
            Debug.Log("Tiedostoa ei löytynyt");
        Destroy(gameObject);
    }
    public void CloseRemoveMenu()
    {
        removeBG.SetActive(false);
    }
}