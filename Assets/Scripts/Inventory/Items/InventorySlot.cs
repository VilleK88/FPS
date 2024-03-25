using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

public class InventorySlot : MonoBehaviour
{
    public int itemId = -1;
    public bool stackable;
    public int count = 0;
}
