using UnityEngine;
using UnityEngine.Pool; // Required for Object Pooling
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class BloodDirector : MonoBehaviour
{
    [Header("Settings")]
    public ParticleSystem bloodParticleSystem;
    public GameObject decalPrefab; // Your URP Decal Projector Prefab
    public int maxDecals = 200;
    [SerializeField] float decalSurfaceOffset = 0.02f;

    private List<ParticleCollisionEvent> collisionEvents;
    private ObjectPool<GameObject> decalPool;

    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        
        // Initialize Pool to save memory
        decalPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(decalPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            maxSize: maxDecals
        );
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle collided with: " + other.name + " on layer " + LayerMask.LayerToName(other.layer));

        // 1. Get collisions for this specific object
        int numCollisionEvents = bloodParticleSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            // 2. Get data
            Vector3 pos = collisionEvents[i].intersection;
            Vector3 normal = collisionEvents[i].normal;

            // 3. Spawn Decal from Pool
            SpawnDecal(pos, normal);
        }
    }

    void SpawnDecal(Vector3 position, Vector3 normal)
    {
        GameObject decal = decalPool.Get();
        
        // Position slightly off the wall to prevent z-fighting (if not using Decal Projector)
        decal.transform.rotation = Quaternion.LookRotation(normal);
        // Then optional twist:
        decal.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));
        decal.transform.position = position + normal * decalSurfaceOffset;
        // Optional: Return to pool after X seconds
        StartCoroutine(ReleaseDecalAfterDelay(decal, 60f));
    }

    System.Collections.IEnumerator ReleaseDecalAfterDelay(GameObject decal, float delay)
    {
        yield return new WaitForSeconds(delay);
        decalPool.Release(decal);
    }
}
