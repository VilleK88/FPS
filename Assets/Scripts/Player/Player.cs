using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Plater Interaction Info")]
    [SerializeField] Camera cam;
    public Interactable focus;
    Ray ray;
    RaycastHit hit;
    Vector3 cursorPosition;
    [SerializeField] LayerMask groundLayer;

    private void Start()
    {
        
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
}
