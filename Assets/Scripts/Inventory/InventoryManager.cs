using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[Serializable]
public class InventoryManagerData
{
    public int activeWeapon;
}
public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion
    public InventoryManagerData inventoryData = new InventoryManagerData();
    public Dictionary<int, InventoryItem> inventory = new Dictionary<int, InventoryItem>();
    public Item[] itemsDatabase; // where the inventory gets the item scriptable objects when loading the game
    public GameObject[] weaponPrefabs;
    public InventorySlot slotPrefab;
    public InventoryItem inventoryItem; // prefab
    public Transform inventoryTransform; // where the slotPrefabs are instantiated.
    public InventorySlot[] inventorySlotsUI; // table where the slotPrefabs are put.
    public EquipmentSlot[] equipmentSlotsUI;
    [SerializeField] Animator inventoryAnim; // inventory screen
    [SerializeField] Animator equipmentAnim; // equipment screen
    public bool closed = true; // inventory ui closed or not
    public GameObject player;
    public MiddlePoint middlePoint; // crosshair
    public GameObject interactableIcon;
    public List<int> weaponIDsList = new List<int>();
    public Button selectSlot; // for keyboard use
    public InventoryItem tempInventoryItem; // for keyboard use
    public bool isPaused;
    public Image closeButton;
    private void Start()
    {
        inventorySlotsUI = new InventorySlot[20];
        for (int i = 0; i < 20; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, inventoryTransform);
            inventorySlotsUI[i] = slot;
            slot.InitializeButton();
        }
        if (GameManager.instance.loadInventory)
        {
            LoadInventorySlotData();
            AddSavedInventorySlotData();
            WeaponCollected();
            LoadHowManyBulletsLeftInMagazine();
            InitializeArmor();
            GameManager.instance.loadInventory = false;
        }
        else
            ClearInventory();
        selectSlot = inventorySlotsUI[0].GetComponent<Button>();
        selectSlot.Select();
        OnOffExitButton(false);
    }
    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.I) && !InGameMenuControls.instance.saveMenu.GetComponent<SaveMenu>().inputFieldOn) || Input.GetKeyDown(KeyCode.Tab))
        {
            if (PlayerManager.instance.dead) return;
            if (closed)
            {
                OpenInventory();
                PauseGame();
            }
            else
            {
                CloseInventory(true);
                StopPause();
            }
        }
        if (!closed || InGameMenuControls.instance.menuButtonsParentObject.activeSelf)
            return;
        EquipWeapon();
    }
    public void PauseGame()
    {
        PlayerManager.instance.MiddlePointTurnOnOff(false);
        PlayerManager.instance.TurnOnOrOffInteractable(false);
        isPaused = true;
        Time.timeScale = 0;
    }
    public void StopPause()
    {
        Time.timeScale = 1;
        isPaused = false;
        PlayerManager.instance.MiddlePointTurnOnOff(true);
        PlayerManager.instance.TurnOnOrOffInteractable(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void CancelKeyboardItemTransfer()
    {
        tempInventoryItem = null;
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            InventoryItem cancelItemTransfer = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if (cancelItemTransfer != null)
            {
                cancelItemTransfer.transparent.SetActive(false);
                cancelItemTransfer.CloseItemMenus();
            }
        }
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            InventoryItem cancelItemTransfer = equipmentSlotsUI[i].GetComponentInChildren<InventoryItem>();
            if (cancelItemTransfer != null)
            {
                cancelItemTransfer.transparent.SetActive(false);
                cancelItemTransfer.CloseItemMenus();
            }
        }
    }
    public void MakeTempInventoryItemForTransfer(InventoryItem inventoryItemInTransfer)
    {
        tempInventoryItem = inventoryItemInTransfer;
    }
    public bool CheckIfArmorSlotEmpty(InventoryItem newArmorItem) // when putting armor on armor slot
    {
        InventoryItem armorItem = equipmentSlotsUI[0].GetComponentInChildren<InventoryItem>();
        if (armorItem == null)
        {
            InventoryItem thisArmorItem = Instantiate(inventoryItem, equipmentSlotsUI[0].transform);
            thisArmorItem.item = newArmorItem.item;
            thisArmorItem.itemId = newArmorItem.itemId;
            thisArmorItem.pickupItemID = newArmorItem.pickupItemID;
            thisArmorItem.itemName = newArmorItem.itemName;
            thisArmorItem.img.sprite = newArmorItem.item.icon;
            thisArmorItem.itemType = newArmorItem.itemType;
            thisArmorItem.stackable = newArmorItem.stackable;
            thisArmorItem.maxStack = newArmorItem.maxStack;
            thisArmorItem.count = newArmorItem.count;
            thisArmorItem.InitializeSlot();
            return true;
        }
        return false;
    }
    public void InitializeArmor()
    {
        InventoryItem armorItem = equipmentSlotsUI[0].GetComponentInChildren<InventoryItem>();
        if (armorItem != null)
        {
            Armor armorSO = armorItem.item as Armor;
            PlayerHealth playerHealthScript = player.GetComponent<PlayerHealth>();
            if (playerHealthScript != null)
            {
                playerHealthScript.armorMultiplier = armorSO.armorMultiplier;
            }
        }
    }
    public bool CheckIfRoomInWeaponSlots(InventoryItem newWeaponItem)
    {
        for (int i = 1; i < equipmentSlotsUI.Length; i++)
        {
            InventoryItem weaponItem = equipmentSlotsUI[i].GetComponentInChildren<InventoryItem>();
            if (weaponItem == null)
            {
                InventoryItem thisItemWeapon = Instantiate(inventoryItem, equipmentSlotsUI[i].transform);
                thisItemWeapon.item = newWeaponItem.item;
                thisItemWeapon.itemId = newWeaponItem.itemId;
                thisItemWeapon.pickupItemID = newWeaponItem.pickupItemID;
                thisItemWeapon.itemName = newWeaponItem.itemName;
                thisItemWeapon.img.sprite = newWeaponItem.item.icon;
                thisItemWeapon.itemType = newWeaponItem.itemType;
                thisItemWeapon.stackable = newWeaponItem.stackable;
                thisItemWeapon.maxStack = newWeaponItem.maxStack;
                thisItemWeapon.count = newWeaponItem.count;
                thisItemWeapon.InitializeSlot();
                return true;
            }
        }
        return false;
    }
    void EquipWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(!PlayerManager.instance.dead) DrawWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!PlayerManager.instance.dead) DrawWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!PlayerManager.instance.dead) DrawWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!PlayerManager.instance.dead) DrawWeapon(4);
        }
    }
    void DrawWeapon(int index)
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            if (equipmentSlotsUI[index].slotData.itemId > -1)
            {
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    if (weaponSlots[i].GetComponent<Weapon>().weaponId == equipmentSlotsUI[index].slotData.itemId)
                    {
                        if (!weaponSlots[i].activeSelf)
                        {
                            weaponSlots[i].SetActive(!weaponSlots[i].activeSelf);
                            inventoryData.activeWeapon = index;
                            Weapon currentlyEquipedWeapon = weaponSlots[i].GetComponent<Weapon>();
                            currentlyEquipedWeapon.UpdateTotalAmmoStatus();
                            InitializeWeaponValues(currentlyEquipedWeapon, equipmentSlotsUI[index]);
                            if(currentlyEquipedWeapon.thisWeaponModel == WeaponModel.Knife)
                            {
                                currentlyEquipedWeapon.anim.SetLayerWeight(0, 1);
                                currentlyEquipedWeapon.anim.SetTrigger("Equip");
                                StartCoroutine(EquipWeaponDelay(currentlyEquipedWeapon));
                                InitializeKnifeForEquip(currentlyEquipedWeapon);
                            }
                        }
                        else
                        {
                            //weaponSlots[i].GetComponent<Weapon>().anim.SetLayerWeight(i, 0);
                            Weapon unequipedWeapon = weaponSlots[i].GetComponent<Weapon>();
                            if(unequipedWeapon.weaponObject != null) unequipedWeapon.weaponObject.SetActive(false);
                            weaponSlots[i].SetActive(!weaponSlots[i].activeSelf);
                            inventoryData.activeWeapon = 99;
                        }
                    }
                    else
                        weaponSlots[i].SetActive(false);
                }
            }
        }
        EnemyManager.Instance.CloseEnemyHealthbars();
    }
    void InitializeWeaponValues(Weapon currentlyEquipedWeapon, EquipmentSlot currentlyEquipedWeaponSlot)
    {
        InventoryItem currentInventoryItem = currentlyEquipedWeaponSlot.GetComponentInChildren<InventoryItem>();
        Item currentItem = currentInventoryItem.item;
        WeaponSO weaponSO = currentItem as WeaponSO;
        currentlyEquipedWeapon.name = weaponSO.name;
        currentlyEquipedWeapon.weaponId = weaponSO.weaponID;
        currentlyEquipedWeapon.thisWeaponModel = weaponSO.weaponModel;
        currentlyEquipedWeapon.currentShootingMode = weaponSO.shootingMode;
        currentlyEquipedWeapon.ammoType = weaponSO.ammoType;
        currentlyEquipedWeapon.shootingDelay = weaponSO.shootingDelay;
        currentlyEquipedWeapon.bulletsPerBurst = weaponSO.bulletsPerBurst;
        currentlyEquipedWeapon.spreadIntensity = weaponSO.spreadIntensity;
        currentlyEquipedWeapon.bulletVelocity = weaponSO.bulletVelocity;
        currentlyEquipedWeapon.bulletDamage = weaponSO.damage;
        currentlyEquipedWeapon.reloadTime = weaponSO.reloadTime;
        currentlyEquipedWeapon.magazineSize = weaponSO.magazineSize;
    }
    IEnumerator EquipWeaponDelay(Weapon currentlyEquipedWeapon)
    {
        yield return new WaitForSeconds(0.2f);
        currentlyEquipedWeapon.weaponObject.SetActive(true);
    }
    void InitializeKnifeForEquip(Weapon currentlyEquipedWeapon)
    {
        currentlyEquipedWeapon.secondKnifeAttack = false;
        currentlyEquipedWeapon.thirdKnifeAttack = false;
        currentlyEquipedWeapon.nextAttackCooldown = 0;
        currentlyEquipedWeapon.knifeScript.boxCollider.enabled = false;
        currentlyEquipedWeapon.knifeScript.damage = currentlyEquipedWeapon.bulletDamage;
        currentlyEquipedWeapon.GetComponentInChildren<KnifeHitbox>(true).damage = currentlyEquipedWeapon.bulletDamage;
    }
    public void DrawActiveWeapon() // after closing inventory or loading game
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (inventoryData.activeWeapon != 99)
        {
            for (int i = 0; i < equipmentSlotsUI.Length; i++)
            {
                if (equipmentSlotsUI[i].slotData.slotType == SlotType.Weapon && equipmentSlotsUI[i].slotData.itemId > -1 && i == inventoryData.activeWeapon)
                {
                    for (int secondI = 0; secondI < weaponSlots.Length; secondI++)
                    {
                        if (equipmentSlotsUI[i].slotData.itemId == weaponSlots[secondI].GetComponent<Weapon>().weaponId)
                        {
                            weaponSlots[secondI].SetActive(!weaponSlots[secondI].activeSelf);
                            weaponSlots[secondI].GetComponent<Weapon>().UpdateTotalAmmoStatus();
                        }
                    }
                }
            }
        }
    }
    public void HolsterWeapons()
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
                weaponSlots[i].SetActive(false);
        }
        EnemyManager.Instance.CloseEnemyHealthbars();
    }
    public void SaveHowManyBulletsLeftInMagazine()
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        GameManager.instance.bulletsLeft = new int[weaponSlots.Length];
        if (player != null)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                GameManager.instance.bulletsLeft[i] = weaponSlots[i].GetComponent<Weapon>().bulletsLeft;
            }
        }
    }
    public void LoadHowManyBulletsLeftInMagazine()
    {
        player = PlayerManager.instance.GetPlayer();
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        if (player != null)
        {
            for (int i = 0; i < GameManager.instance.bulletsLeft.Length; i++)
            {
                weaponSlots[i].SetActive(true);
                weaponSlots[i].GetComponent<Weapon>().bulletsLeft = GameManager.instance.bulletsLeft[i];
                weaponSlots[i].SetActive(false);
            }
        }
    }
    public void WeaponCollected()
    {
        player = PlayerManager.instance.GetPlayer();
        foreach (var slotData in GameManager.instance.inventorySlotsData)
            weaponIDsList.Add(slotData.itemId);
        foreach (var slotData in GameManager.instance.equipmentSlotsData)
            weaponIDsList.Add(slotData.itemId);
        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
        foreach (var weaponSlot in weaponSlots)
        {
            Weapon weaponScript = weaponSlot.GetComponent<Weapon>();
            if (weaponScript != null)
            {
                if (weaponIDsList.Contains(weaponScript.weaponId))
                {
                    weaponSlot.SetActive(true);
                    weaponScript.weaponCollected = true;
                    weaponSlot.SetActive(false);
                }
            }
        }
    }
    public void OpenInventory()
    {
        inventoryAnim.GetComponent<Animator>().SetBool("InventoryOn", true);
        equipmentAnim.GetComponent<Animator>().SetBool("EquipmentScreenOn", true);
        closed = false;
        HolsterWeapons();
        if (Score.Instance != null && AccountManager.Instance != null)
        {
            if (AccountManager.Instance.loggedIn)
                Score.Instance.killsText.enabled = false;
        }
        InGameMenuControls.instance.CloseSettings();
        InGameMenuControls.instance.CloseLoadMenu();
        InGameMenuControls.instance.CloseSaveMenu();
        InGameMenuControls.instance.menuButtonsParentObject.SetActive(false);
        OnOffExitButton(true);
        selectSlot.Select();
    }
    public void CloseInventory(bool esc)
    {
        inventoryAnim.GetComponent<Animator>().SetBool("InventoryOn", false);
        equipmentAnim.GetComponent<Animator>().SetBool("EquipmentScreenOn", false);
        CloseInventoryItemMenus();
        closed = true;
        HolsterWeapons();
        DrawActiveWeapon();
        InitializeArmor();
        if (Score.Instance != null && AccountManager.Instance != null)
        {
            if (AccountManager.Instance.loggedIn)
                Score.Instance.killsText.enabled = true;
        }
        if (tempInventoryItem != null)
        {
            tempInventoryItem.GetComponentInParent<InventorySlot>().CloseTransparentBG(tempInventoryItem);
            tempInventoryItem = null;
        }
        OnOffExitButton(false);
        if (!esc)
            StopPause();
    }
    public void ExitButton()
    {
        CloseInventory(false);
    }
    void OnOffExitButton(bool on)
    {
        if(on)
        {
            closeButton.enabled = true;
            closeButton.raycastTarget = true;
        }
        else
        {
            closeButton.enabled = false;
            closeButton.raycastTarget = false;
        }
    }
    void CloseInventoryItemMenus() // when closing inventory
    {
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            InventoryItem thisItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
            if (thisItem != null)
            {
                thisItem.sliderBG.SetActive(false);
                thisItem.itemMenuMoreThanOne.SetActive(false);
                thisItem.itemMenu.SetActive(false);
            }
        }
    }
    public bool AddInventoryItem(Item newItem, string pickupItemID, int count)
    {
        // add to stackable
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if (inventorySlotsUI[i].slotData.itemId == newItem.itemID && inventorySlotsUI[i].slotData.stackable == true)
            {
                InventoryItem itemInSlot = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                if (newItem.stackMax > inventorySlotsUI[i].slotData.count)
                {
                    int tempTotalCount = itemInSlot.count + count;
                    if (itemInSlot.maxStack >= tempTotalCount)
                    {
                        inventorySlotsUI[i].slotData.count += count;
                        itemInSlot.count += count;
                        itemInSlot.slider.maxValue += count;
                        itemInSlot.RefreshCount();
                    }
                    else
                    {
                        int decreaseCount = tempTotalCount - itemInSlot.maxStack;
                        inventorySlotsUI[i].slotData.count = newItem.stackMax;
                        itemInSlot.count = newItem.stackMax;
                        itemInSlot.slider.maxValue = newItem.stackMax;
                        itemInSlot.RefreshCount();

                        // find next empty slot
                        for (i = 0; i < inventorySlotsUI.Length; i++)
                        {
                            if (inventorySlotsUI[i].slotData.itemId == -1)
                            {
                                InitializeSlot(inventorySlotsUI[i], newItem);
                                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                                thisInventoryItem.GetComponent<InventoryItem>().item = newItem;
                                thisInventoryItem.GetComponent<InventoryItem>().itemId = newItem.itemID;
                                thisInventoryItem.GetComponent<InventoryItem>().itemName = newItem.itemName;
                                thisInventoryItem.GetComponent<InventoryItem>().itemType = newItem.itemType;
                                thisInventoryItem.GetComponent<InventoryItem>().stackable = newItem.stackable;
                                thisInventoryItem.GetComponent<InventoryItem>().maxStack = newItem.stackMax;
                                thisInventoryItem.GetComponent<InventoryItem>().img.sprite = newItem.icon;
                                thisInventoryItem.GetComponent<InventoryItem>().count = decreaseCount;
                                thisInventoryItem.GetComponent<InventoryItem>().slider.maxValue = thisInventoryItem.count;
                                InventorySlot slot = thisInventoryItem.GetComponentInParent<InventorySlot>();
                                slot.slotData.count = thisInventoryItem.count;
                                thisInventoryItem.RefreshCount();
                                if (thisInventoryItem.count > 1)
                                    thisInventoryItem.InitializeSlider();
                                thisInventoryItem.InitializeAmmoStatus();
                                break;
                            }
                        }
                    }
                    if (newItem.itemType == ItemType.Ammo)
                    {
                        player = PlayerManager.instance.GetPlayer();
                        GameObject[] weaponSlots = player.GetComponent<Player>().weaponSlots;
                        if (player != null)
                        {
                            for (i = 0; i < weaponSlots.Length; i++)
                            {
                                weaponSlots[i].GetComponent<Weapon>().UpdateTotalAmmoStatus();
                            }
                        }
                    }
                    return true;
                }
            }
        }
        // find next empty slot
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if (inventorySlotsUI[i].slotData.itemId == -1)
            {
                InitializeSlot(inventorySlotsUI[i], newItem);
                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.GetComponent<InventoryItem>().item = newItem;
                if (newItem.itemType == ItemType.Weapon)
                {
                    inventorySlotsUI[i].slotData.pickupItemID = pickupItemID;
                    thisInventoryItem.GetComponent<InventoryItem>().pickupItemID = pickupItemID;
                }
                thisInventoryItem.GetComponent<InventoryItem>().InitializeItem(count);
                return true;
            }
        }
        return false;
    }
    void InitializeSlot(InventorySlot slotGo, Item itemGo)
    {
        slotGo.slotData.itemId = itemGo.itemID;
        slotGo.slotData.itemName = itemGo.itemName;
        slotGo.slotData.itemType = itemGo.itemType;
        slotGo.slotData.stackable = itemGo.stackable;
        slotGo.slotData.stackMax = itemGo.stackMax;
        slotGo.slotData.count += 1;
    }
    public bool SplitStack(Item newItem, string pickupItemID, int count)
    {
        for (int i = 0; i < inventorySlotsUI.Length; i++) // find next empty slot
        {
            if (inventorySlotsUI[i].slotData.itemId == -1)
            {
                inventorySlotsUI[i].slotData.itemId = newItem.itemID;
                inventorySlotsUI[i].slotData.itemName = newItem.itemName;
                inventorySlotsUI[i].slotData.itemType = newItem.itemType;
                inventorySlotsUI[i].slotData.stackable = newItem.stackable;
                inventorySlotsUI[i].slotData.stackMax = newItem.stackMax;
                inventorySlotsUI[i].slotData.count = count;
                Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.GetComponent<InventoryItem>().item = newItem;
                if (newItem.itemType == ItemType.Weapon)
                {
                    inventorySlotsUI[i].slotData.pickupItemID = pickupItemID;
                    thisInventoryItem.GetComponent<InventoryItem>().pickupItemID = pickupItemID;
                }
                thisInventoryItem.GetComponent<InventoryItem>().InitializeItem(count);
                return true;
            }
        }
        return false;
    }
    public void SaveInventory() // and equipment to GameManager
    {
        GameManager.instance.inventorySlotsData = new InventorySlotData[inventorySlotsUI.Length];
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            GameManager.instance.inventorySlotsData[i] = inventorySlotsUI[i].slotData;
        }
        GameManager.instance.equipmentSlotsData = new InventorySlotData[equipmentSlotsUI.Length];
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            GameManager.instance.equipmentSlotsData[i] = equipmentSlotsUI[i].slotData;
        }
        GameManager.instance.inventoryData = inventoryData;
    }
    public void LoadInventorySlotData() // and equipment slot data from GameManager
    {
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            inventorySlotsUI[i].slotData = GameManager.instance.inventorySlotsData[i];
        }
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            equipmentSlotsUI[i].slotData = GameManager.instance.equipmentSlotsData[i];
        }
        inventoryData = GameManager.instance.inventoryData;
    }
    public void AddSavedInventorySlotData() // to inventory
    {
        for (int i = 0; i < inventorySlotsUI.Length; i++)
        {
            if (inventorySlotsUI[i].slotData.itemId > -1)
            {
                inventorySlotsUI[i].slotData.itemId = GameManager.instance.inventorySlotsData[i].itemId;
                inventorySlotsUI[i].slotData.itemType = GameManager.instance.inventorySlotsData[i].itemType;
                inventorySlotsUI[i].slotData.pickupItemID = GameManager.instance.inventorySlotsData[i].pickupItemID;
                inventorySlotsUI[i].slotData.stackable = GameManager.instance.inventorySlotsData[i].stackable;
                inventorySlotsUI[i].slotData.stackMax = GameManager.instance.inventorySlotsData[i].stackMax;
                InventoryItem newItemGo = Instantiate(inventoryItem, inventorySlotsUI[i].transform);
                InventoryItem thisInventoryItem = inventorySlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.item = itemsDatabase[inventorySlotsUI[i].slotData.itemId];
                newItemGo.itemId = thisInventoryItem.item.itemID;
                newItemGo.pickupItemID = inventorySlotsUI[i].slotData.pickupItemID;
                newItemGo.itemName = thisInventoryItem.item.itemName;
                newItemGo.itemType = thisInventoryItem.item.itemType;
                newItemGo.stackable = thisInventoryItem.item.stackable;
                newItemGo.maxStack = thisInventoryItem.item.stackMax;
                newItemGo.count = inventorySlotsUI[i].slotData.count;
                newItemGo.img.sprite = thisInventoryItem.item.icon;
                newItemGo.InitializeSlider();
                thisInventoryItem.GetComponent<InventoryItem>().RefreshCount();
            }
        }
        // add saved equipment slot data
        for (int i = 0; i < equipmentSlotsUI.Length; i++)
        {
            if (equipmentSlotsUI[i].slotData.itemId > -1)
            {
                equipmentSlotsUI[i].slotData.itemId = GameManager.instance.equipmentSlotsData[i].itemId;
                equipmentSlotsUI[i].slotData.itemType = GameManager.instance.equipmentSlotsData[i].itemType;
                equipmentSlotsUI[i].slotData.pickupItemID = GameManager.instance.equipmentSlotsData[i].pickupItemID;
                equipmentSlotsUI[i].slotData.stackable = GameManager.instance.equipmentSlotsData[i].stackable;
                equipmentSlotsUI[i].slotData.stackMax = GameManager.instance.equipmentSlotsData[i].stackMax;
                InventoryItem newItemGo = Instantiate(inventoryItem, equipmentSlotsUI[i].transform);
                InventoryItem thisInventoryItem = equipmentSlotsUI[i].GetComponentInChildren<InventoryItem>();
                thisInventoryItem.item = itemsDatabase[equipmentSlotsUI[i].slotData.itemId];
                newItemGo.itemId = thisInventoryItem.item.itemID;
                newItemGo.itemName = thisInventoryItem.item.itemName;
                newItemGo.itemType = thisInventoryItem.item.itemType;
                newItemGo.pickupItemID = thisInventoryItem.pickupItemID;
                newItemGo.stackable = thisInventoryItem.item.stackable;
                newItemGo.maxStack = thisInventoryItem.item.stackMax;
                newItemGo.count = equipmentSlotsUI[i].slotData.count;
                newItemGo.img.sprite = thisInventoryItem.item.icon;
                thisInventoryItem.GetComponent<InventoryItem>().RefreshCount();
            }
        }
    }
    public void ClearInventory() // and equipment
    {
        for (int i = 0; i < GameManager.instance.inventorySlotsData.Length; i++)
        {
            GameManager.instance.inventorySlotsData[i] = null;
        }
        for (int i = 0; i < GameManager.instance.equipmentSlotsData.Length; i++)
        {
            GameManager.instance.equipmentSlotsData[i] = null;
        }
        GameManager.instance.inventoryData = null;
    }
}