using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    [Header("Pistol Specific")]
    [SerializeField] private float coolDownTime;
    [SerializeField] private float range;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bullet;
    

    private float lastFired;
    // Since pistol is semi-automatic, keep track of whether or not fired yet 
    private bool fired;

    private void Awake()
    {
        fired = false;
        lastFired = 0;
    }

    public override void Fire1(bool useAmmo = false)
    {
        if (Time.unscaledTime - lastFired < coolDownTime  || fired == true) { return; }
        if (useAmmo) {
            if (AmmoAmount1 == 0) return;
            AmmoAmount1--;
        }
        lastFired = Time.unscaledTime;
        fired = true;

        // Fire the weapon
        GameObject bulletInstance = Instantiate(bullet, _attackPoint.position, _holder.transform.rotation);
        Bullet b = bulletInstance.GetComponent<Bullet>();
        b.damageInfo = CreateDamageInfo();
        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletSpeed;
    }


    public override void Fire1Stop(bool useAmmo = false)
    {
        fired = false;
    }
}
