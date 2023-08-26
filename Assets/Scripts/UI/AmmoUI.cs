using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private AmmoUIDictionary _ammoSlots;
    [SerializeField] private GameObject ammoUpdates;

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
        int oldText, newText;
        OnPlayerAmmoChangedArgs args = (OnPlayerAmmoChangedArgs)e;

        int.TryParse(_ammoSlots[args.AmmoTypeChanged].Text, out oldText);
        _ammoSlots[args.AmmoTypeChanged].Text = args.CurrentAmount.ToString();
        int.TryParse(_ammoSlots[args.AmmoTypeChanged].Text, out newText);

        Vector3 ammoPos = _ammoSlots[args.AmmoTypeChanged].transform.position;
        ammoUpdates.transform.position =
            new Vector3(ammoUpdates.transform.position.x, ammoPos.y, ammoUpdates.transform.position.z);
        
        TextMeshProUGUI updateText = ammoUpdates.GetComponentInChildren<TextMeshProUGUI>();
        updateText.text = newText - oldText > 0 ? "+" + (newText - oldText).ToString() : (newText - oldText).ToString();
        StartCoroutine(Blink(1f, ammoUpdates));

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

    private IEnumerator Blink(float seconds, GameObject obj) {
        obj.SetActive(true);
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
}
