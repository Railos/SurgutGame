Date: 2026-01-09

# Feature: Player Implements IDamageable

## What Changed And Why
Added a new `Player` component that implements `IDamageable` so weapons and other damage sources can apply damage to the player through the shared interface. This keeps damage handling deterministic and centralized, and it mirrors how `Enemy` already works.

## Usage / Setup
1. Add `Player` to the player GameObject (the same object you want to receive damage).
2. Set `Max Health` and (optionally) `Destroy On Death` in the Inspector.
3. Ensure any hit detection scripts (for example `FistPunch`, `HitscanWeapon`, or `PelletProjectile`) can reach the player's collider. These scripts already look for `IDamageable`, so no extra wiring is needed beyond making sure the Player has a collider and is on an appropriate layer.

## Examples
Example: Apply damage from any script that already found an `IDamageable`:

```csharp
IDamageable target = hit.transform.GetComponent<IDamageable>();
if (target != null)
{
    target.TakeDamage(25f);
}
```

## Class And Method Details
### `Player` (class)
`Player` is a `MonoBehaviour` that implements `IDamageable`. It owns current health, tracks a simple dead/alive state, and optionally destroys the GameObject when health reaches zero.

### `Awake()` (method)
Initializes the player's current health to the configured `maxHealth` so the player starts with full health when the scene loads.

### `TakeDamage(float amount)` (method)
Implements `IDamageable.TakeDamage`. It ignores non-positive damage and ignores further damage after death. Otherwise it subtracts from current health and triggers `Die()` when health reaches zero or below.

### `Die()` (method)
Marks the player as dead and optionally destroys the GameObject when `destroyOnDeath` is enabled. This provides a minimal, deterministic death behavior that can be extended later.

### `CurrentHealth`, `MaxHealth`, `IsDead` (properties)
Read-only accessors so other systems can query the player's health state without modifying it directly.

## Risks / TODOs
- No UI or feedback is included yet; if you need a health bar or death effects, add them in a separate change.
- `Destroy On Death` is optional; if you want respawn logic instead of destruction, add a dedicated respawn system and call it from `Die()`.
