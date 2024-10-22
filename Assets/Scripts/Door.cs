using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public Animator anim;
    public Door door;
    public BoxCollider boxCollider;
    public bool open = false;
    private void Start()
    {
        boxCollider.GetComponentInChildren<BoxCollider>();
        anim.GetComponentInParent<Animator>();
    }
    public void InteractWithDoor()
    {
        if (!open)
        {
            anim.SetTrigger("Open");
            open = true;
        }
        else
        {
            anim.SetTrigger("Close");
            open = false;
        }
    }
}