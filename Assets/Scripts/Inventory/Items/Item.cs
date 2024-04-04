using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public bool stackable = true;
    public int stackMax;
    public Sprite icon = null;

    [TextArea]
    public string description;

    public virtual void Use()
    {

    }
}

public enum ItemType
{
    Default,
    Medpack,
    Armor,
    Weapon,
}