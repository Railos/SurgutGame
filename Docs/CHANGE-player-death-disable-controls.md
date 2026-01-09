Date: 2026-01-09

# Change: Disable Player Controls On Death

## What Changed And Why
When the player dies, we now stop all player-controlled movement, camera rotation, and weapon input, and we also unfreeze rigidbody rotation constraints so the body can rotate naturally. This makes death deterministic and prevents the player from continuing to move, look, or shoot after health reaches zero.

## Usage / Setup
1. On your player GameObject, open the `Player` component.
2. (Optional) Assign references under **Death Handling** if you want explicit wiring. If left empty, the script auto-caches common components at `Awake()`.
3. Ensure your weapon scripts live under the player hierarchy so `GetComponentsInChildren<Weapon>` and `GetComponentsInChildren<FistPunch>` can find them if you keep the auto-disable toggles enabled.
4. If you have extra scripts that should stop on death (UI input, audio, etc.), add them to `Extra Disable On Death`.

## Examples
Example: Stop a custom sprint script on death using the `Extra Disable On Death` array.

```csharp
public class PlayerSprint : MonoBehaviour
{
    // This component can be added to Player.extraDisableOnDeath.
}
```

## Class And Method Details
### `Player` (class)
Owns the player health state and now handles death-time control shutdown. It implements `IDamageable` so any weapon or hazard can damage the player.

### `Awake()` (method)
Initializes current health and caches component references for death handling.

### `TakeDamage(float amount)` (method)
Handles incoming damage; triggers `Die()` once health reaches zero.

### `Die()` (method)
Marks the player as dead, disables gameplay controls, and optionally destroys the GameObject if `Destroy On Death` is enabled.

### `CacheReferences()` (method)
Finds and stores the player Rigidbody and key control components if they were not assigned in the Inspector.

### `DisableGameplay()` (method)
Clears rigidbody rotation constraints and disables:
- `PlayerController`, `PlayerCamera`, `PlayerDash`, and `WeaponSwitcher` (if present).
- All `Weapon` components in the player hierarchy (if `Disable Weapon Scripts` is true).
- All `FistPunch` components in the player hierarchy (if `Disable Fist Punch` is true).
- Any scripts listed in `Extra Disable On Death`.

### `Destroy On Death` (field)
If enabled, the player GameObject is destroyed after death handling runs.

## Risks / TODOs
- If you have weapons outside the player hierarchy, they will not be found by the auto-disable scan; add them to `Extra Disable On Death` or move them under the player.
- No ragdoll or death animation is triggered yet; add those in a separate change if needed.
