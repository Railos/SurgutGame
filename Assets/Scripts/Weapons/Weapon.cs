using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Weapon : MonoBehaviour
{
    public InputActionReference shootAction;
    public Camera cam;
    public float damage;
    public float waitUntilNextShot;
    public bool readyToShoot = true;
    public bool holdToShoot;
    public bool isShooting;
    public LayerMask hitLayers;
    public Recoil recoilScript;

    private void Start()
    {
        if (shootAction != null) shootAction.action.Enable();
    }

    public virtual void Update()
    {
        HandleInput();
        CheckShoot();
    }

    public virtual void HandleInput()
    {
        if (holdToShoot) isShooting = shootAction.action.IsInProgress();
        else isShooting = shootAction.action.triggered;
    }

    public virtual void CheckShoot()
    {
        if (!readyToShoot) return;
        if (!isShooting) return;
        Shoot();
    }

    public virtual void Shoot()
    {
        if (recoilScript != null) recoilScript.RecoilFire();
        readyToShoot = false;
        Invoke(nameof(WaitUntilNextShot), waitUntilNextShot);
    }

    public virtual void Reload()
    {

    }

    public virtual void WaitUntilNextShot()
    {
        readyToShoot = true;
    }
}
