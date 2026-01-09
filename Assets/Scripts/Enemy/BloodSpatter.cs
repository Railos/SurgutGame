using UnityEngine;

public class BloodSpatter : MonoBehaviour
{
    [SerializeField] bool destroyOnEnter = true;

    public Vector3 SurfaceNormal { get; private set; } = Vector3.up;

    public void SetSurfaceNormal(Vector3 normal)
    {
        if (normal.sqrMagnitude < 0.0001f)
        {
            return;
        }

        SurfaceNormal = normal.normalized;
    }

    public bool TryEnter(Transform mobTransform)
    {
        if (mobTransform == null)
        {
            return false;
        }

        mobTransform.position = transform.position;

        if (destroyOnEnter)
        {
            Destroy(gameObject);
        }

        return true;
    }
}
