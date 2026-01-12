Date: 2026-01-12

# Shotgun Alt-Fire Double Blast

## What Changed and Why
- Added an alternative fire mode on RMB that consumes both loaded shells at once, giving the shotgun a high-impact option when it is fully loaded.
- Alt-fire increases pellet damage by 1.25x and applies backward knockback to emphasize the heavier blast.

## Usage / Setup
- Assign the `Alt Fire Action` reference on the Shotgun component to the `AltFire` input action (RMB by default in `Controls.inputactions`).
- (Optional) Assign `Player Rigidbody` on the Shotgun component so knockback is applied to the player body.
- Tune `Alt Fire Damage Multiplier` and `Alt Fire Knockback Force` to balance the alternate mode.

## Examples
- If `Alt Fire Damage Multiplier = 1.25` and `Alt Fire Knockback Force = 8`:
  - With 2 shells loaded, RMB fires two shell volleys at once.
  - Each pellet deals 1.25x normal damage.
  - The player is pushed backward from the shot direction by an impulse of 8.
- If only 1 shell is loaded, RMB does nothing until ammo refills to 2.

## New/Changed Code Details
- `altFireAction` (InputActionReference): Input action for alt-fire (RMB). Checked each frame for a triggered press.
- `altFireDamageMultiplier` (float): Multiplier applied to pellet damage during alt-fire.
- `altFireKnockbackForce` (float): Impulse force applied opposite the shot direction when alt-fire fires.
- `playerRigidbody` (Rigidbody): Cached player body used for knockback. Defaults to parent Rigidbody if not assigned.
- `OnEnable()/OnDisable()`: Enables/disables `altFireAction` so it stays active with the weapon.
- `HandleAltFire()`: Validates input, cooldown, and ammo count, then performs the double blast.
- `FirePelletVolleys(int volleys, float damageMultiplier)`: Shared helper that spawns the requested number of pellet volleys with a damage multiplier.
- `ApplyAltFireKnockback()`: Applies the backward impulse using the camera forward direction.

## Risks / TODOs
- Risk: High knockback values can interfere with movement control or launch the player in tight spaces.
- TODO: Hook current ammo and alt-fire readiness into the HUD for clearer feedback.
