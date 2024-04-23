using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    #region Singleton
    public static HealthManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion
    public Image frontHealthbar;
    public Image backHealthbar;
}
