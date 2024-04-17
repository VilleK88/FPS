using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.EventSystems;

public class MoveItem : MonoBehaviour
{
    float moveSpeed = 200;
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.T))
        {
            SetToParentsChild();
        }
    }

    void SetToParentsChild()
    {
        InventoryItem thisItem = GetComponent<InventoryItem>();
        transform.SetParent(thisItem.parentAfterDrag);
        thisItem.img.raycastTarget = true;
        thisItem.countText.raycastTarget = true;
        thisItem.dragging = false;
        thisItem.InitializeSlot();
    }
}
