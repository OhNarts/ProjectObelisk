using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessHealthHandler : HealthHandler
{
    [SerializeField] private AmmoTypeFloatDictionary _weaknessDictionary; public AmmoTypeFloatDictionary WeaknessDictionary {get => _weaknessDictionary; }

    public override void Damage(DamageInfo info) {
        int addedDamage = 0;
        if (_weaknessDictionary.Contains(info.ammoType)) {
            addedDamage = (int)(_weaknessDictionary[info.ammoType] * info.damage);
        }
        DamageInfo addedInfo = new DamageInfo {
            ammoType = info.ammoType,
            attacker = info.attacker,
            damage = info.damage + addedDamage
        };
        Debug.LogFormat("New Damage Amount:{0}", addedInfo.damage);
        base.Damage(addedInfo);
    }
 
}
