using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion
    public GameObject player;
    public TextMeshProUGUI sneakIndicatorText;
    public Image sneakIndicatorImage;
    public GameObject GetPlayer()
    {
        return player;
    }
}
