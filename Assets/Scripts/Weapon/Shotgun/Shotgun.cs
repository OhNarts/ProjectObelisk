using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Shotgun Specific")]
    [SerializeField] private float coolDownTime;

    private float lastFired;
    private bool fired;

    private void Awake()
    {
        fired = false;
        lastFired = 0;
    }

    public override void Fire1(Dictionary<AmmoType, int> ammo)
    {
        base.Fire1(ammo);

    }
}
