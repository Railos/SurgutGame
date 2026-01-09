Date: 2026-01-08

# Feature: Fist Punch Melee

## What changed and why
- Added a new `FistPunch` MonoBehaviour to play a fist punch animation and apply a single forward melee hit on button press.
- The script is designed to be deterministic and low cost by using one ray/sphere cast per punch press and avoiding per-frame allocations.

## Usage and setup
1) Add `FistPunch` to the player or the weapon/fist GameObject.
2) Assign `Punch Action` with the Input System action you want to use for punching.
3) Assign `Fist Animator` and set `Punch Trigger` to the trigger name used in your Animator (default is `Punch`).
4) Assign `Cam` or `Punch Origin`:
   - Use `Punch Origin` if you have a specific transform on the fist.
   - Use `Cam` if you want the punch to follow the camera forward.
5) Set `Range`, `Radius`, `Damage`, `Impact Force`, `Hit Layers`, and `Cooldown` to match your game balance.

Example setup values:
- `Range`: 2.0
- `Radius`: 0.15 (set to 0 for a pure raycast)
- `Damage`: 20
- `Cooldown`: 0.4
- `Hit Layers`: Enemy layers only

## Script details
### Class: `FistPunch`
Handles a single forward melee hit and triggers a fist animation when the punch input is pressed.

### Methods
- `Start()`: Enables the punch input action so it can receive button presses.
- `Update()`: Checks for a punch press and cooldown, then performs the punch.
- `IsPunchPressed()`: Returns true when the assigned input action was triggered this frame.
- `ExecutePunch()`: Plays the animation and performs the hit check.
- `PlayPunchAnimation()`: Triggers the Animator parameter defined by `Punch Trigger`.
- `TryHitTarget()`: Performs a raycast or sphere cast forward and applies damage and impact force to the first valid hit.
- `GetPunchOrigin()`: Chooses `Punch Origin` first, then `Cam`, then the current transform for the cast start.

## Risks / TODOs
- If `Punch Action` is not assigned, the punch will never trigger.
- If `Fist Animator` or `Punch Trigger` is not set correctly, the punch animation will not play.
- Consider adding hit effects or audio if you want stronger feedback.
