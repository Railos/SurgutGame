Date: 2026-01-14

# Change: Single blood puddle under enemy on hit

## What changed and why
- Enemy hits now spawn a single blood puddle on the floor directly beneath the enemy using a downward raycast.
- A blood spatter effect is also spawned from the enemy with a random rotation to add impact feedback.

## Usage / setup
1. Select an enemy prefab or instance with `Enemy`.
2. In the Inspector, set:
   - `Blood Spatter Prefab` to the hit effect spawned at the enemy (for example `Assets/BloodParticle.prefab`).
   - `Blood Puddle Prefab` to the decal or mesh that should appear on the floor (for example `Assets/BloodPuddle.prefab`).
   - `Blood Puddle Floor Layers` to the layers that should receive puddles.
3. Tune the raycast:
   - `Blood Puddle Raycast Height` controls where the ray starts above the enemy.
   - `Blood Puddle Raycast Distance` controls how far the ray checks downward.
4. Tune the spatter origin:
   - `Blood Spatter Spawn Height` controls how high above the enemy the spatter effect spawns.
4. Play and damage the enemy. Each hit spawns one puddle where the ray hits the floor.

Example values:
- `Blood Spatter Prefab`: `Assets/BloodParticle.prefab`
- `Blood Puddle Prefab`: `Assets/BloodPuddle.prefab`
- `Blood Spatter Spawn Height`: 1
- `Blood Puddle Raycast Height`: 1
- `Blood Puddle Raycast Distance`: 5

## Code-level notes
- `Enemy.SpawnBloodPuddle` performs one downward raycast and instantiates the puddle prefab at the hit point.
- The puddle is aligned to the surface normal and offset slightly using `bloodPuddleSurfaceOffset`.
- `Enemy.SpawnBloodSpatter` instantiates the spatter prefab with a random horizontal direction so it does not point upward.
- Serialized fields used by the change:
  - `bloodSpatterPrefab`: spatter prefab for hits.
  - `bloodPuddlePrefab`: puddle prefab for hits.
  - `bloodPuddleFloorLayers`: layers that can receive puddles.
  - `bloodPuddleRaycastHeight`: ray origin height.
  - `bloodPuddleRaycastDistance`: ray length.
  - `bloodPuddleSurfaceOffset`: small offset to avoid z-fighting.
  - `bloodSpatterSpawnHeight`: spatter origin height.

## Risks / TODOs
- Only floors directly below the enemy will receive puddles. If you need wall splats again, we can reintroduce angled raycasts.
