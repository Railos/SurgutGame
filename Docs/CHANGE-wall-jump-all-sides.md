Date: 2026-01-10

# Wall Jump All-Sides Detection

## What changed and why
Wall detection now checks forward and backward directions in addition to left and right. This lets the player stick to walls from any side instead of only the left/right axes.

## Files
- `Assets/Scripts/Player/WallJump.cs`

## Setup / Usage
1. Enable `Use Forward Wall Checks` on the `WallJump` component (it is enabled by default).
2. Keep `Wall Check Transform` aligned with your camera/player yaw so forward/back directions match movement.

Example: For an FPS controller, leave `Wall Check Transform` empty so it uses the orientation transform and keeps forward/back checks aligned with look direction.

## Behavior Overview
- The wall check now casts rays left, right, forward, and backward.
- If any ray hits a wall layer, wall stick/slide and wall jump become available.

## `WallJump` class changes
### New field
- `useForwardWallChecks`: Toggles forward/back raycasts for full-side wall sticking.

### Updated logic
- `CheckWall(...)`: Adds forward/back raycasts when `useForwardWallChecks` is enabled.

## Risks / TODOs
- If your player has large colliders, increase `Wall Check Distance` to avoid missing walls.
- TODO: Consider a single sphere cast to replace the four raycasts if you want uniform coverage.
