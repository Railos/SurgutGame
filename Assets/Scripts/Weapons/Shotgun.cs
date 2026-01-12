using UnityEngine;
using UnityEngine.InputSystem;

public class Shotgun : BulletWeapon
{
    private const int MaxAmmo = 2;

    [Header("Input")]
    [SerializeField] private InputActionReference altFireAction;
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float pelletSpeed = 45f;
    [SerializeField] private float pelletLifetime = 0.6f;
    [SerializeField] private float spreadAngle = 6f;
    [SerializeField] private float impactForce = 6f;
    [SerializeField] private Collider[] ignoreColliders;
    [Min(0.01f)][SerializeField] private float reloadInterval = 0.6f;
    [Min(0.01f)][SerializeField] private float altFireDamageMultiplier = 1.25f;
    [Min(0f)][SerializeField] private float altFireKnockbackForce = 8f;
    [SerializeField] private Rigidbody playerRigidbody;

    private int currentAmmo = MaxAmmo;
    private float reloadTimer;

    private void Awake()
    {
        currentAmmo = MaxAmmo;
        reloadTimer = 0f;
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponentInParent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        if (altFireAction != null)
        {
            altFireAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (altFireAction != null)
        {
            altFireAction.action.Disable();
        }
    }

    public override void Update()
    {
        HandleAltFire();
        base.Update();
        AutoReload(Time.deltaTime);
    }

    public override void CheckShoot()
    {
        if (!readyToShoot) return;
        if (!isShooting) return;
        if (currentAmmo <= 0) return;
        Shoot();
    }

    public override void Shoot()
    {
        if (cam == null || pelletPrefab == null)
        {
            base.Shoot();
            return;
        }

        if (currentAmmo <= 0)
        {
            return;
        }

        currentAmmo--;
        reloadTimer = 0f;

        FirePelletVolleys(1, 1f);
        base.Shoot();
    }

    private void HandleAltFire()
    {
        if (altFireAction == null)
        {
            return;
        }

        if (!altFireAction.action.triggered)
        {
            return;
        }

        if (!readyToShoot)
        {
            return;
        }

        if (currentAmmo < 2)
        {
            return;
        }

        if (cam == null || pelletPrefab == null)
        {
            base.Shoot();
            return;
        }

        currentAmmo -= 2;
        reloadTimer = 0f;

        FirePelletVolleys(2, altFireDamageMultiplier);
        ApplyAltFireKnockback();
        base.Shoot();
    }

    private void FirePelletVolleys(int volleys, float damageMultiplier)
    {
        if (cam == null || pelletPrefab == null)
        {
            return;
        }

        int pellets = Mathf.Max(1, pelletCount);
        float pelletDamage = damage * damageMultiplier;
        Vector3 origin = muzzle != null ? muzzle.position : cam.transform.position;

        for (int volley = 0; volley < volleys; volley++)
        {
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
        }
    }

    private void ApplyAltFireKnockback()
    {
        if (playerRigidbody == null || cam == null || altFireKnockbackForce <= 0f)
        {
            return;
        }

        Vector3 shotDirection = cam.transform.forward;
        playerRigidbody.AddForce(-shotDirection * altFireKnockbackForce, ForceMode.Impulse);
    }

    private void AutoReload(float deltaTime)
    {
        if (currentAmmo >= MaxAmmo)
        {
            reloadTimer = 0f;
            return;
        }

        if (reloadInterval <= 0f)
        {
            currentAmmo = MaxAmmo;
            reloadTimer = 0f;
            return;
        }

        reloadTimer += deltaTime;

        while (reloadTimer >= reloadInterval && currentAmmo < MaxAmmo)
        {
            currentAmmo++;
            reloadTimer -= reloadInterval;
        }

        if (currentAmmo >= MaxAmmo)
        {
            reloadTimer = 0f;
        }
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
