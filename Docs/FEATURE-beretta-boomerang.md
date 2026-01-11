Date: 2026-01-11

# Dual Berettas Alt-Fire Boomerang Throw

## Summary
Added an alternate fire (RMB) that throws the left or right beretta as a boomerang. The thrown pistol cannot be used for shooting until it returns, so the player can temporarily have only one (or zero) berettas available.

## Why
This creates a distinct secondary attack for the dual pistols, adds risk/reward timing, and preserves deterministic, low-allocation behavior during combat.

## Files Changed
- `Assets/Scripts/Weapons/DualBerettas.cs`
- `Assets/Scripts/Weapons/BerettaBoomerang.cs`
- `Assets/Controls.inputactions`

## Usage / Setup
1) In `Assets/Controls.inputactions`, assign the new `AltFire` action to the Dual Berettas (InputActionReference in the inspector).
2) Create a boomerang prefab:
   - Add a `Rigidbody` (set to kinematic by script) and a `Collider` (set to trigger by script).
   - Add `BerettaBoomerang` and any mesh/visuals.
3) In the Dual Berettas inspector:
   - Assign `Boomerang Prefab`.
   - Assign `Left Throw Origin` and `Right Throw Origin` (muzzle points on each pistol).
   - Assign `Return Target` (weapon root or player hand transform).
   - Assign optional `Left Beretta Visual` and `Right Beretta Visual` to hide the gun while it is thrown.
   - Tune speeds, distances, and damage as needed.

## Example
Example Dual Berettas setup:
- `Alt Fire Action`: Controls/Player/AltFire (RMB).
- `Boomerang Prefab`: BerettaBoomerangPrefab.
- `Left Throw Origin`: LeftMuzzle.
- `Right Throw Origin`: RightMuzzle.
- `Return Target`: PlayerCamera (or weapon root).
- `Throw Speed`: 18, `Return Speed`: 22, `Throw Max Distance`: 12.

## Method and Class Notes

### `BerettaBoomerang` (new class)
- `Awake`: Caches the Rigidbody/Collider and forces kinematic + trigger so the boomerang is safe and deterministic.
- `Initialize(...)`: Accepts the owner, side (left/right), return target, movement tuning, damage, and hit layers, then initializes travel state for this throw.
- `FixedUpdate`: Moves the boomerang outward and then back to the return target, switches to return when max distance or lifetime is reached, and triggers a catch when within `catchDistance`.
- `OnTriggerEnter`: Applies damage and impulse to valid targets based on `hitLayers`, ignoring the owner hierarchy and trigger-only colliders.
- `MoveTo`: Moves using `Rigidbody.MovePosition` when possible, or falls back to setting `transform.position`.
- `NotifyOwner`: Calls back to `DualBerettas` once so the weapon knows the boomerang is back.
- `DestroySelf`: Ensures the owner is notified before the object is destroyed.
- `OnDestroy`: Final safety net to notify the owner if the object disappears early.

### `DualBerettas`
- `OnEnable` / `OnDisable`: Enables and disables the alternate fire input action when the weapon is active.
- `Update`: Processes alt-fire before the normal shooting flow so availability updates are respected in the same frame.
- `Shoot`: Uses the next available pistol (left/right), skipping any thrown side. If neither is available, it does nothing.
- `OnBoomerangReturned`: Marks the corresponding pistol as available again and re-enables its visual.
- `HandleAltFire`: Early exit unless the alt-fire input is triggered, then throws.
- `IsAltFireTriggered`: Reads the alt-fire action safely.
- `TryThrowBoomerang`: Spawns a boomerang from the chosen side, initializes it, and hides the thrown pistol.
- `SelectLeftForShot`: Chooses which pistol can shoot next based on availability.
- `SelectLeftForThrow`: Chooses which pistol should be thrown next based on availability.
- `SetVisualActive`: Safely toggles pistol visuals when thrown/returned.

## Risks / TODOs
- The boomerang currently uses simple trigger-based hits; if more accurate hit feedback is needed, consider a dedicated hitbox or trail test.
- The new `AltFire` action was added only for mouse RMB; add gamepad bindings if needed.
- The throw uses instantiate/destroy; add pooling if object churn becomes a performance issue.
