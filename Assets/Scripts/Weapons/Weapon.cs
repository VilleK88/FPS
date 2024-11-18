using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    public bool silenced;
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
    public float bulletDamage;
    [Header("Particle Effect, Animation, Sound")]
    public GameObject muzzleEffect;
    [SerializeField] public Animator anim;
    [SerializeField] AudioClip shootingSound;
    [SerializeField] AudioClip reloadingSound;
    [SerializeField] AudioClip emptyMagazineSound;
    [Header("Loading")]
    public float reloadTime = 1.5f;
    public int magazineSize = 7, bulletsLeft, totalAmmo;
    public int tempTotalAmmo;
    public bool isReloading;
    public bool weaponCollected; // can't collect same weapon twice
    [Header("Show Enemy Healthbar")]
    float rayDistance = 50f;
    public LayerMask enemyLayer;
    public EnemyHealth lastHitEnemy;
    [Header("Knife")]
    [HideInInspector] public float nextAttackCooldown = 0;
    [HideInInspector] public bool secondKnifeAttack = false;
    [HideInInspector] public bool thirdKnifeAttack = false;
    public KnifeHitbox knifeScript;
    public GameObject weaponObject;
    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        //anim = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (!InventoryManager.instance.closed || InGameMenuControls.instance.menuButtonsParentObject.activeSelf)
            return;
        if (thisWeaponModel != WeaponModel.Knife)
            RangedWeapon();
        else
            Knife();
        ShowEnemyHealthBar();
    }
    void RangedWeapon()
    {
        if (bulletsLeft == 0 && Input.GetKeyDown(KeyCode.Mouse0))
            AudioManager.instance.PlayPlayerSound(emptyMagazineSound);
        if (currentShootingMode == ShootingMode.Auto)
            isShooting = Input.GetKey(KeyCode.Mouse0); // holding down left mouse button
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
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
        else if (thisWeaponModel == WeaponModel.AssaultRifle && !isShooting)
            anim.SetBool("Shoot", false);
        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{totalAmmo}";
        }
    }
    void Knife()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!secondKnifeAttack && !thirdKnifeAttack)
            {
                anim.SetTrigger("KnifeAttack1");
                secondKnifeAttack = true;
                nextAttackCooldown = 1.2f;
            }
            else if(secondKnifeAttack && !thirdKnifeAttack)
            {
                anim.SetTrigger("KnifeAttack2");
                thirdKnifeAttack = true;
                nextAttackCooldown = 1.2f;
            }
            else if(secondKnifeAttack && thirdKnifeAttack)
            {
                anim.SetTrigger("KnifeAttack3");
                nextAttackCooldown = 1.2f;
                secondKnifeAttack = false;
                thirdKnifeAttack = false;
            }
        }
        if (nextAttackCooldown > 0)
            nextAttackCooldown -= Time.deltaTime;
        else
        {
            secondKnifeAttack = false;
            thirdKnifeAttack = false;
        }
    }
    void ShowEnemyHealthBar()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance, enemyLayer))
        {
            //EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.ShowHealth();
                lastHitEnemy = enemyHealth;
                return;
            }
        }
        if (lastHitEnemy != null)
            lastHitEnemy.HideHealth();
    }
    void FireWeapon()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        if(thisWeaponModel != WeaponModel.AssaultRifle)
            anim.SetTrigger("Shoot");
        else
        {
            anim.SetBool("Shoot", true);
        }
        //anim.SetTrigger("Shoot");
        AudioManager.instance.PlayPlayerSound(shootingSound);
        readyToShoot = false;
        if (thisWeaponModel != WeaponModel.Shotgun)
        {
            Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity); // instantiate the bullet
            bullet.transform.forward = shootingDirection; // Pointing the bullet to face the shooting direction
            bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse); // Shoot the bullet
            bullet.GetComponent<Bullet>().damage = bulletDamage;
        }
        else
        {
            for (int i = 0; i < 12; i++)
            {
                Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity); // instantiate the bullet
                bullet.transform.forward = shootingDirection; // Pointing the bullet to face the shooting direction
                bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse); // Shoot the bullet
                bullet.GetComponent<Bullet>().damage = bulletDamage;
            }
        }
        if (allowReset) // Checking if we are done shooting
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) // Burst mode
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
            if (inventoryItem != null && inventoryItem.itemType == ItemType.Ammo)
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
        for (int i = itemCount + 1; i < InventoryManager.instance.inventorySlotsUI.Length; i++)
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
                if (item != null && ammo != null)
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
        AudioManager.instance.PlayPlayerSound(reloadingSound);
        if (thisWeaponModel == WeaponModel.Shotgun)
            anim.SetTrigger("Reload");
        //anim.SetTrigger("Reload"); // reload animation not yet made
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }
    void ReloadCompleted()
    {
        int tempTotalBulletsLeft = tempTotalAmmo + bulletsLeft;
        if (tempTotalBulletsLeft >= magazineSize)
            bulletsLeft = magazineSize;
        else if (tempTotalBulletsLeft < magazineSize)
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
        {
            targetPoint = hit.point; // hitting something
        }
        else
            targetPoint = ray.GetPoint(100); // shooting at the air
        Vector3 direction = targetPoint - bulletSpawn.position;
        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);
        return direction + new Vector3(x, y, 0); // returning the shooting direction and spread
    }
}