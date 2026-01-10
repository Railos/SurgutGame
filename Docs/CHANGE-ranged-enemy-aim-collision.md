Date: 2026-01-10

# Ranged Enemy Aim

## What changed and why
- Removed `aimHeight` so bullets now travel directly toward the player's current position for simpler, predictable aiming.

## Usage / setup
- No setup required for aim; bullets now target the player's transform position directly.

## Examples
### Example 1: Direct aim
- Enemy fires from `Fire Point` or `fireHeight` directly at the player's transform position.
- Result: shots are straight-line to the player without vertical offsets.

## Implementation details (methods and fields)
### Removed field
- `aimHeight`: no longer used; aiming now targets `playerTarget.position`.

### Method: `FireShot()`
- Uses `playerTarget.position` as the target point without a height offset.

## Risks / TODOs
- None.
