using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Weapon : MonoBehaviour
{
    public Camera playerCamera;

    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 0.3f;
    public ShootingMode currentShootingMode;

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
    public int magazineSize = 7, bulletsLeft;
    public bool isReloading;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        anim = GetComponent<Animator>();
        bulletsLeft = magazineSize;
    }

    private void Update()
    {
        if (bulletsLeft == 0 && isShooting)
            AudioManager.instance.PlaySound(emptyMagazineSound);

        if (!InventoryManager.instance.closed || InGameMenuControls.instance.menuButtons.activeSelf)
            return;

        if(currentShootingMode == ShootingMode.Auto)
            isShooting = Input.GetKey(KeyCode.Mouse0); // holding down left mouse button
        else if(currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            isShooting = Input.GetKeyDown(KeyCode.Mouse0); // clicking left mouse button once

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading) // reload weapon
            Reload();

        /*if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0) // automatic weapon reload
            Reload();*/

        if (readyToShoot && isShooting && bulletsLeft > 0)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

        if (AmmoManager.Instance.ammoDisplay != null)
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
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

    void Reload()
    {
        AudioManager.instance.PlaySound(reloadingSound);
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
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