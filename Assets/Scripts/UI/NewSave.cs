using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NewSave : MonoBehaviour
{
    public GameObject inputMenu;
    public void SaveGame()
    {
        InGameMenuControls.instance.SaveGame();
    }
}