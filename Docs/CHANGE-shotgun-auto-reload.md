Date: 2026-01-12

# Shotgun Auto-Reload Two-Shell Limit

## What Changed and Why
- The shotgun now has a fixed maximum of 2 shells and will not fire if empty. This matches the requested behavior and keeps shooting predictable.
- The shotgun automatically reloads one shell at a time after a short delay, so ammo refills without manual input.

## Usage / Setup
- In the Shotgun component, set `Reload Interval` to the delay (in seconds) between auto-loaded shells.
- No manual reload input is required; simply wait for shells to refill.

## Examples
- If `Reload Interval = 0.6`:
  - Fire twice: ammo goes from 2 to 0.
  - After 0.6s, ammo becomes 1.
  - After 1.2s total, ammo becomes 2.

## New/Changed Code Details
- `MaxAmmo` (const int): Fixed max shells for the shotgun (2) so it can never exceed two shells.
- `reloadInterval` (float, serialized): Time between automatic shell loads; set in the inspector to tune how fast shells return.
- `currentAmmo` (int): Runtime-only ammo count that starts full and is reduced on each shot.
- `reloadTimer` (float): Accumulates time between shell loads to enforce the per-shell delay.
- `Awake()`: Initializes `currentAmmo` to full and clears the reload timer so the shotgun starts ready.
- `Update()`: Calls base weapon input/shot logic, then advances auto-reload timing each frame.
- `CheckShoot()`: Prevents firing when ammo is empty by adding an ammo check before calling `Shoot()`.
- `Shoot()`: Spends one shell per shot, resets the reload timer, then fires pellets and applies cooldown.
- `AutoReload(float deltaTime)`: Handles one-by-one shell recovery; it advances the timer and adds one shell per interval until full.

## Risks / TODOs
- Risk: Very small `Reload Interval` values make the shotgun effectively always full, which could change balance.
- TODO: Expose current ammo to UI if the player needs visible feedback.
