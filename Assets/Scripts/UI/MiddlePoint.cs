using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiddlePoint : MonoBehaviour
{
    public Image img;

    private void Start()
    {
        //img = GetComponent<Image>();
    }

    /*private void Update()
    {
        if (!InventoryManager.instance.closed || InGameMenuControls.instance.menuButtons.activeSelf)
        {
            img.enabled = false;
            img.raycastTarget = false;
        }
        else
        {
            img.enabled = true;
            img.raycastTarget = true;
        }
    }*/
}
