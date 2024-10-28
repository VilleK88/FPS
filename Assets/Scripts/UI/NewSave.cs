using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSave : MonoBehaviour
{
    public void SaveGame()
    {
        InGameMenuControls.instance.SaveGame();
    }
}