using TMPro;
using UnityEngine;
public class AmmoManager : MonoBehaviour
{
    #region Singleton
    public static AmmoManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion
    [Header("UI")]
    public TextMeshProUGUI ammoDisplay;
}