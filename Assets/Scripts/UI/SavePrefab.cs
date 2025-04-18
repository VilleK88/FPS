using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SavePrefab : MonoBehaviour, IPointerClickHandler
{
    public Button myButton;
    public Image saveImage;
    public TextMeshProUGUI saveName;
    public TextMeshProUGUI timeDate;
    public GameObject removeBG;
    public GameObject saveOverBG;
    public Button[] removeButtons;
    public Button[] saveOverButtons;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) removeBG.SetActive(true);
        if (eventData.button == PointerEventData.InputButton.Left) saveOverBG.SetActive(true);
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
            Debug.Log("Tiedostoa ei l�ytynyt");
        if (File.Exists(screenshotPath))
            File.Delete(screenshotPath);
        else
            Debug.Log("Tiedostoa ei l�ytynyt");
        Destroy(gameObject);
    }
    public void SaveOver()
    {
        string filename = saveName.text;
        if (!filename.EndsWith(".dat"))
            filename += ".dat";
        string saveFilePath = Path.Combine(Application.persistentDataPath, filename);
        InGameMenuControls.instance.SaveGame(3, saveFilePath);
    }
    public void CloseRemoveMenu()
    {
        removeBG.SetActive(false);
    }
    public void CloseSaveOverMenu()
    {
        SaveMenu saveMenuScript = InGameMenuControls.instance.saveMenu.gameObject.GetComponent<SaveMenu>();
        saveMenuScript.closingSavePrefabMenuCountdown = 0.5f;
        saveMenuScript.savePrefabMenuOpen = false;
        saveMenuScript.savePrefabMenuButtons = null;
        saveMenuScript.SelectButton();
        Debug.Log("Close save over menu");
        saveOverBG.SetActive(false);
    }
}