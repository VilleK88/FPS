using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public BoxCollider boxCollider;
    public Animator anim;
    public bool open= false;
    private void Start()
    {
        boxCollider.GetComponent<BoxCollider>();
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