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

    public override void Fire1(AmmoDictionary ammo)
    {
        base.Fire1(ammo);
        if (Time.unscaledTime - lastFired < coolDownTime  || fired == true || ammo[_ammoType1] < _ammoCost1) { return; }

        lastFired = Time.unscaledTime;

        // Subtract ammo cost
        ammo[_ammoType1] -= _ammoCost1;
        fired = true;

        // Fire the weapon
        GameObject bulletInstance = Instantiate(bullet, _attackPoint.position, _holder.transform.rotation);
        Bullet b = bulletInstance.GetComponent<Bullet>();
        b.damageInfo = new DamageInfo()
        {
            damage = _damage,
            attacker = _holder
        };
        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletSpeed;
    }


    public override void Fire1Stop(AmmoDictionary ammo)
    {
        base.Fire1Stop(ammo);
        fired = false;
    }
}
