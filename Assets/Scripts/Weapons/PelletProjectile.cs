using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PelletProjectile : MonoBehaviour
{
    private float lifetime = 0.6f;
    private float damage = 1f;
    private float impactForce = 6f;
    private LayerMask hitLayers;

    private Rigidbody cachedRigidbody;

    private void Awake()
    {
        cachedRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (lifetime > 0f)
        {
            Invoke(nameof(DestroySelf), lifetime);
        }
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(DestroySelf));
    }

    public void Initialize(float pelletDamage, LayerMask layers, float force, float lifeTime)
    {
        damage = pelletDamage;
        hitLayers = layers;
        impactForce = force;
        if (lifeTime > 0f)
        {
            lifetime = lifeTime;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        if (cachedRigidbody != null)
        {
            cachedRigidbody.linearVelocity = velocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null && collision.collider.GetComponent<PelletProjectile>() != null)
        {
            return;
        }

        bool validLayer = (hitLayers.value & (1 << collision.gameObject.layer)) != 0;
        if (!validLayer)
        {
            return;
        }

        IDamageable target = collision.transform.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        if (collision.rigidbody != null)
        {
            ContactPoint contact = collision.GetContact(0);
            collision.rigidbody.AddForce(-contact.normal * impactForce, ForceMode.Impulse);
        }

        DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
