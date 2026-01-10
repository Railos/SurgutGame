using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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
    [SerializeField] float agentAcceleration = 12f;

    [Header("Attack")]
    [SerializeField] float attackRange = 1.75f;
    [SerializeField] float attackDamage = 10f;
    [SerializeField] float attackCooldown = 0.75f;
    [SerializeField] LayerMask attackLayers;
    [SerializeField] string attackTrigger = "Attack";

    NavMeshAgent agent;
    Animator animator;
    float nextAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.acceleration = agentAcceleration;
        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = false;
        agent.isStopped = true;
        animator = GetComponentInChildren<Animator>();
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

    private bool IsPlayerInAttackRange()
    {
        if (playerTarget == null)
        {
            return false;
        }

        Vector3 toPlayer = playerTarget.position - transform.position;
        float sqrDistance = toPlayer.sqrMagnitude;
        return sqrDistance <= attackRange * attackRange;
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;

        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
        {
            animator.SetTrigger(attackTrigger);
        }

        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = (playerTarget.position - origin);
        float distance = toPlayer.magnitude;
        if (distance <= 0.001f)
        {
            return;
        }

        Vector3 direction = toPlayer / distance;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange, attackLayers, QueryTriggerInteraction.Ignore))
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    private void FacePlayer()
    {
        if (playerTarget == null)
        {
            return;
        }

        Vector3 toPlayer = playerTarget.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void UpdateChaseDestination()
    {
        if (playerTarget == null || !agent.isOnNavMesh)
        {
            return;
        }

        agent.SetDestination(playerTarget.position);
    }

    private void StopAgent()
    {
        if (agent.isStopped)
        {
            return;
        }

        agent.isStopped = true;
        agent.ResetPath();
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
            owner.StopAgent();
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
                return;
            }

            if (owner.IsPlayerInAttackRange())
            {
                SwitchState(new AttackState(owner));
                return;
            }

            owner.UpdateChaseDestination();
            owner.FacePlayer();
        }

        protected override void OnEnter()
        {
            owner.agent.isStopped = false;
        }

        protected override void OnExit()
        {
            owner.StopAgent();
        }
    }

    private sealed class AttackState : EnemyState
    {
        private readonly SwarmerEnemy owner;

        public AttackState(SwarmerEnemy enemy) : base(enemy)
        {
            owner = enemy;
        }

        protected override void OnEnter()
        {
            owner.nextAttackTime = 0f;
            owner.StopAgent();
        }

        protected override void OnUpdate()
        {
            if (!owner.CanSeePlayer())
            {
                SwitchState(new IdleState(owner));
                return;
            }

            if (!owner.IsPlayerInAttackRange())
            {
                SwitchState(new ChaseState(owner));
                return;
            }

            owner.TryAttack();
            owner.FacePlayer();
        }
    }
}
