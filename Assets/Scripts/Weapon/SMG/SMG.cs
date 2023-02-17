using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : Weapon
{   
    [Header("SMG Specific")]
    [SerializeField] private float coolDownTime;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletRotationDifference;
    private float lastFired;
    private bool currentlyFiring;
    private bool useAmmo;
    
    private void Awake()
    {
        lastFired = 0;
    }
    private void Update() {
        if (currentlyFiring) {
        if (Time.unscaledTime - lastFired < coolDownTime /*|| fired == true*/) { return; }
        if (useAmmo) {
            if (AmmoAmount1 == 0) return;
            AmmoAmount1--;
        }
        lastFired = Time.unscaledTime;
        //fired = true;

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
    }
    public override void Fire1(bool useAmmo = false)
    {
        this.useAmmo = useAmmo;
        this.currentlyFiring = true;
    }

    public override void Fire1Stop(bool useAmmo = false)
    {
        this.currentlyFiring = false;
    }
}
