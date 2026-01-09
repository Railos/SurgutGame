Date: 2026-01-08

# Shotgun Projectile Pellets

## Summary
Converted the `Shotgun` weapon to fire physical pellet GameObjects instead of hitscan rays to better match a powerful, tactile shotgun feel.

## Why
Physical pellets allow visible projectiles, collision-based impacts, and more authentic cone-shaped spread compared to instant ray hits. This also enables easier tuning of travel speed and lifetime.

## What Changed
### `Assets/Scripts/Weapons/Shotgun.cs`
- Replaced hitscan raycasts with pellet prefab instantiation.
- Added tuning fields for pellet prefab, muzzle, speed, and lifetime.
- Spread now uses a circular cone shape via `Random.insideUnitCircle`.

### `Assets/Scripts/Weapons/PelletProjectile.cs`
- New projectile script to handle lifetime, collisions, and damage/force application.

## Usage / Setup
1. Create a pellet prefab with a `Rigidbody`, `Collider`, and `PelletProjectile` component.
2. Assign the prefab to `pelletPrefab` on the `Shotgun` component.
3. (Optional) Assign a `muzzle` transform to control spawn position.
4. Set `pelletSpeed` and `pelletLifetime` to control travel distance and feel.

## Example Setup
Pellet prefab settings:
- `Rigidbody`: dynamic, use gravity as desired.
- `Collider`: sphere or capsule collider.
- `PelletProjectile`: keep defaults; values are overridden by `Shotgun` via `PelletProjectile.Initialize`.

Shotgun settings:
- `pelletCount = 10`
- `pelletSpeed = 50`
- `pelletLifetime = 0.5`
- `spreadAngle = 8`

## Risks / TODOs
- Instantiating many pellets per shot can create GC pressure without pooling; consider a simple projectile pool if performance dips.
- If the pellet prefab is missing required components, pellets will not behave correctly; validate prefab setup in the editor.
