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
        if (instance != null && instance != this)
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
    public GameObject interactableIconObject;
    private Image interactableIconImg;
    [SerializeField] private TextMeshProUGUI interactableText;
    public Image middlePoint;
    private void Start()
    {
        interactableIconImg = interactableIconObject.GetComponent<Image>();
        interactableText = interactableIconObject.GetComponentInChildren<TextMeshProUGUI>();
    }
    public GameObject GetPlayer()
    {
        return player;
    }
    public void TurnOnOrOffInteractable(bool onOrOff)
    {
        if(onOrOff)
        {
            interactableIconImg.enabled = true;
            interactableText.enabled = true;
        }
        else
        {
            interactableIconImg.enabled = false;
            interactableText.enabled = false;
        }
    }
    public void MiddlePointTurnOnOff(bool onOrOff)
    {
        if (onOrOff)
            middlePoint.enabled = true;
        else
            middlePoint.enabled = false;
    }
}