Date: 2026-01-10

# Pellet Projectile Lifetime Scheduling

## What changed and why
- Fixed pellet lifetime scheduling so a custom lifetime passed in `Initialize(...)` actually takes effect.
- Previously, `OnEnable()` scheduled destruction using the default lifetime before `Initialize(...)` ran, causing bullets to destroy early even when a longer lifetime was set.

## Usage / setup
- No setup changes. Continue calling `Initialize(pelletDamage, hitLayers, impactForce, pelletLifetime)` on spawned bullets.

## Examples
### Example 1: Long-lived pellet
```csharp
projectile.Initialize(6f, hitLayers, 6f, 3f);
```
- Result: the bullet now lasts ~3 seconds instead of destroying at the default 0.6 seconds.

### Example 2: Immediate cleanup
```csharp
projectile.Initialize(6f, hitLayers, 6f, 0f);
```
- Result: no timed destroy is scheduled, so the bullet persists until it collides or is destroyed by other logic.

## Implementation details (methods and fields)
### Method: `Initialize(...)`
- Updates `lifetime` from the passed value.
- Cancels any previously scheduled `DestroySelf()` invoke from `OnEnable()`.
- Schedules a new `DestroySelf()` invoke using the updated lifetime.

## Risks / TODOs
- If `lifetime` is set to 0, the projectile will persist unless destroyed by collision.
