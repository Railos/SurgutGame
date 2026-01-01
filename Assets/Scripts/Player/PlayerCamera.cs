using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Input References")]
    public InputActionReference lookAction; // Bind to "Delta" (Mouse) or "Look" (Stick)

    [Header("References")]
    public Transform cameraObject;
    public Transform orientation;
    public Transform cameraHolder;

    [Header("Settings")]
    public float sensX = 0.5f; // Lower default sens for New Input Delta
    public float sensY = 0.5f;

    float xRotation;
    float yRotation;
    float tiltRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraObject.parent != null)
            cameraObject.parent = null;

        if (lookAction != null) lookAction.action.Enable();
    }

    void LateUpdate()
    {
        // New Input System "Delta" is already frame-rate independent (pixels moved)
        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

        float mouseX = mouseDelta.x * sensX; // No Time.deltaTime needed for Delta
        float mouseY = mouseDelta.y * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraObject.rotation = Quaternion.Euler(xRotation, yRotation, tiltRotation);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        cameraHolder.rotation = Quaternion.Euler(xRotation, yRotation, tiltRotation);

        cameraObject.position = cameraHolder.position;
    }
    
    public void SetTilt(float tilt)
    {
        tiltRotation = tilt;
    }

    public float GetTilt()
    {
        return tiltRotation;
    }
}
