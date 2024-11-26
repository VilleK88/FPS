using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GodModeUI : MonoBehaviour
{
    #region Singleton
    public static GodModeUI Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    #endregion
    public TextMeshProUGUI godModeText;
    public void GodModeOn()
    {
        godModeText.enabled = true;
    }
    public void GodModeOff()
    {
        godModeText.enabled = false;
    }
}