using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Interactable : MonoBehaviour
{
    public float radius = 3; // 3 original radius
    public Transform interactionTransform;
    bool isFocus = false;
    Transform player;
    bool hasInteracted = false;
    float distance;
    public GameObject imgObject;
    public Animator anim;
    public Door door;
    private void Start()
    {
        if (anim != null)
            anim.GetComponent<Animator>();
        if (door != null)
            door.GetComponent<Door>();
    }
    public virtual void Interact()
    {
        //Debug.Log("Interacting with " + transform.name);
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