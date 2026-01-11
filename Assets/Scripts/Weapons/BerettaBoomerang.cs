using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BerettaBoomerang : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 720f;

    private Rigidbody cachedRigidbody;
    private Collider cachedCollider;
    private DualBerettas owner;
    private Transform returnTarget;
    private Vector3 startPosition;
    private Vector3 travelDirection;
    private Vector3 currentMoveDirection;
    private LayerMask hitLayers;

    private float speed;
    private float returnSpeed;
    private float maxDistance;
    private float catchDistance;
    private float damage;
    private float impactForce;
    private float lifeTime;
    private float lifeTimer;

    private bool isLeft;
    private bool returning;
    private bool notifiedOwner;

    private void Awake()
    {
        cachedRigidbody = GetComponent<Rigidbody>();
        cachedCollider = GetComponent<Collider>();

        if (cachedRigidbody != null)
        {
            cachedRigidbody.isKinematic = true;
            cachedRigidbody.useGravity = false;
            cachedRigidbody.constraints = RigidbodyConstraints.None;
        }

        if (cachedCollider != null)
        {
            cachedCollider.isTrigger = true;
        }
    }

    public void Initialize(
        DualBerettas ownerWeapon,
        bool leftSide,
        Transform target,
        Vector3 direction,
        float travelSpeed,
        float returnTravelSpeed,
        float maxTravelDistance,
        float catchRadius,
        float throwDamage,
        float throwImpactForce,
        LayerMask layers,
        float maxLifeTime)
    {
        owner = ownerWeapon;
        isLeft = leftSide;
        returnTarget = target;
        speed = Mathf.Max(0f, travelSpeed);
        returnSpeed = Mathf.Max(0f, returnTravelSpeed);
        maxDistance = Mathf.Max(0f, maxTravelDistance);
        catchDistance = Mathf.Max(0.01f, catchRadius);
        damage = throwDamage;
        impactForce = throwImpactForce;
        hitLayers = layers;
        lifeTime = maxLifeTime;
        transform.SetParent(null, true);
        startPosition = transform.position;
        travelDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
        currentMoveDirection = travelDirection;
        returning = false;
        lifeTimer = 0f;
        notifiedOwner = false;
    }

    private void Update()
    {
        if (returnTarget == null)
        {
            DestroySelf();
            return;
        }

        lifeTimer += Time.deltaTime;
        if (lifeTime > 0f && lifeTimer >= lifeTime)
        {
            returning = true;
        }

        Vector3 currentPosition = transform.position;

        if (!returning && maxDistance > 0f)
        {
            float traveledSqr = (currentPosition - startPosition).sqrMagnitude;
            if (traveledSqr >= maxDistance * maxDistance)
            {
                returning = true;
            }
        }

        Vector3 nextPosition;
        if (returning)
        {
            Vector3 toTarget = returnTarget.position - currentPosition;
            float distanceSqr = toTarget.sqrMagnitude;
            if (distanceSqr <= catchDistance * catchDistance)
            {
                NotifyOwner();
                DestroySelf();
                return;
            }

            Vector3 dir = toTarget.normalized;
            currentMoveDirection = dir;
            nextPosition = currentPosition + dir * returnSpeed * Time.deltaTime;
        }
        else
        {
            currentMoveDirection = travelDirection;
            nextPosition = currentPosition + travelDirection * speed * Time.deltaTime;
        }

        MoveTo(nextPosition);

        if (spinSpeed != 0f)
        {
            transform.Rotate(Vector3.right, spinSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.isTrigger)
        {
            return;
        }

        if (owner != null && other.transform.IsChildOf(owner.transform))
        {
            return;
        }

        bool validLayer = (hitLayers.value & (1 << other.gameObject.layer)) != 0;
        if (!validLayer)
        {
            return;
        }

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        if (other.attachedRigidbody != null)
        {
            Vector3 forceDirection = currentMoveDirection.sqrMagnitude > 0.0001f
                ? currentMoveDirection
                : transform.forward;
            other.attachedRigidbody.AddForce(forceDirection * impactForce, ForceMode.Impulse);
        }
    }

    private void MoveTo(Vector3 position)
    {
        transform.position = position;
        if (cachedRigidbody != null)
        {
            cachedRigidbody.position = position;
        }
    }

    private void NotifyOwner()
    {
        if (notifiedOwner)
        {
            return;
        }

        notifiedOwner = true;
        if (owner != null)
        {
            owner.OnBoomerangReturned(this, isLeft);
        }
    }

    private void DestroySelf()
    {
        NotifyOwner();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NotifyOwner();
    }
}
