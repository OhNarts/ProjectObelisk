using System;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private AmmoUIDictionary _ammoSlots;

    void OnEnable() {
        foreach (AmmoType ammoType in _ammoSlots.Keys) {
            _ammoSlots[ammoType].Text = PlayerInfo.Instance.Ammo[ammoType].ToString();
        }
        PlayerInfo.Instance.OnPlayerAmmoChanged += AmmoChanged;
    }

    void OnDisable() {
        PlayerInfo.Instance.OnPlayerAmmoChanged -= AmmoChanged;
    }

    private void AmmoChanged(object sender, EventArgs e) {
        OnPlayerAmmoChangedArgs args = (OnPlayerAmmoChangedArgs)e;
        _ammoSlots[args.AmmoTypeChanged].Text = args.CurrentAmount.ToString();
    }
}
