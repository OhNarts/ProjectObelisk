using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    [Header("Pistol Specific")]
    [SerializeField] private float fireRate;
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

        //PistolBullet b = bullet.GetComponent<PistolBullet>();
        //b.damageInfo = new DamageInfo()
        //{
        //    damage = _damage,
        //    attacker = _holder
        //};
        //b.speed = bulletSpeed;
    }

    public override void Fire1(Dictionary<AmmoType, int> ammo)
    {
        base.Fire1(ammo);
        if (Time.unscaledTime - lastFired < fireRate  || fired == true || ammo[_ammoType1] < _ammoCost1) { return; }

        lastFired = Time.unscaledTime;

        // Subtract ammo cost
        ammo[_ammoType1] -= _ammoCost1;
        fired = true;

        // Fire the weapon
        GameObject bulletInstance = Instantiate(bullet, _attackPoint.position, _holder.transform.rotation);
        PistolBullet b = bulletInstance.GetComponent<PistolBullet>();
        b.damageInfo = new DamageInfo()
        {
            damage = _damage,
            attacker = _holder
        };
        b.speed = bulletSpeed;
        //b.speed = 0;
        Destroy(bulletInstance, 100);

        //RaycastHit hit;
        //if (!Physics.Raycast(transform.position, transform.forward, out hit, range)) return;

        //HealthHandler enemyHealth = hit.transform.GetComponent<HealthHandler>();
        //if (enemyHealth != null)
        //{
        //    enemyHealth.Damage(new DamageInfo() { 
        //        damage = _damage, 
        //        attacker = _holder
        //    });
        //}
    }


    public override void Fire1Stop(Dictionary<AmmoType, int> ammo)
    {
        base.Fire1Stop(ammo);
        fired = false;
    }
}
