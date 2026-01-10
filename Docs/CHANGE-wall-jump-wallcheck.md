Date: 2026-01-10

# Wall Jump Wall Check Reference

## What changed and why
The wall detection raycasts now use a configurable `wallCheckTransform` (defaulting to `PlayerController.orientation` if present). This fixes cases where the player object does not rotate with the camera, which caused the old `transform.right` wall checks to only work on world-aligned walls.

## Files
- `Assets/Scripts/Player/WallJump.cs`

## Setup / Usage
1. On the player `WallJump` component, leave `Wall Check Transform` empty to auto-use `PlayerController.orientation`.
2. If you want a different reference, assign a custom transform (for example the player root if it rotates with yaw).

Example: For a camera-driven FPS, keep `Wall Check Transform` empty so it uses the orientation transform.

## Behavior Overview
- Wall checks now follow the chosen transform's right/left axes instead of the player root's world axes.
- This makes wall sticking reliable even when the player object does not rotate with the camera.

## `WallJump` class changes
### New field
- `wallCheckTransform`: Optional transform that defines the left/right axes used for wall raycasts.

### Updated logic
- `Awake()`: Auto-selects `PlayerController.orientation` if available; otherwise falls back to `transform`.
- `CheckWall(...)`: Uses `wallCheckTransform.right` for raycast directions.

## Risks / TODOs
- If `wallCheckTransform` is assigned to an object that rotates unexpectedly, wall checks can become inconsistent.
- TODO: Add optional forward/back checks if you need diagonal wall contact detection.
