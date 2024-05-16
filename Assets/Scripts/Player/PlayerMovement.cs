using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [Header("Movement")]
    float x, z;
    Vector3 move;
    float speed = 8;
    float sneakSpeed = 4;
    float runningSpeed = 15;
    float originalSpeed;
    public bool moving;
    public bool sneaking;
    [Header("Jumping")]
    Vector3 velocity;
    float gravity = -19.62f;
    [SerializeField] Transform groundCheck;
    float groundDistance = 0.4f;
    public LayerMask groundMask;
    float jumpHeight = 3;
    private void Start()
    {
        originalSpeed = speed;
    }
    private void Update()
    {
        if (!InventoryManager.instance.closed || InGameMenuControls.instance.menuButtons.activeSelf)
            return;
        if (IsGrounded() && velocity.y < 0)
            velocity.y = -2;
        Move();
        Jump();
    }
    void Move()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            moving = true;
        else
            moving = false;
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!sneaking)
            {
                sneaking = true;
                speed = sneakSpeed;
            }
            else
            {
                sneaking = false;
                speed = originalSpeed;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) && !sneaking)
            speed = runningSpeed;
        else if(!sneaking)
            speed = originalSpeed;
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
}