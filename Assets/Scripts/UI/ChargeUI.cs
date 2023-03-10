using System;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
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
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }



    private void OnPlayerCurrentWeaponChanged(object sender, EventArgs e) {
        if ( PlayerState.CurrentWeapon != null && PlayerState.CurrentWeapon.GetType() == typeof(Railgun))  {
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
        if (!_charging) {
            _slider.value = 0;
            return;
        }
        if (Time.unscaledTime >= _chargeTimeEnd) {
            _slider.value = 1;
            return;
        }
        _slider.value = 1 - (_chargeTimeEnd - Time.unscaledTime)/(_chargeTimeEnd - _startTime);
    }

}
