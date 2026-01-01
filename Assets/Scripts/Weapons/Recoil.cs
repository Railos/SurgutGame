using UnityEngine;

public class Recoil : MonoBehaviour
{
    // References
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [Header("Settings")]
    public float snappiness = 6f; // How fast it snaps up
    public float kickUp = 2f;
    public float returnSpeed = 2f; // How fast it returns to center

    void Update()
    {
        // 1. Slerp target rotation back to zero (Restoring force)
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        
        // 2. Slerp current rotation to the target (Smoothing the kick)
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        
        // 3. Apply to Local Rotation
        // We add this to whatever rotation the WeaponAnchor/Sway is already doing.
        // NOTE: If this script is on the SAME object as WeaponSway, you might fight for control.
        // Ideally, create a child "RecoilPivot" or use 'localRotation *= ...' carefully.
        // For simplicity, let's assume this is a child of WeaponSway or applied additively.
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        // Define the Kick (Randomized)
        // X: Pitch Up (Negative X is usually up in Unity if forward is Z, depends on your rig)
        // Y: Yaw (Random Side to Side)
        // Z: Tilt (Random twist)
        
        float recoilX = -kickUp; // Kick UP
        float recoilY = Random.Range(-0.5f, 0.5f); // Random Side shake
        float recoilZ = Random.Range(-0.3f, 0.3f); // Random Tilt

        // Add to target (This allows recoil to stack/climb if you spam)
        targetRotation += new Vector3(recoilX, recoilY, recoilZ);
    }
}
