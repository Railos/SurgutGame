using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} died!");
        Destroy(gameObject); 
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
