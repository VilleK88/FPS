using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Inventory/Weapon")]
public class WeaponSO : Item
{
    [Header("WeaponType, ShootingMode, AmmoType, WeaponID")]
    public WeaponModel weaponModel;
    public ShootingMode shootingMode;
    public AmmoType ammoType;
    public int weaponID;
    [Header("Shooting")]
    public float shootingDelay;
    public int bulletsPerBurst;
    public float spreadIntensity;
    [Header("Bullet")]
    public float bulletVelocity;
    public float damage;
    [Header("Loading")]
    public float reloadTime;
    public int magazineSize;
}
public enum WeaponModel
{
    Pistol,
    AssaultRifle,
    Shotgun,
    Knife
}
public enum ShootingMode
{
    Single,
    Burst,
    Auto
}