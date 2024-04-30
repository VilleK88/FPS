using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [Header("Plater Interaction Info")]
    [SerializeField] Camera cam;
    public Interactable focus;
    Ray ray;
    RaycastHit hit;
    Vector3 cursorPosition;
    [SerializeField] LayerMask groundLayer;
    public VectorValue startingPosition;
    public GameObject[] weaponSlots;
    [Header("References")]
    public Transform attackPoint;
    public GameObject objectToThrow;
    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;
    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;
    bool readyToThrow = true;
    private void Start()
    {
        if (GameManager.instance.changeScene)
            InventoryManager.instance.LoadHowManyBulletsLeftInMagazine();
        if (GameManager.instance.loadPlayerPosition)
        {
            LoadPlayerTransformPosition();
            GameManager.instance.loadPlayerPosition = false;
            InventoryManager.instance.LoadHowManyBulletsLeftInMagazine();
        }
        else
        {
            if(characterController != null && GameManager.instance.changeScene)
            {
                characterController.enabled = false;
                characterController.transform.position = startingPosition.initialValue;
                characterController.enabled = true;
                GameManager.instance.changeScene = false;
            }
        }
    }
    private void Update()
    {
        MouseInteraction();
        if(Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
            Throw();
    }
    void Throw()
    {
        readyToThrow = false;
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.transform.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwardForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        totalThrows--;
        Invoke(nameof(ResetThrow), throwCooldown);
    }
    void ResetThrow()
    {
        readyToThrow = true;
    }
    void MouseInteraction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(ray, out hit, 100, groundLayer))
                RemoveFocus();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 100))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null)
                    SetFocus(interactable);
            }
        }
    }
    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
                focus.OnDefocused();
            focus = newFocus;
        }
        newFocus.OnFocused(transform);
    }
    void RemoveFocus()
    {
        if (focus != null)
            focus.OnDefocused();
        focus = null;
    }
    public void SavePlayerTransformPosition()
    {
        GameManager.instance.x = transform.position.x;
        GameManager.instance.y = 4;
        GameManager.instance.z = transform.position.z;
        GameManager.instance.xRotation = transform.rotation.eulerAngles.x;
        GameManager.instance.yRotation = transform.rotation.eulerAngles.y;
        GameManager.instance.zRotation = transform.rotation.eulerAngles.z;
    }
    public void LoadPlayerTransformPosition()
    {
        characterController.enabled = false;
        characterController.transform.position = new Vector3(GameManager.instance.x, GameManager.instance.y, GameManager.instance.z);
        characterController.transform.rotation = Quaternion.Euler(GameManager.instance.xRotation, GameManager.instance.yRotation, GameManager.instance.zRotation);
        characterController.enabled = true;
    }
}