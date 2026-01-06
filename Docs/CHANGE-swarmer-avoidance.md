Date: 2026-01-07

# Swarmer Avoidance Steering

## What changed and why
- Added a lightweight obstacle-avoidance steer to `SwarmerEnemy` so it can slide around walls and crates while chasing.
- Uses a forward spherecast to detect nearby blockers and blends the chase direction with a planar steer.

## Usage / setup
- Set `avoidanceLayers` to the layers you want the swarmer to steer around.
- Tune `avoidanceDistance` to match how far ahead the swarmer should check.
- Tune `avoidanceRadius` to match the swarmer’s body size.
- Adjust `avoidanceStrength` (0 to 1) to control how aggressively it steers.

## Risks / TODOs
- This is not full pathfinding; if a target is fully enclosed, the swarmer can still get stuck.
- If `avoidanceLayers` is empty, the steer will never trigger.
