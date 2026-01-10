Date: 2026-01-10

# Ranged Enemy (NavMesh + Burst HFSM)

## What changed and why
- Added `RangedEnemy` as a new `Enemy`-derived HFSM enemy that holds position by default, engages when the player is close enough, and uses burst shooting while maintaining a preferred distance.
- Implemented distance-based engagement/disengagement to keep behavior predictable and reduce unnecessary movement when the player is not nearby.
- Integrated `NavMeshAgent`-driven repositioning so the enemy can back off or step in to maintain a safe firing distance.

## Usage / setup
- Add `RangedEnemy` to an enemy prefab and ensure a `NavMeshAgent` component is present (the script requires it).
- Bake a NavMesh for the level so repositioning has valid paths.
- Assign `Player Target` if you want a direct reference; otherwise the script auto-finds the object tagged `Player`.
- Configure `Shot Layers` to include the player and anything that should block shots (walls, props).
- Tune `Engage Distance`, `Disengage Distance`, and `Preferred Distance` to control when the enemy starts fighting and how far it tries to stand from the player.

## Examples
### Example 1: Basic stand-and-shoot setup
- `Engage Distance = 15`
- `Disengage Distance = 20`
- `Preferred Distance = 12`
- `Shots Per Burst = 3`
- `Time Between Shots = 0.12`
- Result: the enemy stays idle until the player is within 15 units, then fires bursts while trying to stay about 12 units away.

### Example 2: Aggressive kiting
- `Preferred Distance = 18`
- `Distance Tolerance = 1`
- `Burst Range = 22`
- Result: the enemy backs away earlier and keeps a larger buffer while still firing from range.

## Implementation details (classes and methods)
### Class: `RangedEnemy`
- `Awake()`: Caches the `NavMeshAgent`, applies movement settings, disables agent rotation for manual facing, tries to resolve the player target, and instantiates HFSM state objects once to avoid per-frame allocations.
- `CreateRootState()`: Returns the cached `IdleState` as the HFSM root state.
- `TryResolvePlayer()`: Assigns `playerTarget` using the `Player` tag if no target is set.
- `IsPlayerInEngageRange()`: Checks if the player is close enough to enter combat.
- `ShouldDisengage()`: Checks if the player is far enough to return to idle.
- `ShouldReposition()`: Checks if the player is outside the preferred distance band (too close or too far).
- `IsPlayerInBurstRange()`: Checks if the player is within shooting distance.
- `GetSqrDistanceToPlayer()`: Returns the squared distance to the player, or `float.MaxValue` if no target exists.
- `FacePlayer()`: Rotates the enemy toward the player on the Y axis using `turnSpeed`.
- `StopAgent()`: Stops and clears the `NavMeshAgent` path so the enemy stays still.
- `UpdateRepositionDestination()`: Computes a desired position at `preferredDistance` from the player, samples the NavMesh, and sets the agent destination.
- `ResetBurst()`: Clears burst timers and shot counters to restart the burst cadence cleanly.
- `TryBurstShoot()`: Manages burst timing, cooldown, and fires individual shots when allowed.
- `FireShot()`: Raycasts toward the player from `firePoint` (or a height offset) and applies damage to the first `IDamageable` hit.

### State: `IdleState`
- `OnEnter()`: Stops movement, resets burst timers, and resolves the player target.
- `OnUpdate()`: Transitions to `CombatState` when the player enters engage range.

### State: `CombatState`
- `OnEnter()`: Resolves the player target and initializes the first sub-state (`BurstShootState`).
- `OnUpdate()`: Returns to `IdleState` if the player disengages, otherwise selects `RepositionState` or `BurstShootState` based on distance and faces the player.

### State: `RepositionState`
- `OnEnter()`: Enables the `NavMeshAgent` movement.
- `OnExit()`: Stops the agent to avoid drifting.
- `OnUpdate()`: Continuously updates the destination to maintain the preferred distance from the player.

### State: `BurstShootState`
- `OnEnter()`: Stops movement to shoot from a stable position.
- `OnUpdate()`: Handles burst firing cadence via `TryBurstShoot()`.

## Risks / TODOs
- If the enemy is spawned off the NavMesh, repositioning will not work until it is placed on a valid NavMesh surface.
- Shooting uses hitscan raycasts; if you need projectiles or recoil, add them in `FireShot()`.
- Ensure `shotLayers` includes the player or the enemy will never deal damage.
