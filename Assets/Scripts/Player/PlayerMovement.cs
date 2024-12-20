using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public CharacterController controller;
    [SerializeField] CapsuleCollider capsuleCollider;
    [Header("Movement")]
    float x, z;
    Vector3 move;
    float speed = 8;
    float sneakSpeed = 5.5f;
    float runningSpeed = 15;
    float originalSpeed;
    public bool moving;
    [Header("Sneaking")]
    public bool sneaking;
    float characterControllerOriginalHeight = 3.5f;
    float characterControllerSneakingHeight = 1.5f;
    [Header("Jumping")]
    Vector3 velocity;
    float gravity = -19.62f;
    [SerializeField] Transform groundCheck;
    float groundDistance = 0.4f; // 0.4f original groundDistance
    float sneakGroundDistance = 0.9f;
    float originalGroundDistance = 0.4f;
    public LayerMask groundMask;
    float jumpHeight = 3;
    private void Start()
    {
        originalSpeed = speed;
        if(GameManager.instance.ifSneaking)
        {
            Sneak();
            GameManager.instance.ifSneaking = false;
        }
    }
    private void Update()
    {
        if (!InventoryManager.instance.closed || InGameMenuControls.instance.menuButtonsParentObject.activeSelf)
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
                Sneak();
            else
                StopSneaking();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = runningSpeed;
            if (sneaking)
                StopSneaking();
        }
        else if (!sneaking)
            speed = originalSpeed;
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            if (sneaking)
                StopSneaking();
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    void Sneak()
    {
        sneaking = true;
        speed = sneakSpeed;
        PlayerManager.instance.sneakIndicatorText.text = "Sneaking";
        controller.height = characterControllerSneakingHeight;
        capsuleCollider.height = characterControllerSneakingHeight;
        groundDistance = sneakGroundDistance;
    }
    void StopSneaking()
    {
        sneaking = false;
        speed = originalSpeed;
        PlayerManager.instance.sneakIndicatorText.text = "";
        controller.height = characterControllerOriginalHeight;
        capsuleCollider.height = characterControllerOriginalHeight;
        groundDistance = originalGroundDistance;
    }
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
}