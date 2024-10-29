using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoadPrefab : MonoBehaviour
{
    public Image saveImage;
    public TextMeshProUGUI saveName;
    public TextMeshProUGUI timeDate;
    public string filePath;
    public GameData gameData;
    public void LoadGame()
    {
        InGameMenuControls.instance.LoadGame(filePath);
        Debug.Log(filePath);
    }
}