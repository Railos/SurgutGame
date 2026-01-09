Date: 2026-01-08

# Dual Berettas (Alternate Fire)

## What changed and why
- Added `DualBerettas`, a hitscan pistol variant that alternates left/right recoil each time a shot is actually fired.
- This keeps the core hitscan behavior while enabling "left-right-left-right" feel for dual pistols.

## Usage / setup
1) Create a GameObject for the dual pistols and add the `DualBerettas` component.
2) Assign the same inputs and camera you already use for `HitscanWeapon`.
3) (Optional) Assign `Left Recoil` and `Right Recoil` to separate recoil pivots on each pistol.
4) Set `Start With Left` to choose which pistol fires first.

### Example setup (Inspector)
- `Shoot Action`: your shoot input action
- `Cam`: player camera
- `Damage`: 10
- `Wait Until Next Shot`: 0.2
- `Hit Layers`: Enemy, Environment
- `Left Recoil`: left pistol recoil component
- `Right Recoil`: right pistol recoil component
- `Start With Left`: true

## Class and method details
### `DualBerettas` (class)
Inherits from `HitscanWeapon` and only changes which recoil source is used per shot. The hitscan ray, damage application, and cooldown logic stay in `HitscanWeapon` and `Weapon`.

Fields:
- `Left Recoil`: recoil component used on left-pistol shots.
- `Right Recoil`: recoil component used on right-pistol shots.
- `Start With Left`: determines which pistol fires first on the initial shot.

### `Awake()`
Initializes the internal alternating state using `Start With Left` so the first shot uses the expected pistol.

### `Shoot()`
Chooses the next recoil component (left or right), temporarily swaps it into `recoilScript`, calls the base hitscan shot logic, then restores the previous recoil reference.

## Risks / TODOs
- If `Left Recoil` or `Right Recoil` is not assigned, the script falls back to the existing `recoilScript` on the weapon.
- No separate muzzle origin or visual effects per pistol are handled here; if you need alternating muzzle flashes or different shot origins, add those in a follow-up change.
