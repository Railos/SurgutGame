Date: 2026-01-08

# Shotgun Damage Per Pellet

## Summary
The `damage` field on `Shotgun` now represents the damage per pellet instead of total damage split across pellets.

## Why
You asked to control single-pellet damage directly on the shotgun, using the existing `damage` field.

## What Changed
### `Assets/Scripts/Weapons/Shotgun.cs`
- Removed the damage distribution option and set each pellet to use `damage` directly.

## Usage / Setup
1. Set `damage` to the per-pellet value you want.
2. Adjust `pelletCount` to control total per-shot damage.

## Example
- `damage = 2`
- `pelletCount = 8`

This yields 16 total damage if all pellets hit.

## Risks / TODOs
- Total damage scales with pellet count; balance values accordingly.
