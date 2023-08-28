using System;
using UnityEngine;

public class OnRailgunChargeChangeArgs : EventArgs {
    private bool _started; public bool Started {get => _started;}
    public OnRailgunChargeChangeArgs (bool started) {
        _started = started;
    }
}

public class Railgun : Weapon
{
    [Header("Railgun Specific")]
    [SerializeField] private float coolDownTime;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float waitSeconds; public float WaitSeconds{get => waitSeconds;}
    [SerializeField] private Sound _chargeSound;

    private float lastFired;
    private float timePressed;
    private float timeReleased;

    public event EventHandler<OnRailgunChargeChangeArgs> OnRailGunChargeChange;
    
    // Update is called once per frame
    private void Awake()
    {
        lastFired = 0;
        timePressed = -1;
    }

    public override void Fire1Start(bool useAmmo = false)
    {
        PlaySound(_chargeSound);
        timePressed = Time.unscaledTime;
        if (Time.unscaledTime - lastFired < coolDownTime || AmmoAmount1 == 0) {
            timePressed = -1; 
            return; 
        }
        OnRailGunChargeChange?.Invoke(this, new OnRailgunChargeChangeArgs(true));
    }

    public override void Fire1Stop(bool useAmmo = false)
    {
        timeReleased = Time.unscaledTime;
        if (timePressed != -1 && timeReleased - timePressed > waitSeconds && useAmmo) {
            if (AmmoAmount1 == 0) return;
            AmmoAmount1--;
            base.Fire1Stop();
            lastFired = Time.unscaledTime;
            FireBullet();
        }
        OnRailGunChargeChange?.Invoke(this, new OnRailgunChargeChangeArgs(false));
    }

    private void FireBullet() {
        GameObject bulletInstance = Instantiate(bullet, _attackPoint.position, _holder.transform.rotation);
        Bullet b = bulletInstance.GetComponent<Bullet>();
        b.damageInfo = CreateDamageInfo();
        //b.destroyOnContact = false;
        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletSpeed;
    }
}
