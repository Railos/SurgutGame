Date: 2026-01-10

# Swarmer NavMesh Movement

## What changed and why
- Swapped swarmer movement from manual Rigidbody steering to a `NavMeshAgent` so it can navigate around obstacles.
- The chase state now updates an agent destination each frame and uses agent avoidance to reduce wall/crate sticking.

## Usage / setup
- Ensure the swarmer GameObject has a `NavMeshAgent` component (required by the script).
- Bake a NavMesh for the level and ensure the swarmer is spawned on it.
- Tune `moveSpeed`, `agentAcceleration`, and `stoppingDistance` on the swarmer to control chase feel.

## Risks / TODOs
- If the swarmer is not on a NavMesh, it will not move or path around obstacles.
