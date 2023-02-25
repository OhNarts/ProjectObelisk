using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeUI : MonoBehaviour
{
    private Railgun _railgun;
    private bool _charging;
    private float _chargeTimeEnd;
    private float _startTime;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        PlayerState.OnPlayerCurrentWeaponChanged += OnPlayerCurrentWeaponChanged;
        GameManager.OnGameStateChanged += OnGameStateChanged; 
    }

    void OnDestroy() {
        PlayerState.OnPlayerCurrentWeaponChanged -= OnPlayerCurrentWeaponChanged;
    }



    private void OnPlayerCurrentWeaponChanged(object sender, EventArgs e) {
        if (PlayerState.CurrentWeapon.GetType() == typeof(Railgun))  {
            _railgun = (Railgun)(PlayerState.CurrentWeapon);
            _railgun.OnRailGunChargeChange += OnRailGunChargeChange;   
            gameObject.SetActive(true); 
            return;
        }
        if (_railgun != null) _railgun.OnRailGunChargeChange -= OnRailGunChargeChange;
        gameObject.SetActive(false); 
    }

    private void OnGameStateChanged(object sender, OnGameStateChangedArgs e) {
        if (e.OldState == GameState.Combat && e.NewState == GameState.PostCombat) {
            if (_railgun != null) {
                _railgun.OnRailGunChargeChange -= OnRailGunChargeChange;
                _railgun = null;
            }
        }
        gameObject.SetActive(false);
    }

    private void OnRailGunChargeChange(object sender, OnRailgunChargeChangeArgs e) {
        _charging = e.Started;
        _startTime = Time.unscaledTime;
        _chargeTimeEnd = _startTime + _railgun.WaitSeconds;
    }

    void Update() {
        if (!_charging) return;
        Debug.Log("Display Charge");
    }

}
