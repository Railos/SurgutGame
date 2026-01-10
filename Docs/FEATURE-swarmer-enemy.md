Date: 2026-01-06

# Swarmer Enemy (HFSM)

## What changed and why
- Added `SwarmerEnemy`, a simple HFSM-driven enemy that idles until the player is visible, then runs directly toward them.
- Uses the new `Enemy` HFSM base to keep detection, chase, and attack logic in clear, modular states.
- Movement now uses `NavMeshAgent` so the swarmer can navigate around obstacles.

## Usage / setup
- Add `SwarmerEnemy` and `NavMeshAgent` to an enemy GameObject.
- Assign `playerTarget` in the Inspector, or tag the player GameObject as `Player` so the swarmer can auto-resolve it on start.
- Set `visionBlockers` to layers that should block line of sight (e.g., walls).
- Set `attackLayers` to the layers that contain the player collider.
- Tune `viewDistance`, `viewAngle`, `moveSpeed`, `stoppingDistance`, and the attack values to match gameplay feel.

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
attackLayers = Player
```

## Risks / TODOs
- NavMesh coverage must exist where the swarmer moves; otherwise it will not navigate.
- Attack raycast uses layer filtering; ensure the player collider is on an included layer.
- If the player tag is missing and `playerTarget` is not assigned, the swarmer will remain idle.
