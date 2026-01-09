Date: 2026-01-09

# Swarmer Melee Attack

## What changed and why
- Added a melee attack state to `SwarmerEnemy` so it can damage the player when close.
- The attack triggers an animation (when an `Animator` is present) and performs a short-range raycast to apply damage.

## Usage / setup
- Assign `attackLayers` to include the player collider layer.
- Set `attackRange`, `attackDamage`, and `attackCooldown` to tune feel and balance.
- If using an animator, add a trigger parameter matching `attackTrigger` (default: `Attack`).

## Risks / TODOs
- This is a simple raycast hit; if the player is outside the ray despite being close, the attack may miss.
- Animation and damage are not synchronized to hit frames; add animation events if you need precise timing.
