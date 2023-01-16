using System;
using System.Collections.Generic;
using UnityEngine;

#region Event Args
public class OnPlayerWeaponsChangedArgs : EventArgs {
    private WeaponItem _weaponAdded; public WeaponItem WeaponAdded {get => _weaponAdded;}
    public OnPlayerWeaponsChangedArgs(WeaponItem wep) {
        _weaponAdded = wep;
    }
}

public class OnPlayerAmmoChangedArgs : EventArgs {
    private AmmoType _ammoTypeChanged; public AmmoType AmmoTypeChanged {get => _ammoTypeChanged;}
    private int _oldAmount; public int OldAmount {get => _oldAmount;}
    private int _currentAmount; public int CurrentAmount {get => _currentAmount;}
    public OnPlayerAmmoChangedArgs (AmmoType ammoType, int oldAmount, int newAmount) {
        _ammoTypeChanged = ammoType;
        _oldAmount = oldAmount;
        _currentAmount = newAmount;
    }
}
#endregion

[CreateAssetMenu(fileName = "New Player Info")]
public class PlayerInfo : ScriptableObject
{   
    # region Singleton Stuff
    private static readonly object key = new object();
    private static PlayerInfo instance = null;
    public static PlayerInfo Instance
    {
        get 
        {
            lock (key)
            {
                if (instance == null)
                {
                    instance = Resources.Load<PlayerInfo>("PlayerInfo");
                }
            }
            return instance;
        }
    }
    #endregion

    #region Events
    public delegate void OnPlayerWeaponsChangedHandler(object sender, EventArgs e);
    public event OnPlayerWeaponsChangedHandler OnPlayerWeaponsChanged;
    #endregion 
    public AmmoDictionary Ammo;
    public float MaxHealth;
    public float Health;
    public HashSet<WeaponItem> Weapons;
    
    /// <summary>
    /// Adds a weapon to the players weapon inventory
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public bool AddWeapon(WeaponItem weapon) {
        bool wepAdded = Weapons.Add(weapon);
        if (wepAdded) {
            OnPlayerWeaponsChanged?.Invoke(this,
            new OnPlayerWeaponsChangedArgs(weapon));
        }
        return wepAdded;
    }

}
