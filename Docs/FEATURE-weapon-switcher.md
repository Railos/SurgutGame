Date: 2026-01-08

# Weapon Switcher (2-slot)

## What changed and why
Added a `WeaponSwitcher` component that toggles between two weapon GameObjects using input actions. This keeps switching logic centralized and predictable, so only one weapon is active at a time.

## Usage / setup
1) Create an empty GameObject (or use the player root) and add `WeaponSwitcher`.
2) Assign `Select Slot 1 Action` and `Select Slot 2 Action` to your input actions (ex: "Weapon1" and "Weapon2").
3) Populate `Weapon Objects` with two weapon GameObjects:
   - Index 0: Dual Berettas
   - Index 1: Shotgun
4) Set `Start Index` to the weapon you want active at spawn (0 or 1).

## Example
If `Weapon Objects` is `[DualBerettasGO, ShotgunGO]` and:
- `Select Slot 1 Action` is bound to key `1`
- `Select Slot 2 Action` is bound to key `2`
Then pressing `1` activates Dual Berettas and deactivates Shotgun; pressing `2` does the opposite.

## New class and members
`WeaponSwitcher` (MonoBehaviour)
- Fields:
  - `selectSlot1Action`: Input action reference for slot 1 (ex: Dual Berettas).
  - `selectSlot2Action`: Input action reference for slot 2 (ex: Shotgun).
  - `weaponObjects`: Array of weapon GameObjects to toggle. Index 0 is slot 1, index 1 is slot 2.
  - `startIndex`: Which weapon index is active on start.
  - `currentIndex`: Runtime tracking of the currently active weapon.
- Methods:
  - `OnEnable()`: Enables the input actions so they can trigger weapon switching.
  - `Start()`: Validates weapon list and forces a clean initial active state for the chosen start index.
  - `Update()`: Checks for input action triggers and switches to the corresponding slot.
  - `OnDisable()`: Disables the input actions when the component is not active.
  - `IsTriggered(InputActionReference)`: Returns whether an action reference is valid and triggered this frame.
  - `SwitchTo(int index, bool forceAll)`: Core switching logic. `forceAll` ensures all weapons are set active/inactive on startup.
  - `SetWeaponActive(int index, bool active)`: Safely toggles a weapon GameObject if it exists and needs a state change.

## Risks / TODOs
- Risk: If the `weaponObjects` list is missing or contains nulls, switching will do nothing. Ensure both entries are assigned.
- TODO: Add support for more than two slots or scroll wheel cycling if needed later.
