Date: 2026-01-08

# Double and Triple Jump

## What changed and why
The player jump logic now supports multiple jumps before landing, so you can configure double or triple jumps. This keeps movement responsive for shooter gameplay while staying deterministic and avoiding allocations in hot paths.

Changes in `Assets/Scripts/Player/PlayerController.cs`:
- Added `maxJumpCount` (serialized) to control how many jumps are allowed before landing.
- Added `remainingJumps` to track how many jumps are left in the current airborne sequence.
- Added `wasGrounded` to detect landing transitions and reset jumps only when the player actually lands.
- Updated the jump input check to allow a jump whenever `remainingJumps > 0` (not just when grounded).
- When landing is detected, `remainingJumps` resets to `maxJumpCount` so you can jump again.

## New fields and behavior details
- `maxJumpCount` (int, serialized): The total number of jumps allowed between landings. Set to 2 for double jump, 3 for triple jump. Default is 3.
- `remainingJumps` (int, private): Decremented after each jump. Prevents extra jumps once it reaches 0.
- `wasGrounded` (bool, private): Tracks the previous grounded state. When transitioning from not grounded to grounded, the jump count resets.

No new classes or methods were added. The existing `Jump()` method is reused; it now consumes one jump from the counter each time it is called.

## Usage / setup
1) Select the player GameObject that has `PlayerController` attached.
2) In the Inspector, set `Max Jump Count`:
   - 2 = double jump
   - 3 = triple jump
3) Play and press jump in the air to use the extra jumps.

Example:
- If `Max Jump Count = 3`, you can jump from the ground plus two extra times before you must land.
- If `Max Jump Count = 2`, you can jump from the ground plus one extra time before you must land.

## Risks / TODOs
- If ground detection is noisy, jumps may reset early on uneven surfaces. Consider adjusting `playerHeight` or ground raycast distance if this happens.
- If you want different jump force or effects for air jumps, add separate tuning fields and update `Jump()` accordingly.
