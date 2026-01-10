Date: 2026-01-10

# Ranged Enemy Bullets

## What changed and why
- Swapped `RangedEnemy` shooting from hitscan raycasts to spawning real bullet prefabs so combat visuals and collisions match gameplay expectations.
- Added bullet configuration fields to control projectile speed, lifetime, and impact force without changing code.

## Usage / setup
- Assign a `Bullet Prefab` to `RangedEnemy`. The prefab should include a `Rigidbody` and `Collider`.
- If the prefab uses `PelletProjectile`, it will receive damage, layer, impact force, and lifetime from the enemy.
- Make sure `Shot Layers` includes the player and any other valid targets the bullet should affect.
- Optional: assign `Fire Point` for the muzzle position; otherwise the enemy uses a height offset.

## Examples
### Example 1: Using an existing pellet prefab
- `Bullet Prefab = PelletPrefab`
- `Bullet Speed = 18`
- `Bullet Lifetime = 1.2`
- Result: the enemy spawns pellets that fly forward and apply `shotDamage` on impact.

### Example 2: Using a custom rigidbody bullet
- Prefab only has `Rigidbody + Collider` (no `PelletProjectile`).
- Result: the enemy still launches the bullet with velocity, but damage must be handled by the bullet script on the prefab.

## Implementation details (methods and fields)
### Fields added to `RangedEnemy`
- `bulletPrefab`: GameObject to spawn per shot.
- `bulletSpeed`: Initial velocity for the bullet.
- `bulletLifetime`: Lifetime passed into `PelletProjectile` (if present).
- `bulletImpactForce`: Impact force passed into `PelletProjectile` (if present).

### Method: `FireShot()`
- Validates `bulletPrefab` is set before firing.
- Instantiates the prefab facing the player direction.
- If `PelletProjectile` exists, initializes damage/layers/force/lifetime and sets velocity.
- If no `PelletProjectile` exists, assigns velocity directly to the `Rigidbody`.

## Risks / TODOs
- If `bulletPrefab` is missing, the enemy will not fire.
- If the prefab lacks a `Rigidbody`, bullets will not move unless handled by a custom script.
