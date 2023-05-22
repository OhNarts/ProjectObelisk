using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _uiImage;
    [SerializeField] private TextMeshProUGUI _weaponName;
    // [SerializeField] private string _weaponName;
    private WeaponItem _weapon;
    public WeaponItem Weapon {
        set {
            _weapon = value;
            _uiImage.sprite = _weapon.Sprite;
            _uiImage.GetComponent<Image>().color = _weapon.Color;
            if (_weapon.WeaponName == "DoubleBarrelShotgun") {
                _weaponName.text = "DoubleBarrel";
            } else {
                _weaponName.text = _weapon.WeaponName;
            }
        }
        get {
            return _weapon;
        }
    }
}
