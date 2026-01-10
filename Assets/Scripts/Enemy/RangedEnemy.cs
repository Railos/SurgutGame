using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemy : Enemy
{
    [Header("Detection")]
    [SerializeField] float engageDistance = 18f;
    [SerializeField] float disengageDistance = 22f;
    [SerializeField] Transform playerTarget;

    [Header("Movement")]
    [SerializeField] float preferredDistance = 12f;
    [SerializeField] float distanceTolerance = 1.5f;
    [SerializeField] float turnSpeed = 540f;
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float agentAcceleration = 12f;
    [SerializeField] float agentStoppingDistance = 0f;
    [SerializeField] float navMeshSampleRadius = 2f;

    [Header("Shooting")]
    [SerializeField] float burstRange = 20f;
    [SerializeField] int shotsPerBurst = 3;
    [SerializeField] float timeBetweenShots = 0.12f;
    [SerializeField] float burstCooldown = 1.25f;
    [FormerlySerializedAs("shotDamage")]
    [SerializeField] float pelletDamage = 6f;
    [FormerlySerializedAs("shotLayers")]
    [SerializeField] LayerMask hitLayers = ~0;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireHeight = 1.3f;
    [SerializeField] GameObject bulletPrefab;
    [FormerlySerializedAs("bulletSpeed")]
    [SerializeField] float pelletSpeed = 18f;
    [FormerlySerializedAs("bulletLifetime")]
    [SerializeField] float pelletLifetime = 1.2f;
    [FormerlySerializedAs("bulletImpactForce")]
    [SerializeField] float impactForce = 6f;

    NavMeshAgent agent;
    float nextShotTime;
    float nextBurstTime;
    int shotsRemainingInBurst;

    IdleState idleState;
    CombatState combatState;
    RepositionState repositionState;
    BurstShootState burstShootState;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.acceleration = agentAcceleration;
        agent.stoppingDistance = agentStoppingDistance;
        agent.updateRotation = false;
        agent.isStopped = true;

        TryResolvePlayer();

        idleState = new IdleState(this);
        combatState = new CombatState(this);
        repositionState = new RepositionState(this);
        burstShootState = new BurstShootState(this);
    }

    protected override EnemyState CreateRootState()
    {
        return idleState;
    }

    void TryResolvePlayer()
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

    bool IsPlayerInEngageRange()
    {
        return GetSqrDistanceToPlayer() <= engageDistance * engageDistance;
    }

    bool ShouldDisengage()
    {
        return GetSqrDistanceToPlayer() > disengageDistance * disengageDistance;
    }

    bool ShouldReposition()
    {
        float sqrDistance = GetSqrDistanceToPlayer();
        float minDistance = Mathf.Max(0f, preferredDistance - distanceTolerance);
        float maxDistance = preferredDistance + distanceTolerance;
        return sqrDistance < minDistance * minDistance || sqrDistance > maxDistance * maxDistance;
    }

    bool IsPlayerInBurstRange()
    {
        return GetSqrDistanceToPlayer() <= burstRange * burstRange;
    }

    float GetSqrDistanceToPlayer()
    {
        if (playerTarget == null)
        {
            return float.MaxValue;
        }

        Vector3 toPlayer = playerTarget.position - transform.position;
        return toPlayer.sqrMagnitude;
    }

    void FacePlayer()
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

    void StopAgent()
    {
        if (agent.isStopped)
        {
            return;
        }

        agent.isStopped = true;
        agent.ResetPath();
    }

    void UpdateRepositionDestination()
    {
        if (playerTarget == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        Vector3 awayFromPlayer = transform.position - playerTarget.position;
        awayFromPlayer.y = 0f;
        if (awayFromPlayer.sqrMagnitude < 0.0001f)
        {
            awayFromPlayer = -transform.forward;
        }

        Vector3 desiredPosition = playerTarget.position + awayFromPlayer.normalized * preferredDistance;
        desiredPosition.y = transform.position.y;

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, navMeshSampleRadius, agent.areaMask))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(desiredPosition);
        }
    }

    void ResetBurst()
    {
        shotsRemainingInBurst = 0;
        nextShotTime = 0f;
        nextBurstTime = 0f;
    }

    void TryBurstShoot()
    {
        if (playerTarget == null)
        {
            return;
        }

        if (!IsPlayerInBurstRange())
        {
            return;
        }

        if (Time.time < nextShotTime)
        {
            return;
        }

        if (shotsRemainingInBurst <= 0)
        {
            if (Time.time < nextBurstTime)
            {
                return;
            }

            shotsRemainingInBurst = Mathf.Max(1, shotsPerBurst);
        }

        FireShot();
        shotsRemainingInBurst--;
        nextShotTime = Time.time + timeBetweenShots;

        if (shotsRemainingInBurst <= 0)
        {
            nextBurstTime = Time.time + burstCooldown;
        }
    }

    void FireShot()
    {
        if (bulletPrefab == null)
        {
            return;
        }

        Vector3 origin = firePoint != null ? firePoint.position : transform.position + Vector3.up * fireHeight;
        Vector3 targetPosition = playerTarget.position;
        Vector3 toTarget = targetPosition - origin;
        float distance = toTarget.magnitude;
        if (distance <= 0.001f)
        {
            return;
        }

        Vector3 direction = toTarget / distance;
        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(direction, Vector3.up));
        PelletProjectile projectile = bullet.GetComponent<PelletProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(pelletDamage, hitLayers, impactForce, pelletLifetime);
            projectile.SetVelocity(direction * pelletSpeed);
            return;
        }

        Rigidbody bulletBody = bullet.GetComponent<Rigidbody>();
        if (bulletBody != null)
        {
            bulletBody.linearVelocity = direction * pelletSpeed;
        }
    }


    private sealed class IdleState : EnemyState
    {
        private readonly RangedEnemy owner;

        public IdleState(RangedEnemy enemy) : base(enemy)
        {
            owner = enemy;
        }

        protected override void OnEnter()
        {
            owner.TryResolvePlayer();
            owner.StopAgent();
            owner.ResetBurst();
        }

        protected override void OnUpdate()
        {
            owner.TryResolvePlayer();
            if (owner.IsPlayerInEngageRange())
            {
                SwitchState(owner.combatState);
            }
        }
    }

    private sealed class CombatState : EnemyState
    {
        private readonly RangedEnemy owner;

        public CombatState(RangedEnemy enemy) : base(enemy)
        {
            owner = enemy;
        }

        protected override void OnEnter()
        {
            owner.TryResolvePlayer();
            SetSubState(owner.burstShootState);
        }

        protected override void OnUpdate()
        {
            if (owner.playerTarget == null)
            {
                owner.TryResolvePlayer();
            }

            if (owner.ShouldDisengage())
            {
                SwitchState(owner.idleState);
                return;
            }

            if (owner.ShouldReposition())
            {
                SetSubState(owner.repositionState);
            }
            else
            {
                SetSubState(owner.burstShootState);
            }

            owner.FacePlayer();
        }
    }

    private sealed class RepositionState : EnemyState
    {
        private readonly RangedEnemy owner;

        public RepositionState(RangedEnemy enemy) : base(enemy)
        {
            owner = enemy;
        }

        protected override void OnEnter()
        {
            owner.agent.isStopped = false;
        }

        protected override void OnExit()
        {
            owner.StopAgent();
        }

        protected override void OnUpdate()
        {
            owner.UpdateRepositionDestination();
            owner.FacePlayer();
            owner.TryBurstShoot();
        }
    }

    private sealed class BurstShootState : EnemyState
    {
        private readonly RangedEnemy owner;

        public BurstShootState(RangedEnemy enemy) : base(enemy)
        {
            owner = enemy;
        }

        protected override void OnEnter()
        {
            owner.StopAgent();
        }

        protected override void OnUpdate()
        {
            owner.TryBurstShoot();
        }
    }
}
