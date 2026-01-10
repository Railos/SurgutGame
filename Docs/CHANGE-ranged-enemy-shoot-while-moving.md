Date: 2026-01-10

# Ranged Enemy Shooting While Repositioning

## What changed and why
- Allowed `RangedEnemy` to keep firing burst shots while repositioning so it can retreat or advance without losing offensive pressure.
- Added facing during movement to keep shots directed toward the player.

## Usage / setup
- No setup changes required. The enemy will now shoot during the reposition state when the player is in range.

## Examples
### Example 1: Retreat and fire
- Player gets too close, enemy backs away to `preferredDistance`.
- Result: the enemy continues firing bursts while moving away.

### Example 2: Step-in and fire
- Player is too far, enemy moves closer to `preferredDistance`.
- Result: the enemy keeps firing as it closes distance.

## Implementation details (methods and states)
### State: `RepositionState`
- `OnUpdate()` now calls `FacePlayer()` and `TryBurstShoot()` after updating the NavMesh destination.

## Risks / TODOs
- Continuous firing while moving may feel more aggressive; tune `burstCooldown` or `shotsPerBurst` if needed.
