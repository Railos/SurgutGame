using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

    [Header("Blood")]
    [SerializeField] GameObject bloodSpatterPrefab;
    [SerializeField] GameObject bloodPuddlePrefab;
    [SerializeField] bool spawnBloodOnHit = true;
    [SerializeField] bool spawnBloodOnDeath = true;
    [SerializeField] LayerMask bloodSpatterFloorLayers = ~0;
    [SerializeField] float bloodSpatterRaycastHeight = 1f;
    [SerializeField] float bloodSpatterRaycastDistance = 5f;
    [SerializeField] int bloodSpatterRaycastCount = 3;
    [SerializeField] float bloodSpatterRaycastRadius = 0.5f;
    [SerializeField] float bloodSpatterRaycastAngleJitter = 15f;
    [SerializeField] float bloodSpatterSpawnHeight = 1f;
    [SerializeField] float bloodSpatterSurfaceOffset = 0.01f;
    [SerializeField] Vector3 bloodPuddleLocalUp = Vector3.up;

    protected abstract class EnemyState
    {
        protected readonly Enemy enemy;
        private EnemyState superState;
        private EnemyState subState;

        protected EnemyState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public void Enter()
        {
            OnEnter();
            subState?.Enter();
        }

        public void Exit()
        {
            subState?.Exit();
            OnExit();
        }

        public void Update()
        {
            OnUpdate();
            subState?.Update();
        }

        public void FixedUpdate()
        {
            OnFixedUpdate();
            subState?.FixedUpdate();
        }

        public void LateUpdate()
        {
            OnLateUpdate();
            subState?.LateUpdate();
        }

        public EnemyState GetSuperState()
        {
            return superState;
        }

        public EnemyState GetSubState()
        {
            return subState;
        }

        public EnemyState GetLeafState()
        {
            return subState == null ? this : subState.GetLeafState();
        }

        public void SetSubState(EnemyState newSubState)
        {
            if (newSubState == subState)
            {
                return;
            }

            subState?.Exit();
            subState = newSubState;
            if (subState != null)
            {
                subState.superState = this;
                subState.Enter();
            }
        }

        protected void SwitchState(EnemyState newState)
        {
            enemy.SwitchState(newState);
        }

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnLateUpdate() { }
    }

    private EnemyState rootState;
    private EnemyState currentState;

    void Start()
    {
        currentHealth = maxHealth;
        InitializeStateMachine();
    }

    void Update()
    {
        rootState?.Update();
    }

    void FixedUpdate()
    {
        rootState?.FixedUpdate();
    }

    void LateUpdate()
    {
        rootState?.LateUpdate();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (spawnBloodOnHit)
        {
            SpawnBloodSpatterFromEnemy();
            SpawnBloodPuddles();
        }

        if (currentHealth <= 0)
        {
            if (spawnBloodOnDeath)
            {
                SpawnBloodSpatterFromEnemy();
                SpawnBloodPuddles();
            }

            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} died!");
        Destroy(gameObject); 
    }

    private void SpawnBloodSpatterFromEnemy()
    {
        if (bloodSpatterPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * bloodSpatterSpawnHeight;
        Quaternion spawnRotation = Random.rotation;
        Instantiate(bloodSpatterPrefab, spawnPosition, spawnRotation);
    }

    private void SpawnBloodPuddles()
    {
        if (bloodPuddlePrefab == null)
        {
            return;
        }

        int rayCount = Mathf.Max(1, bloodSpatterRaycastCount);
        float angleStep = 360f / rayCount;
        Vector3 baseOrigin = transform.position + Vector3.up * bloodSpatterSpawnHeight;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (angleStep * i) + Random.Range(-bloodSpatterRaycastAngleJitter, bloodSpatterRaycastAngleJitter);
            float radians = angle * Mathf.Deg2Rad;
            Vector3 planarOffset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * bloodSpatterRaycastRadius;
            Vector3 targetPoint = transform.position + planarOffset + Vector3.down * bloodSpatterRaycastDistance;
            Vector3 direction = targetPoint - baseOrigin;
            float maxDistance = direction.magnitude;

            if (Physics.Raycast(baseOrigin, direction.normalized, out RaycastHit hit, maxDistance, bloodSpatterFloorLayers, QueryTriggerInteraction.Ignore))
            {
                Vector3 localUp = bloodPuddleLocalUp.sqrMagnitude < 0.0001f ? Vector3.up : bloodPuddleLocalUp.normalized;
                Quaternion baseRotation = Quaternion.FromToRotation(localUp, hit.normal);
                Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), hit.normal);
                Vector3 spawnPosition = hit.point + hit.normal * bloodSpatterSurfaceOffset;
                GameObject spatter = Instantiate(bloodPuddlePrefab, spawnPosition, randomRotation * baseRotation);

                BloodSpatter spatterComponent = spatter.GetComponent<BloodSpatter>();
                if (spatterComponent != null)
                {
                    spatterComponent.SetSurfaceNormal(hit.normal);
                }
            }
        }
    }

    protected virtual EnemyState CreateRootState()
    {
        return null;
    }

    protected void SetRootState(EnemyState newRootState)
    {
        if (newRootState == rootState)
        {
            return;
        }

        rootState?.Exit();
        rootState = newRootState;

        if (rootState != null)
        {
            rootState.Enter();
            currentState = rootState.GetLeafState();
        }
        else
        {
            currentState = null;
        }
    }

    private void InitializeStateMachine()
    {
        SetRootState(CreateRootState());
    }

    void SwitchState(EnemyState newState)
    {
        if (newState == null)
        {
            return;
        }

        if (currentState == null)
        {
            SetRootState(newState);
            return;
        }

        EnemyState parent = currentState.GetSuperState();
        if (parent != null)
        {
            parent.SetSubState(newState);
            currentState = newState.GetLeafState();
        }
        else
        {
            SetRootState(newState);
        }
    }
}
