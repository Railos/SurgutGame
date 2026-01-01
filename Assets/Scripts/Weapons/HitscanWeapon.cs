using UnityEngine;

public class HitscanWeapon : Weapon
{

    public float range;

    public override void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, hitLayers))
        {
            IDamageable target = hit.transform.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }


            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * 3f, ForceMode.Impulse);
            }
        }

        base.Shoot();
    }

    public override void Reload()
    {
        base.Reload();
    }

    public override void WaitUntilNextShot()
    {
        base.WaitUntilNextShot();
    }
}
