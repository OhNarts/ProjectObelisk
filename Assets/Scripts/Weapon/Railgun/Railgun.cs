using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : Weapon
{
    [Header("Railgun Specific")]
    [SerializeField] private float coolDownTime;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float waitSeconds;

    private float lastFired;
    private float timePressed;
    private float timeReleased;
    
    // Update is called once per frame
    private void Awake()
    {
        lastFired = 0;
    }

    public override void Fire1Start(bool useAmmo = false)
    {
        timePressed = Time.unscaledTime;
        if (Time.unscaledTime - lastFired < coolDownTime) { return; }
    }

    public override void Fire1Stop(bool useAmmo = false)
    {
        timeReleased = Time.unscaledTime;
        if (timeReleased - timePressed > waitSeconds && useAmmo) {
            if (AmmoAmount1 == 0) return;
            AmmoAmount1--;
            lastFired = Time.unscaledTime;
            FireBullet();
        }
    }

    private void FireBullet() {
        GameObject bulletInstance = Instantiate(bullet, _attackPoint.position, _holder.transform.rotation);
        Bullet b = bulletInstance.GetComponent<Bullet>();
        b.damageInfo = CreateDamageInfo();
        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletSpeed;
    }
}
