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
        StartCoroutine(ChangeColor(args, oldColor));
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

    private IEnumerator ChangeColor(OnPlayerAmmoChangedArgs args, Color oldColor) {
        int component = 3;
        if (args.OldAmount - args.CurrentAmount > 1) {
            component = 0;
        } else if (args.OldAmount - args.CurrentAmount < 0) {
            component = 1;
        }

        float startColor = oldColor[component];
        float startAlpha = oldColor.a;

        for (float i = startColor; i <= 1f; i += 0.075f) {
            oldColor[component] = i;
            oldColor.a += 0.075f;
            _ammoSlots[args.AmmoTypeChanged].Color = oldColor;
            yield return new WaitForSeconds(0.005f);
        }

        for (float i = 1f; i >= startColor; i -= 0.075f) {
            oldColor[component] = i;
            oldColor.a -= 0.075f;
            _ammoSlots[args.AmmoTypeChanged].Color = oldColor;
            yield return new WaitForSeconds(0.02f);
        }

        oldColor[component] = startColor;
        oldColor.a = startAlpha;
        _ammoSlots[args.AmmoTypeChanged].Color = oldColor;
    }
}
