using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Header("Movement")]
    float x, z;
    Vector3 move;
    float speed = 12;

    [Header("Jumping")]
    Vector3 velocity;
    float gravity = -19.62f;
    [SerializeField] Transform groundCheck;
    float groundDistance = 0.4f;
    public LayerMask groundMask;
    float jumpHeight = 3;

    private void Update()
    {
        if (!InventoryManager.instance.closed || InGameMenuControls.instance.menuButtons.activeSelf)
            return;

        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
}
