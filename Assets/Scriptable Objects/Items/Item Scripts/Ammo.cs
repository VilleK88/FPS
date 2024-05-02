using UnityEngine;
[CreateAssetMenu(fileName = "Ammo", menuName = "Inventory/Ammo")]
public class Ammo : Item
{
    public AmmoType ammoType;
}
public enum AmmoType
{
    Default,
    Pistol,
    AssaultRifle,
    Shotgun,
}