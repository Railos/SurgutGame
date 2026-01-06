Date: 2026-01-06

# Swarmer Enemy (HFSM)

## What changed and why
- Added `SwarmerEnemy`, a simple HFSM-driven enemy that idles until the player is visible, then runs directly toward them.
- Uses the new `Enemy` HFSM base to keep detection and chase logic in clear, modular states.

## Usage / setup
- Add `SwarmerEnemy` to an enemy GameObject with a `Rigidbody`.
- Assign `playerTarget` in the Inspector, or tag the player GameObject as `Player` so the swarmer can auto-resolve it on start.
- Set `visionBlockers` to layers that should block line of sight (e.g., walls).
- Tune `viewDistance`, `viewAngle`, `moveSpeed`, and `stoppingDistance` to match gameplay feel.
- To reduce wall/crate sticking, set `avoidanceLayers` and tweak `avoidanceDistance`, `avoidanceRadius`, and `avoidanceStrength`.

## Examples
### Minimal setup checklist
```text
Enemy GameObject
  - Rigidbody
  - SwarmerEnemy
Player GameObject
  - Tag: Player
```

### Example layer setup
```text
visionBlockers = Environment, Default
avoidanceLayers = Environment, Default
```

## Risks / TODOs
- Avoidance is a lightweight steer, not true pathfinding; enemies can still get stuck on complex geometry.
- If the player tag is missing and `playerTarget` is not assigned, the swarmer will remain idle.
