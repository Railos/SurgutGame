using UnityEngine;

public class DualBerettas : HitscanWeapon
{
    [SerializeField] private Recoil leftRecoil;
    [SerializeField] private Recoil rightRecoil;
    [SerializeField] private bool startWithLeft = true;

    private bool useLeftNext;

    private void Awake()
    {
        useLeftNext = startWithLeft;
    }

    public override void Shoot()
    {
        Recoil selectedRecoil = useLeftNext ? leftRecoil : rightRecoil;
        useLeftNext = !useLeftNext;

        Recoil previousRecoil = recoilScript;
        if (selectedRecoil != null)
        {
            recoilScript = selectedRecoil;
        }

        base.Shoot();

        recoilScript = previousRecoil;
    }
}
