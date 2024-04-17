using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : InventorySlot
{
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }
    void OnButtonClicked()
    {
        InventoryItem itemInThisSlot = GetComponentInChildren<InventoryItem>();
        if (itemInThisSlot != null)
            itemInThisSlot.UseItem();
    }
}