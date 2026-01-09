Date: 2026-01-08

# Shotgun Owns All Pellet Settings

## Summary
Moved all pellet tuning off the prefab so the `Shotgun` component fully controls pellet behavior at runtime.

## Why
This keeps weapon feel consistent and prevents hidden prefab values from conflicting with shotgun tuning.

## What Changed
### `Assets/Scripts/Weapons/PelletProjectile.cs`
- Removed serialized fields for `lifetime`, `damage`, and `hitLayers`. These are now set by `Shotgun.Initialize`.

## Usage / Setup
1. Configure `damage`, `pelletLifetime`, `impactForce`, and `hitLayers` on the `Shotgun`.
2. Pellet prefabs no longer need any tuning values.

## Example
- `pelletLifetime = 0.6`
- `impactForce = 6`

## Risks / TODOs
- None.
