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

    public override void Fire1Start(bool useAmmo = false)
    {
        if ( Time.unscaledTime - lastFired < coolDownTime || fired == true ) { return; }    
        if (useAmmo) {
            if (AmmoAmount1 == 0) return;
            AmmoAmount1--;
            base.Fire1Start();
        }
        GameObject[] bullets = new GameObject[3];
        Vector3 currRotOffset = new Vector3(0, -bulletRotationDifference, 0);
        for (int i = 0; i < 3; i++)
        {
            Vector3 currRot = _holder.transform.rotation.eulerAngles + currRotOffset;

            bullets[i] = Instantiate(bullet,
                _attackPoint.position,
                Quaternion.Euler(currRot));

            Bullet b = bullets[i].GetComponent<Bullet>();

            b.damageInfo = CreateDamageInfo();

            bullets[i].GetComponent<Rigidbody>().velocity = bullets[i].transform.forward * bulletSpeed;

            currRotOffset.y += bulletRotationDifference;
        }

        fired = true;
        lastFired = Time.unscaledTime;
    }

    public override void Fire1Stop(bool useAmmo = false)
    {
        fired = false;
    }

}

