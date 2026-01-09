using UnityEngine;
using UnityEngine.InputSystem;

public class FistPunch : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference punchAction;

    [Header("Animation")]
    [SerializeField] private Animator fistAnimator;
    [SerializeField] private string punchTrigger = "Punch";

    [Header("Hit Settings")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform punchOrigin;
    [SerializeField] private float range = 2f;
    [SerializeField] private float radius = 0.15f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float impactForce = 2f;
    [SerializeField] private LayerMask hitLayers = ~0;

    [Header("Timing")]
    [SerializeField] private float cooldown = 0.4f;

    private float nextPunchTime;

    private void Start()
    {
        if (punchAction != null)
        {
            punchAction.action.Enable();
        }
    }

    private void Update()
    {
        if (!IsPunchPressed())
        {
            return;
        }

        if (Time.time < nextPunchTime)
        {
            return;
        }

        ExecutePunch();
        nextPunchTime = Time.time + cooldown;
    }

    private bool IsPunchPressed()
    {
        if (punchAction == null)
        {
            return false;
        }

        return punchAction.action.triggered;
    }

    private void ExecutePunch()
    {
        PlayPunchAnimation();
        TryHitTarget();
    }

    private void PlayPunchAnimation()
    {
        if (fistAnimator == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(punchTrigger))
        {
            fistAnimator.SetTrigger(punchTrigger);
        }
    }

    private void TryHitTarget()
    {
        Transform origin = GetPunchOrigin();
        Vector3 direction = origin.forward;
        Vector3 start = origin.position;

        bool hitSomething;
        RaycastHit hit;

        if (radius > 0f)
        {
            hitSomething = Physics.SphereCast(start, radius, direction, out hit, range, hitLayers, QueryTriggerInteraction.Ignore);
        }
        else
        {
            hitSomething = Physics.Raycast(start, direction, out hit, range, hitLayers, QueryTriggerInteraction.Ignore);
        }

        if (!hitSomething)
        {
            return;
        }

        IDamageable target = hit.transform.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(direction * impactForce, ForceMode.Impulse);
        }
    }

    private Transform GetPunchOrigin()
    {
        if (punchOrigin != null)
        {
            return punchOrigin;
        }

        if (cam != null)
        {
            return cam.transform;
        }

        return transform;
    }
}
