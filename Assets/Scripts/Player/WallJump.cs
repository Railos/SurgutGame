using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WallJump : MonoBehaviour
{
    [Header("Wall Detection")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallCheckDistance = 0.7f;
    [SerializeField] private float wallCheckHeightOffset = 0.9f;
    [SerializeField] private bool useForwardWallChecks = true;

    [Header("Wall Stick / Slide")]
    [SerializeField] private float wallStickTime = 0.2f;
    [SerializeField] private float wallSlideSpeed = 2.5f;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpForce = 12f;
    [SerializeField] private float wallJumpUpForce = 8f;
    [SerializeField] private float wallJumpPushDuration = 0.08f;
    [SerializeField] private float wallJumpCooldown = 0.2f;

    [Header("Ground Fallback")]
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private LayerMask groundMask;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform wallCheckTransform;

    private Rigidbody rb;
    private float wallStickTimer;
    private float lastWallJumpTime;
    private bool isWallSliding;
    private Vector3 lastWallNormal;
    private float wallJumpPushTimer;
    private Vector3 wallJumpVelocityPerSecond;

    public bool IsWallSliding => isWallSliding;
    public Vector3 LastWallNormal => lastWallNormal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        if (wallCheckTransform == null)
        {
            wallCheckTransform = playerController != null && playerController.orientation != null
                ? playerController.orientation
                : transform;
        }
    }

    private void FixedUpdate()
    {
        if (wallJumpPushTimer > 0f)
        {
            float step = Mathf.Min(Time.fixedDeltaTime, wallJumpPushTimer);
            rb.AddForce(wallJumpVelocityPerSecond * step, ForceMode.VelocityChange);
            wallJumpPushTimer -= step;
        }

        if (IsGrounded())
        {
            isWallSliding = false;
            wallStickTimer = 0f;
            return;
        }

        if (Time.time < lastWallJumpTime + wallJumpCooldown)
        {
            isWallSliding = false;
            wallStickTimer = 0f;
            return;
        }

        if (CheckWall(out Vector3 wallNormal))
        {
            lastWallNormal = wallNormal;

            if (!isWallSliding)
            {
                wallStickTimer = wallStickTime;
            }

            isWallSliding = true;

            if (wallStickTimer > 0f)
            {
                wallStickTimer -= Time.fixedDeltaTime;
                if (rb.linearVelocity.y < 0f)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                }
            }
            else if (rb.linearVelocity.y < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallSlideSpeed, rb.linearVelocity.z);
            }
        }
        else
        {
            isWallSliding = false;
            wallStickTimer = 0f;
        }
    }

    public bool TryWallJump()
    {
        if (Time.time < lastWallJumpTime + wallJumpCooldown)
        {
            return false;
        }

        if (IsGrounded())
        {
            return false;
        }

        if (!CheckWall(out Vector3 wallNormal))
        {
            return false;
        }

        lastWallJumpTime = Time.time;
        isWallSliding = false;
        wallStickTimer = 0f;
        lastWallNormal = wallNormal;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 jumpVelocity = wallNormal * wallJumpForce + Vector3.up * wallJumpUpForce;
        if (wallJumpPushDuration <= 0f)
        {
            rb.AddForce(jumpVelocity, ForceMode.Impulse);
            wallJumpPushTimer = 0f;
        }
        else
        {
            wallJumpPushTimer = wallJumpPushDuration;
            wallJumpVelocityPerSecond = jumpVelocity / wallJumpPushDuration;
        }
        return true;
    }

    private bool IsGrounded()
    {
        if (playerController != null)
        {
            return playerController.isGrounded;
        }

        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(
            origin,
            Vector3.down,
            groundCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore);
    }

    private bool CheckWall(out Vector3 wallNormal)
    {
        Vector3 origin = transform.position + Vector3.up * wallCheckHeightOffset;
        Transform checkTransform = wallCheckTransform != null ? wallCheckTransform : transform;
        if (Physics.Raycast(
                origin,
                checkTransform.right,
                out RaycastHit hit,
                wallCheckDistance,
                wallMask,
                QueryTriggerInteraction.Ignore))
        {
            wallNormal = hit.normal;
            return true;
        }

        if (Physics.Raycast(
                origin,
                -checkTransform.right,
                out hit,
                wallCheckDistance,
                wallMask,
                QueryTriggerInteraction.Ignore))
        {
            wallNormal = hit.normal;
            return true;
        }

        if (useForwardWallChecks)
        {
            if (Physics.Raycast(
                    origin,
                    checkTransform.forward,
                    out hit,
                    wallCheckDistance,
                    wallMask,
                    QueryTriggerInteraction.Ignore))
            {
                wallNormal = hit.normal;
                return true;
            }

            if (Physics.Raycast(
                    origin,
                    -checkTransform.forward,
                    out hit,
                    wallCheckDistance,
                    wallMask,
                    QueryTriggerInteraction.Ignore))
            {
                wallNormal = hit.normal;
                return true;
            }
        }

        wallNormal = Vector3.zero;
        return false;
    }
}
