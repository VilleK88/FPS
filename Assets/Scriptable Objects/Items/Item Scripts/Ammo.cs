using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo", menuName = "Inventory/Ammo")]
public class Ammo : Item
{
    public int ammoAmount;
    public int ammoStaxMax;
    public AmmoType ammoType;
}

public enum AmmoType
{
    Pistol,
    AssaultRifle,
    Shotgun,
}
