using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Medpack Object", menuName = "Inventory System/Items/Medpack")]
public class MedpackObject : ItemObject
{
    //public int restoreHealthValue;
    public void Awake()
    {

        type = ItemType.Medpack;
    }
}
