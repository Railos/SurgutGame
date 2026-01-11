using UnityEngine;
using UnityEngine.InputSystem;

public class DualBerettas : HitscanWeapon
{
    [Header("Input")]
    [SerializeField] private InputActionReference altFireAction;

    [Header("Boomerang Throw")]
    [SerializeField] private BerettaBoomerang boomerangPrefab;
    [SerializeField] private Transform leftThrowOrigin;
    [SerializeField] private Transform rightThrowOrigin;
    [SerializeField] private Transform returnTarget;
    [SerializeField] private GameObject leftBerettaVisual;
    [SerializeField] private GameObject rightBerettaVisual;
    [SerializeField] private float throwSpeed = 18f;
    [SerializeField] private float returnSpeed = 22f;
    [SerializeField] private float throwMaxDistance = 12f;
    [SerializeField] private float catchDistance = 0.5f;
    [SerializeField] private float throwDamage = 20f;
    [SerializeField] private float throwImpactForce = 4f;
    [SerializeField] private float throwMaxLifetime = 3f;

    [SerializeField] private Recoil leftRecoil;
    [SerializeField] private Recoil rightRecoil;
    [SerializeField] private bool startWithLeft = true;

    private bool useLeftNext;
    private bool useLeftNextForThrow;
    private bool leftAvailable = true;
    private bool rightAvailable = true;
    private BerettaBoomerang activeLeftBoomerang;
    private BerettaBoomerang activeRightBoomerang;

    private void Awake()
    {
        useLeftNext = startWithLeft;
        useLeftNextForThrow = startWithLeft;
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
    }

    public override void Shoot()
    {
        if (!leftAvailable && !rightAvailable)
        {
            return;
        }

        bool shootLeft = SelectLeftForShot();
        useLeftNext = !shootLeft;
        Recoil selectedRecoil = shootLeft ? leftRecoil : rightRecoil;
        Recoil previousRecoil = recoilScript;
        if (selectedRecoil != null)
        {
            recoilScript = selectedRecoil;
        }

        base.Shoot();

        recoilScript = previousRecoil;
    }

    public void OnBoomerangReturned(BerettaBoomerang boomerang, bool wasLeft)
    {
        if (wasLeft)
        {
            if (activeLeftBoomerang == boomerang)
            {
                activeLeftBoomerang = null;
            }

            leftAvailable = true;
            SetVisualActive(leftBerettaVisual, true);
        }
        else
        {
            if (activeRightBoomerang == boomerang)
            {
                activeRightBoomerang = null;
            }

            rightAvailable = true;
            SetVisualActive(rightBerettaVisual, true);
        }
    }

    private void HandleAltFire()
    {
        if (!IsAltFireTriggered())
        {
            return;
        }

        TryThrowBoomerang();
    }

    private bool IsAltFireTriggered()
    {
        if (altFireAction == null)
        {
            return false;
        }

        return altFireAction.action.triggered;
    }

    private void TryThrowBoomerang()
    {
        if (boomerangPrefab == null)
        {
            return;
        }

        if (!leftAvailable && !rightAvailable)
        {
            return;
        }

        bool throwLeft = SelectLeftForThrow();
        Transform origin = throwLeft ? leftThrowOrigin : rightThrowOrigin;
        if (origin == null)
        {
            origin = cam != null ? cam.transform : transform;
        }

        Transform target = returnTarget != null ? returnTarget : transform;
        Vector3 direction = cam != null ? cam.transform.forward : origin.forward;
        BerettaBoomerang boomerang = Instantiate(boomerangPrefab, origin.position, origin.rotation);
        boomerang.Initialize(
            this,
            throwLeft,
            target,
            direction,
            throwSpeed,
            returnSpeed,
            throwMaxDistance,
            catchDistance,
            throwDamage,
            throwImpactForce,
            hitLayers,
            throwMaxLifetime);

        if (throwLeft)
        {
            leftAvailable = false;
            activeLeftBoomerang = boomerang;
            SetVisualActive(leftBerettaVisual, false);
        }
        else
        {
            rightAvailable = false;
            activeRightBoomerang = boomerang;
            SetVisualActive(rightBerettaVisual, false);
        }

        useLeftNextForThrow = !throwLeft;
    }

    private bool SelectLeftForShot()
    {
        if (useLeftNext && leftAvailable)
        {
            return true;
        }

        if (!useLeftNext && rightAvailable)
        {
            return false;
        }

        return leftAvailable;
    }

    private bool SelectLeftForThrow()
    {
        if (useLeftNextForThrow && leftAvailable)
        {
            return true;
        }

        if (!useLeftNextForThrow && rightAvailable)
        {
            return false;
        }

        return leftAvailable;
    }

    private void SetVisualActive(GameObject visual, bool active)
    {
        if (visual == null)
        {
            return;
        }

        if (visual.activeSelf != active)
        {
            visual.SetActive(active);
        }
    }
}
