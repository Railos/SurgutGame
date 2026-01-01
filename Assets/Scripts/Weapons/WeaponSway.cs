using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference lookAction;
    public InputActionReference moveAction;

    [Header("Sway Settings (Look)")]
    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float swaySmooth = 4f;

    [Header("Tilt Settings (Strafe)")]
    public float tiltAmount = 4f; // Angle to tilt when strafing
    public float tiltSmooth = 4f;

    [Header("Rotation Sway (Turning)")]
    public float rotationSwayAmount = 2f; // Angle to rotate gun when looking
    public float maxRotationSway = 5f;

    private Vector3 initialPos;
    private Quaternion initialRot;
    private Vector2 lookInput;
    private Vector2 moveInput;

    void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;

        if (lookAction != null) lookAction.action.Enable();
        if (moveAction != null) moveAction.action.Enable();
    }

    void Update()
    {
        GetInput();

        // 1. Calculate Sway (Position Lag)
        // Invert input so weapon drags BEHIND the look direction
        float moveX = -lookInput.x * swayAmount;
        float moveY = -lookInput.y * swayAmount;

        moveX = Mathf.Clamp(moveX, -maxSwayAmount, maxSwayAmount);
        moveY = Mathf.Clamp(moveY, -maxSwayAmount, maxSwayAmount);

        Vector3 finalPos = new Vector3(moveX, moveY, 0);

        // 2. Calculate Tilt (Rotation Banking)
        // Z-Tilt based on Strafe (A/D)
        float tiltZ = -moveInput.x * tiltAmount; 
        
        // Y-Rotation (Yaw) based on Look X
        float rotY = -lookInput.x * rotationSwayAmount;
        rotY = Mathf.Clamp(rotY, -maxRotationSway, maxRotationSway);
        
        // X-Rotation (Pitch) based on Look Y
        float rotX = lookInput.y * rotationSwayAmount;
        rotX = Mathf.Clamp(rotX, -maxRotationSway, maxRotationSway);

        Quaternion finalRot = Quaternion.Euler(rotX, rotY, tiltZ);

        // 3. Apply
        // Smoothly lerp to target
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + initialPos, Time.deltaTime * swaySmooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRot * initialRot, Time.deltaTime * tiltSmooth);
    }

    void GetInput()
    {
        if (lookAction != null)
        {
            // Read Mouse Delta directly (Snappy)
            lookInput = lookAction.action.ReadValue<Vector2>();
            
            // NOTE: If using New Input System "Delta", values are in Pixels. 
            // We might want to normalize or clamp them to prevent huge jumps on high-DPI mice.
            // But for sway, raw pixel delta usually feels fine if swayAmount is small.
        }
        
        if (moveAction != null)
        {
            moveInput = moveAction.action.ReadValue<Vector2>();
        }
    }
}
