using System;
using System.Collections;
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
        Color oldColor = new Color(0.1037736f, 0.1037736f, 0.1037736f, 0.3921569f);
        Color newColor;
        if (args.OldAmount - args.CurrentAmount > 1) {
            newColor = new Color(1f, 0f, 0f, 0.3921569f);
        } else if (args.OldAmount - args.CurrentAmount < 0) {
            newColor = new Color(0f, 1f, 0f, 0.3921569f);
        } else {
            newColor = oldColor;
        }
        StartCoroutine(ChangeColor(args, oldColor, newColor));
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

    private IEnumerator ChangeColor(OnPlayerAmmoChangedArgs args, Color oldColor, Color newColor) {
        _ammoSlots[args.AmmoTypeChanged].Color = newColor;
        yield return new WaitForSeconds(0.4f);
        _ammoSlots[args.AmmoTypeChanged].Color = oldColor;
    }
}
