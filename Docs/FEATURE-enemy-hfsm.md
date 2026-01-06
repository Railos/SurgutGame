Date: 2026-01-06

# Enemy HFSM Base

## What changed and why
- Added a hierarchical finite state machine (HFSM) base to `Enemy` so derived enemies can define modular state trees with parent/child states. This replaces scattered behavior flags with structured state classes.
- Centralized update routing (`Update`, `FixedUpdate`, `LateUpdate`) through the HFSM so frame-time behavior is deterministic and tied to the active state tree.
- Introduced explicit state switching (`SetSubState` for children, `SwitchState` for sibling/root) to keep transitions local and easy to reason about.

## Usage / setup
- Derive from `Enemy` and implement `CreateRootState()` to return your initial root state.
- Create custom states by subclassing `Enemy.EnemyState` and overriding `OnEnter`, `OnExit`, `OnUpdate`, `OnFixedUpdate`, `OnLateUpdate` as needed.
- Use `SetSubState(...)` inside a parent state to swap child states.
- Call `SwitchState(...)` inside a state to transition to a sibling or new root state.

## Examples
### Example 1: Simple root state
```csharp
public class GruntEnemy : Enemy
{
    protected override EnemyState CreateRootState()
    {
        return new IdleState(this);
    }

    private sealed class IdleState : EnemyState
    {
        public IdleState(Enemy enemy) : base(enemy) { }

        protected override void OnEnter()
        {
            // Play idle animation, reset timers, etc.
        }

        protected override void OnUpdate()
        {
            // Transition to chase when the player is detected.
            // SwitchState(new ChaseState(enemy));
        }
    }
}
```

### Example 2: Hierarchy with a super state
```csharp
private sealed class CombatState : EnemyState
{
    public CombatState(Enemy enemy) : base(enemy) { }

    protected override void OnEnter()
    {
        SetSubState(new PatrolState(enemy));
    }
}

private sealed class PatrolState : EnemyState
{
    public PatrolState(Enemy enemy) : base(enemy) { }

    protected override void OnUpdate()
    {
        // If player seen, swap the child state inside Combat.
        // GetSuperState().SetSubState(new AttackState(enemy));
    }
}

private sealed class AttackState : EnemyState
{
    public AttackState(Enemy enemy) : base(enemy) { }

    protected override void OnUpdate()
    {
        // If player lost, return to patrol.
        // GetSuperState().SetSubState(new PatrolState(enemy));
    }
}
```

### Example 3: Switching to a new root state
```csharp
private sealed class DeadState : EnemyState
{
    public DeadState(Enemy enemy) : base(enemy) { }

    protected override void OnEnter()
    {
        // Disable movement, play death animation.
    }
}

// From any state:
// SwitchState(new DeadState(enemy));
```

## Risks / TODOs
- Ensure custom states avoid allocations in hot paths (no LINQ, no per-frame `new`).
- If a derived `Enemy` defines its own `Update`/`FixedUpdate`/`LateUpdate`, it must call `base` so the HFSM continues to tick.
- If you need global transitions (e.g., stun/knockback) across any hierarchy, consider a dedicated top-level state to own those transitions.
