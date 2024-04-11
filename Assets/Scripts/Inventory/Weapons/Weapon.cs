using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon model, shooting mode and weapon id")]
    public WeaponModel thisWeaponModel;
    public ShootingMode currentShootingMode;
    public AmmoType ammoType;
    public int weaponId;

    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 0.3f;

    [Header("Burst")]
    public int bulletsPerBurst = 1;
    public int burstBulletsLeft;

    [Header("Spread")]
    public float spreadIntensity;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 500;
    public float bulletPrefabLifeTime = 3f;

    [Header("Particle Effect, Animation, Sound")]
    public GameObject muzzleEffect;
    Animator anim;
    [SerializeField] AudioClip shootingSound;
    [SerializeField] AudioClip reloadingSound;
    [SerializeField] AudioClip emptyMagazineSound;

    [Header("Loading")]
    public float reloadTime = 1.5f;
    public int magazineSize = 7, bulletsLeft, totalAmmo;
    public int tempTotalAmmo;
    public bool isReloading;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        anim = GetComponent<Animator>();
        /*if(!GameManager.instance.loadInventory)
            bulletsLeft = magazineSize;*/
    }

    private void Update()
    {
        if (!InventoryManager.instance.closed || InGameMenuControls.instance.menuButtons.activeSelf)
            return;

        if (bulletsLeft == 0 && isShooting)
            AudioManager.instance.PlaySound(emptyMagazineSound);

        if (currentShootingMode == ShootingMode.Auto)
            isShooting = Input.GetKey(KeyCode.Mouse0); // holding down left mouse button
        else if(currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            isShooting = Input.GetKeyDown(KeyCode.Mouse0); // clicking left mouse button once

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading) // reload weapon
        {
            CheckAmmoStatus();
            if (totalAmmo > 0)
            {
                Reload();
            }
        }

        if (readyToShoot && isShooting && bulletsLeft > 0 && !isReloading)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{totalAmmo}";
        }
    }

    void FireWeapon()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        anim.SetTrigger("Recoil");
        AudioManager.instance.PlaySound(shootingSound);

        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity); // instantiate the bullet

        bullet.transform.forward = shootingDirection; // Pointing the bullet to face the shooting direction

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse); // Shoot the bullet

        if (allowReset) // Checking if we are done shooting
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) // Burst mode
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    public void CheckAmmoStatus()
    {
        for(int i = 0; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
        {
            InventoryItem inventoryItem = InventoryManager.instance.inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if(inventoryItem != null && inventoryItem.itemType == ItemType.Ammo &&
                inventoryItem.item.ammoType == ammoType)
            {
                totalAmmo = inventoryItem.count;
                InventorySlot slot = inventoryItem.GetComponentInParent<InventorySlot>();
                if (inventoryItem.count >= magazineSize)
                {
                    int reduceBulletsLeft = magazineSize - bulletsLeft;
                    inventoryItem.count -= reduceBulletsLeft;
                    inventoryItem.RefreshCount();
                    slot.slotData.count -= reduceBulletsLeft;
                    if (inventoryItem.count <= 0)
                        inventoryItem.DestroyItem();
                    break;
                }
                else
                {
                    int reduceBulletsLeft = magazineSize - bulletsLeft;
                    inventoryItem.count -= reduceBulletsLeft;
                    inventoryItem.RefreshCount();
                    slot.slotData.count -= reduceBulletsLeft;
                    if (inventoryItem.count <= 0)
                        inventoryItem.DestroyItem();
                    break;
                }
            }
        }
    }

    public void InitializeAmmoStatus() // when initializing ammo item
    {
        totalAmmo = 0;
        for (int i = 0; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
        {
            InventoryItem inventoryItem = InventoryManager.instance.inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.itemType == ItemType.Ammo &&
                inventoryItem.item.ammoType == ammoType)
            {
                if (inventoryItem.item.ammoType == ammoType)
                {
                    totalAmmo += inventoryItem.count;
                }
            }
        }
    }

    void Reload()
    {
        tempTotalAmmo = totalAmmo;
        if (totalAmmo >= magazineSize)
        {
            int reduceBulletsLeft = magazineSize - bulletsLeft;
            totalAmmo -= reduceBulletsLeft;
        }
        else
        {
            int reduceBulletsLeft = magazineSize - bulletsLeft;
            int tempTotalAmmo = totalAmmo - reduceBulletsLeft;
            if (tempTotalAmmo >= 0)
                totalAmmo -= reduceBulletsLeft;
            else
                totalAmmo = 0;
        }

        AudioManager.instance.PlaySound(reloadingSound);
        //anim.SetTrigger("Reload"); // reload animation not yet made
        isReloading = true;
        InitializeAmmoStatus();
        Invoke("ReloadCompleted", reloadTime);
    }

    void ReloadCompleted()
    {
        if (totalAmmo >= magazineSize)
        {
            bulletsLeft = magazineSize;
        }
        else if(tempTotalAmmo >= magazineSize)
        {
            bulletsLeft = magazineSize;
        }
        else
        {
            int tempBulletsLeft = bulletsLeft + tempTotalAmmo;
            if (tempBulletsLeft >= magazineSize)
                bulletsLeft = magazineSize;
            else
                bulletsLeft = bulletsLeft + tempTotalAmmo;
        }

        tempTotalAmmo = 0;
        isReloading = false;
    }

    void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // Shooting from the middle of the screen to check where are we pointing at
        //Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // hitting something
            targetPoint = hit.point;
        }
        else
        {
            // shooting at the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);

        // returning the shooting direction and spread
        return direction + new Vector3(x, y, 0);
    }
}

public enum ShootingMode
{
    Single,
    Burst,
    Auto
}

public enum WeaponModel
{
    Pistol,
    AssaultRifle,
    Shotgun
}