using UnityEngine;

public class Shotgun : BulletWeapon
{
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float pelletSpeed = 45f;
    [SerializeField] private float pelletLifetime = 0.6f;
    [SerializeField] private float spreadAngle = 6f;
    [SerializeField] private float impactForce = 6f;
    [SerializeField] private Collider[] ignoreColliders;

    public override void Shoot()
    {
        if (cam == null || pelletPrefab == null)
        {
            base.Shoot();
            return;
        }

        int pellets = Mathf.Max(1, pelletCount);
        float pelletDamage = damage;
        Vector3 origin = muzzle != null ? muzzle.position : cam.transform.position;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = GetPelletDirection();
            Quaternion pelletRotation = Quaternion.LookRotation(direction, cam.transform.up);
            GameObject pellet = Instantiate(pelletPrefab, origin, pelletRotation);
            IgnorePelletCollisions(pellet);

            PelletProjectile projectile = pellet.GetComponent<PelletProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(pelletDamage, hitLayers, impactForce, pelletLifetime);
                projectile.SetVelocity(direction * pelletSpeed);
            }
            else
            {
                Rigidbody pelletRigidbody = pellet.GetComponent<Rigidbody>();
                if (pelletRigidbody != null)
                {
                    pelletRigidbody.linearVelocity = direction * pelletSpeed;
                }
            }
        }

        base.Shoot();
    }

    private void IgnorePelletCollisions(GameObject pellet)
    {
        if (ignoreColliders == null || ignoreColliders.Length == 0)
        {
            return;
        }

        Collider pelletCollider = pellet.GetComponent<Collider>();
        if (pelletCollider == null)
        {
            return;
        }

        for (int i = 0; i < ignoreColliders.Length; i++)
        {
            Collider ignore = ignoreColliders[i];
            if (ignore != null)
            {
                Physics.IgnoreCollision(pelletCollider, ignore, true);
            }
        }
    }

    private Vector3 GetPelletDirection()
    {
        Vector2 circle = Random.insideUnitCircle * spreadAngle;
        Quaternion yaw = Quaternion.AngleAxis(circle.x, cam.transform.up);
        Quaternion pitch = Quaternion.AngleAxis(-circle.y, cam.transform.right);
        Vector3 direction = yaw * pitch * cam.transform.forward;
        return direction.normalized;
    }
}
