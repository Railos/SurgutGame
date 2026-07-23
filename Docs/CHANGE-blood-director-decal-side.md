Date: 2026-01-14

# Change: Blood decal side alignment

## What changed and why
- Blood decals now align to the hit surface normal instead of the inverse, preventing them from appearing on the opposite side of walls or floors.
- The decal position uses a configurable surface offset to avoid z-fighting while staying on the correct side.
- Decals now return to the pool after 60 seconds to prevent long-lived buildup.

## Usage / setup
1. Select the object with `BloodDirector`.
2. Set `Decal Surface Offset` to a small value (e.g., `0.02`) so the decal sits just above the surface.
3. Decals will auto-release after 60 seconds; adjust the code if you need a different lifetime.
4. Test by firing blood particles at walls and floors. Decals should appear on the correct side.

Example values:
- `Decal Surface Offset`: 0.02

## Code-level notes
- `BloodDirector.SpawnDecal` now uses `Quaternion.LookRotation(normal)` so the projector faces along the surface normal.
- The decal position is offset by `normal * decalSurfaceOffset` to ensure it lands on the hit side.
- `BloodDirector.ReleaseDecalAfterDelay` is now called with a 60 second delay.

## Risks / TODOs
- If your decal prefab is oriented differently, you may need to rotate the prefab itself or adjust its local axis.
