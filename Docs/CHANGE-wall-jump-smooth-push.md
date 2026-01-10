Date: 2026-01-10

# Wall Jump Smooth Push

## What changed and why
Wall jumps now apply their push over a short duration instead of a single instant impulse. This reduces the "teleport" feel and makes the jump-out feel more like acceleration while keeping the same total velocity change.

## Files
- `Assets/Scripts/Player/WallJump.cs`

## Setup / Usage
1. Set `Wall Jump Push Duration` to a small value like `0.06` to `0.12` seconds for a smoother push.
2. Set `Wall Jump Push Duration` to `0` if you want the old instant impulse behavior.

Example: Use `Wall Jump Force = 12`, `Wall Jump Up Force = 8`, and `Wall Jump Push Duration = 0.08` for a strong but less jarring jump.

## Behavior Overview
- The wall jump's horizontal and vertical velocity change is now spread over the configured duration.
- The same total impulse is applied, just distributed across a few `FixedUpdate` steps.

## `WallJump` class changes
### New field
- `wallJumpPushDuration`: How long to apply the wall jump velocity change. `0` means instant impulse.

### Updated logic
- `FixedUpdate()`: Applies the pending wall jump velocity change over time using `ForceMode.VelocityChange`.
- `TryWallJump()`: Starts the timed push if `wallJumpPushDuration > 0`.

## Risks / TODOs
- If `Wall Jump Push Duration` is too long, the jump can feel weak; keep it short.
- TODO: Add an optional curve for non-linear push if smoother ease-in/out is desired.
