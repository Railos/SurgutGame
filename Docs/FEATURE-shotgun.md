Date: 2026-01-08

# Shotgun Weapon

## Summary
Added a new `Shotgun` weapon that inherits `BulletWeapon` and fires multiple pellet GameObjects per shot with adjustable spread, speed, lifetime, damage distribution, and impact force.

## Why
The shotgun provides a powerful close-range option by delivering multiple pellet hits in a single trigger pull while staying consistent with the existing weapon base class.

## Script: `Assets/Scripts/Weapons/Shotgun.cs`

### Class: `Shotgun`
Inherits from `BulletWeapon` and uses the `Weapon` base fields (`damage`, `cam`, `hitLayers`, `recoilScript`, `waitUntilNextShot`, `shootAction`, `holdToShoot`).

#### Serialized fields
- `pelletCount`: Number of pellets fired per shot. Higher values increase coverage and total projectile count.
- `pelletPrefab`: Prefab instantiated per pellet. Should include a `Rigidbody`, `Collider`, and `PelletProjectile` component.
- `muzzle`: Optional transform used as the spawn point. Falls back to the camera position if not set.
- `pelletSpeed`: Initial velocity applied to each pellet rigidbody.
- `pelletLifetime`: Time in seconds before pellets are destroyed, effectively limiting range.
- `spreadAngle`: Degrees of random cone spread applied to each pellet direction.
- `impactForce`: Impulse applied to rigidbodies hit by pellets.
- `ignoreColliders`: Optional colliders (player/body/weapon) that pellets should ignore to prevent immediate self-collisions.

#### Methods
- `Shoot()`: Overrides the base weapon fire. It:
  - Validates that `cam` and `pelletPrefab` are assigned.
  - Calculates pellet count and per-pellet damage.
  - Instantiates `pelletCount` pellet prefabs, each with a randomized cone direction.
  - Initializes `PelletProjectile` data and sets rigidbody velocity.
  - Calls `base.Shoot()` to trigger recoil and cooldown timing.
- `GetPelletDirection()`: Builds a direction vector by sampling a point inside a circle and using it as yaw/pitch offsets around the camera forward axis for a circular cone spread.

## Script: `Assets/Scripts/Weapons/PelletProjectile.cs`

### Class: `PelletProjectile`
Handles pellet lifetime, collision, and damage application.

#### Runtime fields (set by `Shotgun`)
- `lifetime`: Time before the pellet destroys itself.
- `damage`: Damage applied on collision when using `Initialize`.
- `hitLayers`: Layers that can receive damage/force.

#### Methods
- `Initialize(float pelletDamage, LayerMask layers, float force, float lifeTime)`: Configures damage, layers, impact force, and optional lifetime override per pellet.
- `SetVelocity(Vector3 velocity)`: Applies the initial velocity to the pellet rigidbody.
- `OnCollisionEnter(Collision collision)`: Applies damage/force to valid layers and destroys the pellet.

## Usage / Setup
1. Add the `Shotgun` component to your weapon GameObject.
2. Assign `cam` (player camera), `shootAction`, `hitLayers`, and optionally `recoilScript`.
3. Create a pellet prefab with a `Rigidbody`, `Collider`, and `PelletProjectile` component, then assign it to `pelletPrefab`.
4. Optionally assign `muzzle` to control spawn position.
5. Set `damage`, `waitUntilNextShot`, `pelletCount`, `pelletSpeed`, `pelletLifetime`, `spreadAngle`, and `impactForce` to tune the feel.
4. If you want total damage to remain stable as pellet count changes, keep `distributeDamageAcrossPellets` enabled.

## Example Settings
Example close-range setup:
- `damage = 1.5`
- `pelletCount = 8`
- `pelletSpeed = 45`
- `pelletLifetime = 0.6`
- `spreadAngle = 7`
- `impactForce = 7`
- `distributeDamageAcrossPellets = true`

With these values, each pellet deals `1.5` damage. Total damage per shot scales by `pelletCount`. The approximate range is `pelletSpeed * pelletLifetime`.

## Risks / TODOs
- Multiple pellets can add several impulses to the same rigidbody in one shot; tune `impactForce` if this feels too strong.
- The pellet spread uses `Random.insideUnitCircle`, so shot patterns are non-deterministic; consider a seeded random or fixed pattern if you need strict determinism.
