using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _uiImage;
    private WeaponItem _weapon;
    public WeaponItem Weapon {
        set {
            _weapon = value;
            _uiImage.sprite = _weapon.Sprite;
            _uiImage.GetComponent<Image>().color = _weapon.Color;
        }
        get {
            return _weapon;
        }
    }
}
