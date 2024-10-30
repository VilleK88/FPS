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
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Oikeaa hiiren näppäintä painettu");
            removeBG.SetActive(true);
        }
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Vasenta hiiren näppäintä painettu");
            saveOverBG.SetActive(true);
        }
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
        saveOverBG.SetActive(false);
    }
}