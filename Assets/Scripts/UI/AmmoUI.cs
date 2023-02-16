using System;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private AmmoUIDictionary _ammoSlots;

    void OnEnable() {
        foreach (AmmoType ammoType in _ammoSlots.Keys) {
            _ammoSlots[ammoType].Text = PlayerState.Ammo[ammoType].ToString();
        }
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
        PlayerState.OnPlayerAmmoChanged += AmmoChanged;
    }

    void OnDisable() {
        PlayerState.OnPlayerAmmoChanged -= AmmoChanged;
    }

    private void AmmoChanged(object sender, EventArgs e) {
        OnPlayerAmmoChangedArgs args = (OnPlayerAmmoChangedArgs)e;
        _ammoSlots[args.AmmoTypeChanged].Text = args.CurrentAmount.ToString();
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        if (e.RevertType == OnPlayerStateRevertArgs.PlayerRevertType.LevelStart) {
            foreach (AmmoType ammoType in _ammoSlots.Keys) {
                if (ammoType == AmmoType.NONE) continue;
                else {
                    _ammoSlots[ammoType].Text = PlayerState.Ammo[ammoType].ToString();
                } 
            }
        }
    }
}
