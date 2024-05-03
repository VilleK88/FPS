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
    public bool weaponCollected; // can't collect same weapon twice
    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        anim = GetComponent<Animator>();
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
                Reload();
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
        if(thisWeaponModel != WeaponModel.Shotgun)
        {
            Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity); // instantiate the bullet
            bullet.transform.forward = shootingDirection; // Pointing the bullet to face the shooting direction
            bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse); // Shoot the bullet
        }
        else
        {
            for(int i = 0; i < 12; i++)
            {
                Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity); // instantiate the bullet
                bullet.transform.forward = shootingDirection; // Pointing the bullet to face the shooting direction
                bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse); // Shoot the bullet
            }
        }
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
    public void CheckAmmoStatus() // before reload
    {
        tempTotalAmmo = totalAmmo;
        for (int i = 0; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
        {
            InventoryItem inventoryItem = InventoryManager.instance.inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if(inventoryItem != null && inventoryItem.itemType == ItemType.Ammo)
            {
                Ammo ammo = inventoryItem.item as Ammo;
                if (ammo.ammoType == ammoType)
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
                            inventoryItem.PublicRemoveItem();
                        break;
                    }
                    else
                    {
                        int reduceBulletsLeft = magazineSize - bulletsLeft;
                        inventoryItem.count -= reduceBulletsLeft;
                        inventoryItem.RefreshCount();
                        slot.slotData.count -= reduceBulletsLeft;
                        if (inventoryItem.count <= 0)
                        {
                            int tempInventoryItemAmmoCount = inventoryItem.count;
                            inventoryItem.PublicRemoveItem();
                            if (tempInventoryItemAmmoCount < 0)
                                DecreaseAmmoCountOnNextAmmoItem(tempInventoryItemAmmoCount, i);
                        }
                        break;
                    }
                }
            }
        }
    }
    public void DecreaseAmmoCountOnNextAmmoItem(int decreaseAmount, int itemCount)
    {
        for (int i = itemCount+1; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
        {
            InventoryItem inventoryItem = InventoryManager.instance.inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.itemType == ItemType.Ammo)
            {
                Ammo ammo = inventoryItem.item as Ammo;
                if (ammo.ammoType == ammoType)
                {
                    if (decreaseAmount == 0)
                        break;
                    if (inventoryItem.count + decreaseAmount <= 0)
                    {
                        decreaseAmount += inventoryItem.count;
                        inventoryItem.count = 0;
                        inventoryItem.PublicRemoveItem();
                    }
                    else
                    {
                        inventoryItem.count += decreaseAmount;
                        inventoryItem.RefreshCount();
                        InventorySlot slot = inventoryItem.GetComponentInParent<InventorySlot>();
                        slot.slotData.count = inventoryItem.count;
                        decreaseAmount = 0;
                    }
                }
            }
        }
    }
    public void UpdateTotalAmmoStatus() // check/update total ammo status on inventory
    {
        totalAmmo = 0;
        for (int i = 0; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
        {
            InventoryItem inventoryItem = InventoryManager.instance.inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.itemType == ItemType.Ammo)
            {
                Item item = inventoryItem.item;
                Ammo ammo = item as Ammo;
                if(item != null && ammo != null)
                {
                    if (ammo.ammoType == ammoType)
                    {
                        if (inventoryItem.count > 0)
                            totalAmmo += inventoryItem.count;
                        else
                            totalAmmo = 0;
                    }
                }
            }
        }
    }
    void Reload()
    {
        UpdateTotalAmmoStatus();
        AudioManager.instance.PlaySound(reloadingSound);
        //anim.SetTrigger("Reload"); // reload animation not yet made
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }
    void ReloadCompleted()
    {
        int tempTotalBulletsLeft = tempTotalAmmo + bulletsLeft;
        if (tempTotalBulletsLeft >= magazineSize)
            bulletsLeft = magazineSize;
        else if(tempTotalBulletsLeft < magazineSize)
            bulletsLeft = tempTotalAmmo + bulletsLeft;
        isReloading = false;
        UpdateTotalAmmoStatus();
    }
    void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }
    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Shooting from the middle of the screen to check where are we pointing at
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point; // hitting something
        else
            targetPoint = ray.GetPoint(100); // shooting at the air
        Vector3 direction = targetPoint - bulletSpawn.position;
        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);
        return direction + new Vector3(x, y, 0); // returning the shooting direction and spread
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