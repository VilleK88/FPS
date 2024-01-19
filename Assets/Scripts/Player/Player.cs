using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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


    private void Start()
    {
        if (GameManager.instance.loadPlayerPosition)
        {
            LoadPlayerTransformPosition();
            Debug.Log("PlayerManagerin LoadPlayerTransformPosition debuggaa");
            GameManager.instance.loadPlayerPosition = false;
        }
        else
        {
            if(characterController != null)
            {
                characterController.enabled = false;
                characterController.transform.position = startingPosition.initialValue;
                characterController.enabled = true;
            }
        }
    }

    private void Update()
    {
        MouseInteraction();
    }

    void MouseInteraction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(ray, out hit, 100, groundLayer))
            {
                RemoveFocus();
            }
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, 100))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null)
                {
                    SetFocus(interactable);
                }
            }
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
            {
                focus.OnDefocused();
            }

            focus = newFocus;
        }

        newFocus.OnFocused(transform);
    }

    void RemoveFocus()
    {
        if(focus != null)
        {
            focus.OnDefocused();
        }

        focus = null;
    }

    public void SavePlayerTransformPosition()
    {
        GameManager.instance.x = transform.position.x;
        GameManager.instance.y = 4;
        GameManager.instance.z = transform.position.z;
    }

    public void LoadPlayerTransformPosition()
    {
        //transform.position = new Vector3(GameManager.instance.x, GameManager.instance.y, GameManager.instance.z);
        characterController.enabled = false;
        characterController.transform.position = new Vector3(GameManager.instance.x, GameManager.instance.y, GameManager.instance.z);
        characterController.enabled = true;
    }
}
