using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Shotgun Specific")]
    [SerializeField] private float coolDownTime;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletRotationDifference;
    private float lastFired;
    private bool fired;

    private void Awake()
    {
        fired = false;
        lastFired = 0;
    }

    public override void Fire1(AmmoDictionary ammo)
    {
        base.Fire1(ammo);
        if (Time.unscaledTime - lastFired < coolDownTime || fired == true || ammo[_ammoType1] < _ammoCost1) { return; }
        GameObject[] bullets = new GameObject[3];
        Vector3 currRotOffset = new Vector3(0, -bulletRotationDifference, 0);
        for (int i = 0; i < 3; i++)
        {
            Vector3 currRot = _holder.transform.rotation.eulerAngles + currRotOffset;

            bullets[i] = Instantiate(bullet,
                _attackPoint.position,
                Quaternion.Euler(currRot));

            Bullet b = bullets[i].GetComponent<Bullet>();

            b.damageInfo = new DamageInfo()
            {
                damage = _damage,
                attacker = _holder
            };

            bullets[i].GetComponent<Rigidbody>().velocity = bullets[i].transform.forward * bulletSpeed;

            currRotOffset.y += bulletRotationDifference;
        }

        fired = true;
        lastFired = Time.unscaledTime;
        ammo[_ammoType1] -= _ammoCost1;
    }

    public override void Fire1Stop(AmmoDictionary ammo)
    {
        base.Fire1Stop(ammo);
        fired = false;
    }

}

