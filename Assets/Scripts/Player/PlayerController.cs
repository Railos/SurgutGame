using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Input References")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    [Header("Movement")]
    public float moveSpeed = 15f;
    public float groundDrag = 6f;
    public float jumpForce = 12f;
    public float airMultiplier = 0.4f;

    [Header("Jumping")]
    [SerializeField] private int maxJumpCount = 3;

    [Header("Jumping Gravity (The Snappy Part)")]
    public float fallMultiplier = 2.5f; // Higher = heavier fall (try 2.5 to 3.5)
    public float lowJumpMultiplier = 2f; // For variable jump height (light tap = short hop)

    [Header("Ground Detection")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    // Changed to private set, public get for Camera script
    public bool isGrounded { get; private set; } 

    [Header("Slope Handling")]
    public float maxSlopeAngle = 45f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    // References
    public Transform orientation;
    public bool isDashing;
    Rigidbody rb;
    Vector2 moveInput;
    int remainingJumps;
    bool wasGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        remainingJumps = maxJumpCount;
        
        if (moveAction != null) moveAction.action.Enable();
        if (jumpAction != null) jumpAction.action.Enable();
    }

    void Update()
    {
        // 1. Check Ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (isGrounded && !wasGrounded)
        {
            remainingJumps = maxJumpCount;
        }
        wasGrounded = isGrounded;

        // 2. Read Input
        moveInput = moveAction.action.ReadValue<Vector2>();

        if (jumpAction.action.WasPressedThisFrame() && remainingJumps > 0)
        {
            Jump();
            remainingJumps--;
        }

        SpeedControl();

        if (isDashing)
        {
            rb.linearDamping = 0;
        }
        else
        {
            if (isGrounded)
                rb.linearDamping = groundDrag;
            else
                rb.linearDamping = 0;
        }
        // 3. Handle Drag
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyExtraGravity(); // Apply the "heavy" feel
    }

    private void ApplyExtraGravity()
    {
        // If falling (velocity < 0), apply extra gravity to make it feel heavy
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        // If jumping up but button released (Variable Jump Height), pull down faster
        else if (rb.linearVelocity.y > 0 && !jumpAction.action.IsPressed())
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (isDashing) return;

        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetSlope), 0.25f);
    }

    private void ResetSlope()
    {
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    
    // Public accessor for Camera script
    public bool IsGrounded() => isGrounded;
}
