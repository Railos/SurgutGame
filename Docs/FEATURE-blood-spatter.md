Date: 2026-01-07

# Blood Spatter Objects

This change adds a simple, reusable way for enemies to drop blood spatter objects on the floor every time they are hit and again when they die (optional). The spatter is spawned by raycasting down from the enemy, so it always lands on the ground below.

## What Changed and Why

- `Assets/Scripts/Enemy/Enemy.cs` now supports spawning a blood spatter prefab on hits and/or on death so combat feedback leaves visible marks on the floor.
- `Assets/Scripts/Enemy/BloodSpatter.cs` provides a minimal component for blood spatter objects, including a hook for a future blood mob to “enter” (consume) a spatter without the spatter itself spawning the mob.

These changes keep the logic simple and deterministic while avoiding extra work in frame hot paths.

## Usage / Setup

1) Create a blood spatter prefab (for example, a small quad or decal with a blood material).
2) Add the `BloodSpatter` component to that prefab.
3) Assign the prefab to each enemy in the `bloodSpatterPrefab` field.
4) Set `bloodSpatterFloorLayers` to the layer(s) you consider “floor.”

Optional tuning:
- `spawnBloodOnHit`: enable/disable blood per hit.
- `spawnBloodOnDeath`: enable/disable the extra spawn when the enemy dies.
- `bloodSpatterRaycastHeight`, `bloodSpatterRaycastDistance`: control the downward ray origin and distance.
- `bloodSpatterSurfaceOffset`: pushes the spatter slightly above the surface to avoid z-fighting.

## Simple Examples

Example: assign the prefab in the inspector and use default layers.

Example: a blood mob that “enters” a spatter when it reaches it:

```csharp
BloodSpatter spatter = targetSpatter.GetComponent<BloodSpatter>();
if (spatter != null)
{
    spatter.TryEnter(transform);
}
```

## New/Changed Code Details

### `Enemy` (Assets/Scripts/Enemy/Enemy.cs)

- `spawnBloodOnHit` / `spawnBloodOnDeath`: toggles for when blood spatter is spawned.
- `bloodSpatterPrefab`: the prefab that will be instantiated as the spatter object.
- `bloodSpatterFloorLayers`: layer mask used by the downward raycast to find the ground.
- `bloodSpatterRaycastHeight` / `bloodSpatterRaycastDistance`: controls the raycast used to place spatter below the enemy.
- `bloodSpatterSurfaceOffset`: small vertical offset so the spatter sits cleanly on the surface.
- `SpawnBloodSpatter()`: performs the raycast and instantiates the prefab with a rotation aligned to the surface normal and a random yaw.

`TakeDamage()` now calls `SpawnBloodSpatter()` on every hit (when enabled) and again when the enemy dies (when enabled).

### `BloodSpatter` (Assets/Scripts/Enemy/BloodSpatter.cs)

- `SurfaceNormal`: stored normal of the surface the spatter sits on.
- `SetSurfaceNormal(Vector3 normal)`: assigns the surface normal (ignores zero-length input).
- `TryEnter(Transform mobTransform)`: moves a mob to the spatter position and optionally destroys the spatter; returns `true` on success.
- `destroyOnEnter`: if enabled, the spatter is destroyed when a mob enters it.

## Risks / TODOs

- Spatter count can grow if many enemies are hit; consider pooling or capping if performance becomes an issue.
- If the floor layers are not configured, the spatter will not spawn (raycast miss).
- If you want different spatter size or visuals per enemy type, make separate prefabs or add size overrides.
