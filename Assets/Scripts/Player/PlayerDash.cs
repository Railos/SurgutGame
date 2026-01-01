using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference dashAction;
    public InputActionReference moveAction; // ADDED: To know where to dash

    [Header("Dash Settings")]
    public float dashForce = 40f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 1.5f;

    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;
    private PlayerController pm;

    private bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerController>();
        
        if (dashAction != null) dashAction.action.Enable();
        // moveAction is usually enabled by PlayerController, but safe to check
    }

    void Update()
    {
        if (dashAction.action.WasPressedThisFrame() && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        pm.isDashing = true; 

        // 1. Calculate Direction (Based on WASD)
        Vector3 dir = GetDashDirection();

        // 2. Snappy Impulse 
        // We Zero out X/Z velocity so the dash changes direction INSTANTLY 
        // (even if you were moving left and dash right)
        rb.linearVelocity = new Vector3(0f, 0f, 0f); // Reset momentum for the snappy "Pop"
        
        // Apply Dash Force
        rb.AddForce(dir * dashForce, ForceMode.Impulse);

        // 3. Visual Feedback
        StartCoroutine(DoFovKick(90f, 0.1f));

        yield return new WaitForSeconds(dashDuration);

        // 5. End Dash (Speed limit returns gradually via PlayerController Lerp)
        pm.isDashing = false; 

        StartCoroutine(DoFovKick(70f, 0.25f)); 

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private Vector3 GetDashDirection()
    {
        // 1. Read WASD Input
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // 2. Calculate Direction relative to looking direction
        Vector3 dir = orientation.forward * input.y + orientation.right * input.x;

        // 3. If standing still, dash Forward by default
        if (dir.sqrMagnitude == 0)
            return orientation.forward;

        return dir.normalized;
    }

    private IEnumerator DoFovKick(float targetFov, float duration)
    {
        float startFov = Camera.main.fieldOfView;
        float time = 0;
        while (time < duration)
        {
            Camera.main.fieldOfView = Mathf.Lerp(startFov, targetFov, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        Camera.main.fieldOfView = targetFov;
    }
}
