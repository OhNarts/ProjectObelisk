using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

public class OnPlayerHealthChangedArgs : EventArgs {
    private float _oldHealth; public float OldHealth {get => _oldHealth;}
    private float _newHealth; public float NewHealth {get => _newHealth;}
    public OnPlayerHealthChangedArgs(float oldHealth, float newHealth) {
        _oldHealth = oldHealth;
        _newHealth = newHealth;
    }
}

public class OnPlayerMaxHealthChangedArgs : EventArgs {
    private float _oldMaxHealth; public float OldMaxHealth {get => _oldMaxHealth;}
    private float _newMaxHealth; public float NewMaxHealth {get => _newMaxHealth;}
    public OnPlayerMaxHealthChangedArgs(float oldMaxHealth, float newMaxHealth) {
        _oldMaxHealth = oldMaxHealth;
        _newMaxHealth = newMaxHealth;
    }
}

public class OnPlayerStateRevertArgs : EventArgs {
    public enum PlayerRevertType { LastRoom, LevelStart }
    private PlayerRevertType _revertType; public PlayerRevertType RevertType {get => _revertType;}
    public OnPlayerStateRevertArgs(PlayerRevertType type) {
        _revertType = type;
    }
}
#endregion

[CreateAssetMenu(fileName = "New Player State", menuName = "Player/Player State")]
public class PlayerState : ScriptableObject
{   
    # region Singleton Stuff
    private static readonly object key = new object();
    private static PlayerState _instance = null;
    public static PlayerState Instance
    {
        get 
        {
            lock (key)
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<PlayerState>("PlayerState");
                    _instance._levelStartInfo = new PlayerInfo (_instance._initialInfo);
                    _instance._currentInfo = _instance._levelStartInfo.CreateCopy();
                    SceneManager.sceneLoaded += OnSceneLoaded;
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Events
    public delegate void OnPlayerWeaponsChangedHandler(object sender, EventArgs e);
    public static event OnPlayerWeaponsChangedHandler OnPlayerWeaponsChanged;

    public delegate void OnPlayerAmmoChangedHandler(object sender, EventArgs e);
    public static event OnPlayerAmmoChangedHandler OnPlayerAmmoChanged;

    public delegate void OnPlayerHealthChangedHandler(object sender, EventArgs e);
    public static event OnPlayerHealthChangedHandler OnPlayerHealthChanged;

    public delegate void OnPlayerCurrentWeaponChangedHandler(object sender, EventArgs e);
    public static event OnPlayerCurrentWeaponChangedHandler OnPlayerCurrentWeaponChanged;

    public delegate void OnPlayerMaxHealthChangedHandler(object sender, EventArgs e);
    public static event OnPlayerMaxHealthChangedHandler OnPlayerMaxHealthChanged;

    public static event EventHandler<OnPlayerStateRevertArgs> OnPlayerStateRevert;

    #endregion 
    [SerializeField] private PlayerInfo _currentInfo; public static PlayerInfo CurrentInfo {get => _instance._currentInfo;}
    private PlayerInfo _lastRoomInfo; public static PlayerInfo LastRoomInfo {get => _instance._lastRoomInfo;}
    [SerializeField] private PlayerInfo _levelStartInfo; public static PlayerInfo LevelStartInfo {get => _instance._levelStartInfo;}
    [SerializeField] private PlayerInfoObject _initialInfo; public static PlayerInfoObject InitialInfo {get => _instance._initialInfo;}

    public static AmmoDictionary Ammo {get => Instance._currentInfo.Ammo;}

    public static void AddToAmmo(AmmoType ammoType, int addingAmount) {
        ChangeAmmo(ammoType, Ammo[ammoType] + addingAmount);
    }
    public static void ChangeAmmo(AmmoType ammoType, int newAmount) {
        int oldAmount =  Ammo[ammoType];
        Ammo[ammoType] = newAmount;
        OnPlayerAmmoChanged?.Invoke(Instance,
        new OnPlayerAmmoChangedArgs(ammoType, oldAmount, newAmount));
    }

    public static float MaxHealth {
        get => Instance._currentInfo.MaxHealth;
        set  {
            var oldMax = Instance._currentInfo.MaxHealth;
            Instance._currentInfo.MaxHealth = value;
            OnPlayerMaxHealthChanged?.Invoke(Instance,
            new OnPlayerMaxHealthChangedArgs(oldMax, Instance._currentInfo.MaxHealth));
        }

    }

    public static float Health {
        get {
            return Instance._currentInfo.Health;
        } set {
            var oldHealth = Instance._currentInfo.Health;
            Instance._currentInfo.Health = value;
            OnPlayerHealthChanged?.Invoke(Instance,
            new OnPlayerHealthChangedArgs(oldHealth, Instance._currentInfo.Health));
        }
    }

    public static WeaponHashSet Weapons {
        get => Instance._currentInfo.Weapons;
    }
    
    /// <summary>
    /// Adds a weapon to the players weapon inventory
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public static bool AddWeapon(WeaponItem weapon) {
        bool wepAdded = Instance._currentInfo.Weapons.Add(weapon);
        if (wepAdded) {
            OnPlayerWeaponsChanged?.Invoke(Instance,
            new OnPlayerWeaponsChangedArgs(weapon));
        }
        return wepAdded;
    }

    public static Weapon CurrentWeapon {
        get => Instance._currentInfo.CurrentWeapon;
        set {
            Instance._currentInfo.CurrentWeapon = value;
            OnPlayerCurrentWeaponChanged?.Invoke(Instance, EventArgs.Empty);
        }
    }

    public static Vector3 Position {
        get => Instance._currentInfo.Position;
        set {
            Instance._currentInfo.Position = value;
        }
    }
    
    public static void SaveAsLastRoom() {
        _instance._lastRoomInfo = _instance._currentInfo.CreateCopy();
    }

    public static bool CanRevertToLastRoom() {
        return _instance._lastRoomInfo != null;
    }
    
    public static void RevertToLastRoom() {
        if (!CanRevertToLastRoom()) return;
        _instance._currentInfo = _instance._lastRoomInfo.CreateCopy();
        Debug.Log(_instance._currentInfo.Health);
        OnPlayerStateRevert?.Invoke(Instance, new OnPlayerStateRevertArgs(
            OnPlayerStateRevertArgs.PlayerRevertType.LastRoom
        ));
    }

    public static void RevertToLevelStart() {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
         _instance._currentInfo = _instance._levelStartInfo.CreateCopy();

        // OnPlayerStateRevert?.Invoke(Instance, new OnPlayerStateRevertArgs(
        //     OnPlayerStateRevertArgs.PlayerRevertType.LevelStart
        // ));
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        _instance._levelStartInfo = new PlayerInfo (_instance._initialInfo);
        _instance._currentInfo = _instance._levelStartInfo.CreateCopy();
    }
}
