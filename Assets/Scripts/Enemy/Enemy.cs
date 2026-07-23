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
    [SerializeField] float bloodSpatterSpawnHeight = 1f;
    [SerializeField] LayerMask bloodPuddleFloorLayers = ~0;
    [SerializeField] float bloodPuddleRaycastHeight = 1f;
    [SerializeField] float bloodPuddleRaycastDistance = 5f;
    [SerializeField] float bloodPuddleSurfaceOffset = 0.01f;

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
            SpawnBloodSpatter();
            SpawnBloodPuddle();
        }

        if (currentHealth <= 0)
        {
            if (spawnBloodOnDeath)
            {
                SpawnBloodSpatter();
                SpawnBloodPuddle();
            }

            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} died!");
        Destroy(gameObject); 
    }

    private void SpawnBloodPuddle()
    {
        if (bloodPuddlePrefab == null)
        {
            return;
        }

        Vector3 origin = transform.position + Vector3.up * bloodPuddleRaycastHeight;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, bloodPuddleRaycastDistance, bloodPuddleFloorLayers, QueryTriggerInteraction.Ignore))
        {
            Quaternion baseRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), hit.normal);
            Vector3 spawnPosition = hit.point + hit.normal * bloodPuddleSurfaceOffset;
            GameObject spatter = Instantiate(bloodPuddlePrefab, spawnPosition, randomRotation * baseRotation);

            BloodSpatter spatterComponent = spatter.GetComponent<BloodSpatter>();
            if (spatterComponent != null)
            {
                spatterComponent.SetSurfaceNormal(hit.normal);
            }
        }
    }

    private void SpawnBloodSpatter()
    {
        if (bloodSpatterPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * bloodSpatterSpawnHeight;
        Vector2 planar = Random.insideUnitCircle;
        if (planar.sqrMagnitude < 0.0001f)
        {
            planar = Vector2.up;
        }

        Vector3 direction = new Vector3(planar.x, 0f, planar.y).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(direction, Vector3.up);
        Instantiate(bloodSpatterPrefab, spawnPosition, spawnRotation);
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
