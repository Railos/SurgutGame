Date: 2026-01-14

# Feature: Player mantle (auto snap)

## What changed and why
- Added an automatic mantle check to `PlayerController` so the player can snap onto ledges without pressing a button.
- The mantle is a snap (instant reposition) to keep movement deterministic and avoid animation dependencies.

## Usage / setup
1. Select the player object with `PlayerController`.
2. Set `Mantle Layers` to the same layers you use for ground and climbable ledges.
3. Tune the mantle fields:
   - `Mantle Forward Check Distance` controls how far ahead we look for a ledge face.
   - `Mantle Lower Ray Height` is the height for the wall hit check.
   - `Mantle Upper Ray Height` is the height that must be clear so it is considered a ledge.
   - `Mantle Max Height` is how high above the wall hit we search for a top surface.
   - `Mantle Top Forward Offset` moves the top search slightly forward from the wall hit.
   - `Mantle Top Down Extra` extends the downward search distance for uneven tops.
   - `Mantle Surface Offset` lifts the final snap position to avoid floor clipping.
   - `Mantle Wall Clearance` pushes the snap position away from the wall so the player ends up on top.
   - `Mantle Duration` controls how long the mantle movement takes.
   - `Mantle Use Kinematic` temporarily makes the rigidbody kinematic during the mantle to reduce jitter.
   - `Mantle Interpolation` overrides the rigidbody interpolation during the mantle for smoother motion.
   - `Mantle Cooldown` prevents repeated mantles on the same ledge.
4. Play and move toward a ledge. If the lower ray hits a wall, the upper ray is clear, and a top surface is found, the player snaps onto the ledge.

Example values:
- `Mantle Layers`: set to your ground/ledge layers
- `Mantle Forward Check Distance`: 0.6
- `Mantle Lower Ray Height`: 0.6
- `Mantle Upper Ray Height`: 1.6
- `Mantle Max Height`: 1.5
- `Mantle Top Forward Offset`: 0.2
- `Mantle Top Down Extra`: 0.3
- `Mantle Surface Offset`: 0.05
- `Mantle Wall Clearance`: 0.2
- `Mantle Duration`: 0.15
- `Mantle Use Kinematic`: true
- `Mantle Interpolation`: Interpolate
- `Mantle Cooldown`: 0.2

## Code-level notes
- `PlayerController.TryMantle` performs three checks:
  1. Lower forward raycast must hit a surface in `mantleLayers`.
  2. Upper forward raycast must be clear to confirm a ledge.
  3. Downward raycast from above the ledge must hit a walkable surface (angle <= `maxSlopeAngle`).
- When a mantle succeeds, the player is moved over `mantleDuration` using `rb.MovePosition` with smoothstep easing.
- The target position is the top surface plus `mantleSurfaceOffset`, pushed away from the wall by `mantleWallClearance`.
- While mantling, the rigidbody can be set to kinematic and interpolation can be forced to reduce jitter.
- `TryMantle` is called automatically each `Update` after reading movement input. If it succeeds, jump input is ignored for that frame.
- New serialized fields in `PlayerController`:
  - `enableMantle`: toggles mantling without removing the logic.
  - `mantleLayers`: layers that can be mantled onto.
  - `mantleForwardCheckDistance`: forward ray length for ledge detection.
  - `mantleLowerRayHeight`: height of the lower raycast.
  - `mantleUpperRayHeight`: height of the upper raycast (must be clear).
  - `mantleMaxHeight`: vertical search height for a top surface.
  - `mantleTopForwardOffset`: forward offset for the top search origin.
  - `mantleTopDownExtra`: extra distance for the downward top search.
  - `mantleSurfaceOffset`: vertical offset applied to the snap point.
  - `mantleWallClearance`: additional distance away from the wall when snapping to the top.
  - `mantleDuration`: time in seconds for the mantle movement.
  - `mantleUseKinematic`: toggle to force kinematic during mantle.
  - `mantleInterpolation`: interpolation mode used during mantle.
  - `mantleCooldown`: time between mantle attempts.

## Risks / TODOs
- If `mantleDuration` is too small, the mantle can still feel abrupt.
- If you use other systems that depend on dynamic rigidbody during mantle, leave `Mantle Use Kinematic` off.
- If `mantleLayers` includes non-walkable surfaces, the player may snap onto unwanted geometry.
