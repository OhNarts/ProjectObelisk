using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentWeaponUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;
    [SerializeField] private Image _image;
    [SerializeField] private Weapon _weapon;

    void OnEnable() {
        _weapon = PlayerState.CurrentWeapon;
        PlayerState.OnPlayerCurrentWeaponChanged += WeaponChanged;
        if (_weapon != null)  
        {
            SetUIActive(true);
            _image.sprite = _weapon.WeaponItem.Sprite;
            _textMesh.text = _weapon.AmmoAmount1.ToString();
            _weapon.OnWeaponAmmoChanged += WeaponAmmoChanged;
        } else {
            SetUIActive(false);
        }
    }

    void OnDisable() {
        PlayerState.OnPlayerCurrentWeaponChanged -= WeaponChanged;
        if (_weapon != null) _weapon.OnWeaponAmmoChanged -= WeaponAmmoChanged;
    }

    private void WeaponChanged(object sender, EventArgs e) {
        if (_weapon != null) _weapon.OnWeaponAmmoChanged -= WeaponAmmoChanged;
        _weapon = PlayerState.CurrentWeapon;
        SetUIActive(_weapon != null);
        if (_weapon != null)   {
            _weapon.OnWeaponAmmoChanged += WeaponAmmoChanged;
            _textMesh.text = _weapon.AmmoAmount1.ToString();
            _image.sprite = _weapon.WeaponItem.Sprite;
        }
        
    }

    private void WeaponAmmoChanged(object sender, EventArgs e) {
        _textMesh.text = _weapon.AmmoAmount1.ToString();
    }

    private void SetUIActive(bool active) {
        _image.gameObject.SetActive(active);
        _textMesh.gameObject.SetActive(active);
    }

}
