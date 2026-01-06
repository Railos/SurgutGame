using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SwarmerEnemy : Enemy
{
    [Header("Detection")]
    [SerializeField] float viewDistance = 15f;
    [SerializeField, Range(0f, 180f)] float viewAngle = 120f;
    [SerializeField] float eyeHeight = 1.2f;
    [SerializeField] LayerMask visionBlockers;
    [SerializeField] Transform playerTarget;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float stoppingDistance = 1.5f;
    [SerializeField] float turnSpeed = 360f;
    [SerializeField] float avoidanceDistance = 2f;
    [SerializeField] float avoidanceRadius = 0.35f;
    [SerializeField, Range(0f, 1f)] float avoidanceStrength = 0.65f;
    [SerializeField] LayerMask avoidanceLayers;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        TryResolvePlayer();
    }

    protected override EnemyState CreateRootState()
    {
        return new IdleState(this);
    }

    private void TryResolvePlayer()
    {
        if (playerTarget != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
    }

    private bool CanSeePlayer()
    {
        if (playerTarget == null)
        {
            return false;
        }

        Vector3 toPlayer = playerTarget.position - transform.position;
        float sqrDistance = toPlayer.sqrMagnitude;
        if (sqrDistance > viewDistance * viewDistance)
        {
            return false;
        }

        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (flatToPlayer.sqrMagnitude > 0.0001f)
        {
            float cosHalfFov = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
            float dot = Vector3.Dot(transform.forward, flatToPlayer.normalized);
            if (dot < cosHalfFov)
            {
                return false;
            }
        }

        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 direction = toPlayer.normalized;
        float distance = Mathf.Sqrt(sqrDistance);
        if (Physics.Raycast(origin, direction, distance, visionBlockers, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return true;
    }

    private void MoveTowardsPlayer()
    {
        if (playerTarget == null)
        {
            return;
        }

        Vector3 toPlayer = playerTarget.position - transform.position;
        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
        float sqrDistance = flatToPlayer.sqrMagnitude;
        if (sqrDistance <= stoppingDistance * stoppingDistance)
        {
            return;
        }

        Vector3 direction = flatToPlayer.normalized;
        direction = ApplyAvoidance(direction);
        Vector3 nextPosition = transform.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
    }

    private Vector3 ApplyAvoidance(Vector3 direction)
    {
        if (avoidanceDistance <= 0f || avoidanceRadius <= 0f)
        {
            return direction;
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.SphereCast(origin, avoidanceRadius, direction, out RaycastHit hit, avoidanceDistance, avoidanceLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 steer = Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized;
            if (steer.sqrMagnitude > 0.0001f)
            {
                Vector3 blended = Vector3.Slerp(direction, steer, avoidanceStrength);
                return blended.normalized;
            }
        }

        return direction;
    }

    private sealed class IdleState : EnemyState
    {
        private readonly SwarmerEnemy owner;

        public IdleState(SwarmerEnemy enemy) : base(enemy)
        {
            owner = enemy;
        }

        protected override void OnEnter()
        {
            owner.TryResolvePlayer();
        }

        protected override void OnUpdate()
        {
            if (owner.CanSeePlayer())
            {
                SwitchState(new ChaseState(owner));
            }
        }
    }

    private sealed class ChaseState : EnemyState
    {
        private readonly SwarmerEnemy owner;

        public ChaseState(SwarmerEnemy enemy) : base(enemy)
        {
            owner = enemy;
        }

        protected override void OnUpdate()
        {
            if (!owner.CanSeePlayer())
            {
                SwitchState(new IdleState(owner));
            }
        }

        protected override void OnFixedUpdate()
        {
            owner.MoveTowardsPlayer();
        }
    }
}
