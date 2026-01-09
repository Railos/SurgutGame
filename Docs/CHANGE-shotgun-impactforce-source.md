Date: 2026-01-08

# Shotgun Impact Force Ownership

## Summary
Moved impact force tuning to the `Shotgun` only so pellet prefabs no longer carry their own impact force settings.

## Why
Impact force should be consistent and driven by the weapon, not the projectile prefab, to avoid mismatched behavior across different pellet assets.

## What Changed
### `Assets/Scripts/Weapons/PelletProjectile.cs`
- Removed the serialized `impactForce` field so it is configured only via `Shotgun` at runtime.

## Usage / Setup
1. Set `impactForce` on the `Shotgun` component to control knockback.
2. Pellet prefabs no longer need an impact force value.

## Example
- `impactForce = 6`

## Risks / TODOs
- None.
