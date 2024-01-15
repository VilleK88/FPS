using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public int itemID;
    public bool isDefaultItem = false;

    [Header("Only gameplay")]
    public ItemType type;
    public Vector2Int range = new Vector2Int(5, 4);
    public int cashValue;
    public int damageOutput;

    [Header("Only UI")]
    public bool stackable = true;

    [Header("Both")]
    public Sprite icon = null;

    [TextArea]
    public string description;

    public virtual void Use()
    {
        // Use the item
        // Something might happen

        //Debug.Log("Using " + name);
    }
}

public enum ItemType
{
    cash, healthPack, armor, pistol, rifle, shotgun,
}
