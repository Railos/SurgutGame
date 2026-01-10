Date: 2026-01-10

# Wall Jump

## What changed and why
Added a new `WallJump` component and integrated it with `PlayerController` so the player can stick briefly to walls, slide down slowly, and jump away with a strong impulse. This keeps airborne movement responsive while giving predictable wall behavior without extra allocations in hot paths.

## Files
- `Assets/Scripts/Player/WallJump.cs`
- `Assets/Scripts/Player/PlayerController.cs`

## Setup / Usage
1. Add the `WallJump` component to the player GameObject that already has `PlayerController` and a `Rigidbody`.
2. Assign `Wall Mask` to the layers you consider wall surfaces.
3. Assign `Ground Mask` if you want `WallJump` to be able to detect ground without `PlayerController`. If you are using `PlayerController`, this can stay empty.
4. Tune `Wall Stick Time`, `Wall Slide Speed`, `Wall Jump Force`, and `Wall Jump Up Force` to get the desired feel.
5. Ensure the player capsule or collider is aligned so the side raycasts hit walls at the `Wall Check Height Offset`.

Example: For a snappy wall jump, set `Wall Jump Force` to 14, `Wall Jump Up Force` to 9, and `Wall Slide Speed` to 3.

## Behavior Overview
- When airborne and touching a wall, the player sticks for a short time, then slides down at a capped speed.
- Jump input while on a wall triggers a strong impulse away from the wall plus an upward boost.
- The standard jump is suppressed if a wall jump is performed on the same input frame.
- A small cooldown prevents immediately re-sticking to the same wall right after a wall jump.

## `WallJump` class
`WallJump` controls wall detection, wall stick/slide behavior, and wall jump impulses.

### Fields
- `wallMask`: Layers counted as walls for side raycasts.
- `wallCheckDistance`: How far to raycast left/right to find a wall.
- `wallCheckHeightOffset`: Vertical offset for the wall check origin (use chest height).
- `wallStickTime`: How long the player sticks before sliding.
- `wallSlideSpeed`: Maximum downward speed while sliding.
- `wallJumpForce`: Lateral impulse away from the wall.
- `wallJumpUpForce`: Upward impulse for the wall jump.
- `wallJumpCooldown`: Short delay after wall jumping before wall sliding can resume.
- `groundCheckDistance`: Raycast distance for fallback ground detection.
- `groundMask`: Layers considered ground for fallback checks.
- `playerController`: Optional reference to use `PlayerController.isGrounded` instead of a local raycast.

### Properties
- `IsWallSliding`: True while the component is actively sticking/sliding on a wall.
- `LastWallNormal`: The most recent wall surface normal used for wall behavior.

### Methods
- `Awake()`: Caches the `Rigidbody` and auto-finds `PlayerController` if not assigned.
- `FixedUpdate()`: Updates wall stick/slide state without allocations. Clamps downward velocity while sticking or sliding.
- `TryWallJump()`: Returns true if a wall jump was performed. Applies the impulse, resets stick state, and starts the cooldown.
- `IsGrounded()`: Uses `PlayerController.isGrounded` if available; otherwise uses a fallback raycast.
- `CheckWall(out Vector3 wallNormal)`: Raycasts left and right to detect a wall and outputs the wall normal.

## `PlayerController` integration
`PlayerController` now asks `WallJump.TryWallJump()` when jump is pressed. If a wall jump happens, the normal jump is skipped so the input is consumed by the wall jump.

## Risks / TODOs
- If the player collider is not centered, the side raycasts may miss walls; adjust `Wall Check Height Offset` or the player collider position.
- Very low frame rates can make the stick timer feel longer because it is updated in `FixedUpdate`.
- TODO: Add optional forward/back wall checks for diagonal wall contact, if needed for level layouts.
