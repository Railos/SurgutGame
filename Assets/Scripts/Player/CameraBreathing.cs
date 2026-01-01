using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBreathing : MonoBehaviour
{
    [Header("Input References")]
    public InputActionReference moveAction;

    [Header("References")]
    public Transform cameraHolder; // The CameraPos object
    public PlayerCamera playerCam; // The PlayerCamera script
    public PlayerController controller; // NEW: Reference to check IsGrounded

    [Header("Figure-8 Headbob")]
    public float bobAmplitude = 0.005f; // Keep small for "snappy" feel
    public float bobFrequency = 12f;    // 10-14 is natural walking speed
    
    [Header("Strafe Tilt")]
    public float tiltAmount = 5f;
    public float tiltSpeed = 10f;

    Vector3 startPos;
    float timer;

    void Start()
    {
        startPos = cameraHolder.localPosition;
    }

    void Update()
    {
        if (moveAction == null) return;
        
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        
        // Pass 'true' if input exists AND player is grounded
        bool isMoving = input.magnitude > 0.1f && (controller == null || controller.IsGrounded());
        
        HandleHeadBob(isMoving);
        HandleStrafeTilt(input);
    }

    void HandleHeadBob(bool isMoving)
    {
        if (isMoving)
        {
            // Increment timer based on speed
            timer += Time.deltaTime * bobFrequency;

            // FIGURE-8 MOTION (Lissajous)
            // X: Cos(time/2) - sways left/right once per stride
            // Y: Sin(time)   - bobs up/down twice per stride (per step)
            float xOffset = Mathf.Cos(timer / 2) * bobAmplitude * 2f; 
            float yOffset = Mathf.Sin(timer) * bobAmplitude;

            // Apply offsets to the initial position
            Vector3 targetPos = new Vector3(startPos.x + xOffset, startPos.y + yOffset, startPos.z);
            
            // Direct assignment is snappy
            cameraHolder.localPosition = targetPos; 
        }
        else
        {
            // Reset timer to keep the phase consistent or reset it? 
            // Resetting timer to closest PI ensures we start on the correct foot, 
            // but for simple snappy feel, just lerp back to start.
            timer = Mathf.PI / 2; // Reset phase so we start at the top of the step next time
            
            Vector3 resetPos = new Vector3(startPos.x, startPos.y, startPos.z);
            // Snappy return (High lerp speed)
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, resetPos, Time.deltaTime * 15f);
        }
    }

    void HandleStrafeTilt(Vector2 input)
    {
        // ... (Same logic as before, kept untouched as requested) ...
        float targetTilt = 0f;
        if (input.x > 0) targetTilt = -tiltAmount;
        else if (input.x < 0) targetTilt = tiltAmount;

        playerCam.SetTilt(Mathf.Lerp(playerCam.GetTilt(), targetTilt, Time.deltaTime * tiltSpeed));
    }
}
