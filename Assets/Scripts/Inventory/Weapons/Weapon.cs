using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon model, shooting mode and weapon id")]
    public WeaponModel thisWeaponModel;
    public ShootingMode currentShootingMode;
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

    public Vector3 spawnPosition = new Vector3(0.689f, -1.14f, 2.213f);
    public Vector3 spawnRotation = new Vector3(0, 0, 0);
    public bool isActiveWeapon;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        anim = GetComponent<Animator>();
        bulletsLeft = magazineSize;
        totalAmmo -= magazineSize;
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

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading && totalAmmo > 0) // reload weapon
            Reload();

        /*if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0) // automatic weapon reload
            Reload();*/

        if (readyToShoot && isShooting && bulletsLeft > 0 && !isReloading)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

        if (AmmoManager.Instance.ammoDisplay != null)
        {
            //AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
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

        // instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // Pointing the bullet to face the shooting direction
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // Checking if we are done shooting
        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst mode
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    void CheckAmmoStatus()
    {
        for(int i = 0; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
        {
            InventoryItem inventoryItem = InventoryManager.instance.inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if(inventoryItem != null)
            {
                if(inventoryItem.itemType == ItemType.Ammo)
                {
                    //if(inventoryItem.item.Get)
                }
            }
        }
    }

    void Reload()
    {
        tempTotalAmmo = totalAmmo;
        if (totalAmmo >= magazineSize)
            totalAmmo -= magazineSize;
        else
            totalAmmo -= totalAmmo;

        AudioManager.instance.PlaySound(reloadingSound);
        //anim.SetTrigger("Reload"); // reload animation not yet made
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    void ReloadCompleted()
    {
        if (totalAmmo >= magazineSize)
            bulletsLeft = magazineSize;
        else if(tempTotalAmmo >= magazineSize)
            bulletsLeft = magazineSize;
        else
            bulletsLeft = tempTotalAmmo;

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