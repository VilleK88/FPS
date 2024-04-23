using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Interactable : MonoBehaviour
{
    float radius = 3;
    public Transform interactionTransform;
    bool isFocus = false;
    Transform player;
    bool hasInteracted = false;
    float distance;
    public virtual void Interact()
    {
        Debug.Log("Interacting with " + transform.name);
    }
    private void Update()
    {
        if(isFocus && !hasInteracted)
        {
            distance = Vector3.Distance(player.position, interactionTransform.position);
            if(distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
        else
        {
            OnDefocused();
        }
    }
    public void OnFocused(Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
        hasInteracted = false;
    }
    public void OnDefocused()
    {
        isFocus = false;
        player = null;
        hasInteracted = false;
    }
    private void OnDrawGizmosSelected()
    {
        if(interactionTransform == null)
        {
            interactionTransform = transform;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}