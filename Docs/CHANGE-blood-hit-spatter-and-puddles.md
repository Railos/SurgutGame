Date: 2026-01-14

# Change: Enemy hit spatter and puddles

## What changed and why
- Enemy hits now spawn two different effects:
  - Blood spatter spawns at the enemy position to show the impact.
  - Blood puddles spawn on nearby surfaces hit by downward raycasts to create ground decals.
- This separates impact feedback (spatter) from surface decals (puddles), giving clearer control of each effect.

## Usage / setup
1. Select an enemy prefab or instance with `Enemy`.
2. In the Inspector, set:
   - `Blood Spatter Prefab` to the hit effect spawned from the enemy (for example `Assets/BloodParticle.prefab`).
   - `Blood Puddle Prefab` to the decal or mesh that should appear on the floor (for example `Assets/BloodPuddle.prefab`).
   - `Blood Spatter Floor Layers` to the layers that should receive puddles.
3. Tune the puddle raycast settings:
   - `Blood Spatter Raycast Count` controls how many rays are fired per hit.
   - `Blood Spatter Raycast Radius` controls how far from the enemy center the rays start.
   - `Blood Spatter Raycast Angle Jitter` adds small random offsets so rays do not align perfectly.
4. Tune the spatter origin:
   - `Blood Spatter Spawn Height` controls how high above the enemy the spatter effect spawns.
5. If your puddle prefab is modeled with a non-`Y` up axis, set:
   - `Blood Puddle Local Up` to the axis that should align to the surface normal.
5. Play and damage the enemy. Each hit spawns spatter from the enemy and puddles where raycasts hit the floor layers.

Example values:
- `Blood Spatter Prefab`: `Assets/BloodParticle.prefab`
- `Blood Puddle Prefab`: `Assets/BloodPuddle.prefab`
- `Blood Spatter Raycast Count`: 3
- `Blood Spatter Raycast Radius`: 0.4
- `Blood Spatter Raycast Angle Jitter`: 20
- `Blood Spatter Raycast Height`: 1
- `Blood Spatter Raycast Distance`: 5
- `Blood Spatter Spawn Height`: 1

## Code-level notes
- `Enemy.SpawnBloodSpatterFromEnemy` instantiates the spatter prefab at the enemy position plus a height offset.
- `Enemy.SpawnBloodPuddles` casts angled raycasts from the spatter height toward points around the enemy, so puddles land around the enemy instead of directly underneath.
- `Enemy.SpawnBloodPuddles` uses `bloodPuddleLocalUp` to rotate the puddle so it lays correctly on walls or sloped surfaces.
- New serialized fields:
- `bloodPuddlePrefab`: puddle prefab for raycast hits.
- `bloodSpatterSpawnHeight`: vertical offset for the spatter spawn.
- `bloodPuddleLocalUp`: local up axis for puddle alignment (use `0,0,1` if the prefab is built facing forward).

## Risks / TODOs
- Multiple puddles per hit can increase decal count. If performance or clutter becomes an issue, lower `bloodSpatterRaycastCount` or increase `bloodSpatterRaycastAngleJitter`.
