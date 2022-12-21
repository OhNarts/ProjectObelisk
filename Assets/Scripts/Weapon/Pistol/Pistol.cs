using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    [Header("Pistol Specific")]
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float range;

    private float lastFired;
    // Since pistol is semi-automatic, keep track of whether or not fired yet 
    private bool fired;

    private int hitcount;
    private int firecount;

    private void Awake()
    {
        fired = false;
        lastFired = 0;

        hitcount = 0;
        firecount = 0;
    }

    public override void Fire1(Dictionary<AmmoType, int> ammo)
    {
        base.Fire1(ammo);
        if (ammo[_ammoType1] < _ammoCost1 || fired == true) { return; }
        // Fire the weapon
        Debug.Log("Fire " + ++firecount);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            Debug.Log("hit " + hit.transform.name + " "+ ++hitcount);
        }
        // Subtract ammo cost
        ammo[_ammoType1] -= _ammoCost1; 
        fired = true;
    }


    public override void Fire1Stop(Dictionary<AmmoType, int> ammo)
    {
        base.Fire1Stop(ammo);
        fired = false;
    }

    public override void Fire2(Dictionary<AmmoType, int> ammo)
    {
        base.Fire2(ammo);
        float currTime = Time.fixedTime;
        if (ammo[_ammoType2] < _ammoCost2 || currTime - lastFired < fireRate) { return; }

        // Fire the weapon
        

        // Subtract ammo cost
        ammo[_ammoType2] -= _ammoCost2;
        lastFired = currTime;
    }
}
