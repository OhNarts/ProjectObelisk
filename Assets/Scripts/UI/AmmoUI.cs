using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private AmmoUIDictionary _ammoSlots;
    [SerializeField] private AmmoGameObjectDictionary _ammoUpdates;

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
        Debug.Log(newText + "," + oldText);

        // Vector3 ammoPos = _ammoSlots[args.AmmoTypeChanged].transform.position;
        // ammoUpdates.transform.position =
        //     new Vector3(ammoUpdates.transform.position.x, ammoPos.y, ammoUpdates.transform.position.z);
        
        TextMeshProUGUI updateText = _ammoUpdates[args.AmmoTypeChanged].GetComponentInChildren<TextMeshProUGUI>();
        updateText.text = newText - oldText > 0 ?
            "+" + (newText - oldText).ToString() : (newText - oldText).ToString();
        updateText.color = new Color(1, 1, 1, 1);
        _ammoUpdates[args.AmmoTypeChanged].GetComponent<Image>().color = new Color(0.1037736f, 0.1037736f, 0.1037736f, 0.3921569f);
        StartCoroutine(Blink(1.5f, _ammoUpdates[args.AmmoTypeChanged], updateText));

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

    private IEnumerator Blink(float seconds, GameObject obj1, TextMeshProUGUI text) {
        obj1.SetActive(true);
        Image image = obj1.GetComponent<Image>();

        Color oldImage = image.color;
        Color oldText = text.color;
        float alpha = 0f;
        float counter = oldText.a / (oldImage.a / .075f);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        for (float i = 0f; i < oldImage.a; i += 0.075f) {
            image.color = new Color(oldImage.r, oldImage.g, oldImage.b, i);
            text.color = new Color(oldText.r, oldText.g, oldText.b, alpha);
            alpha += counter;
            yield return new WaitForSeconds(0.03f);
        }

        for (float i = oldImage.a; i > 0f; i -= 0.075f) {
            image.color = new Color(oldImage.r, oldImage.g, oldImage.b, i);
            text.color = new Color(oldText.r, oldText.g, oldText.b, alpha);
            alpha -= counter;
            yield return new WaitForSeconds(0.07f);
        }
        obj1.SetActive(false);
        image.color = oldImage;
        text.color = oldText;
    }

    // private IEnumerator Blink(GameObject obj, bool fadeIn, float seconds) {
    //     obj.SetActive(true);
    //     Image image = obj.GetComponent<Image>();
    //     TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();

    //     float counter = 0;

    //     Color oldColor;
    //     if (image != null) {
    //         oldColor = image.color;
    //     } else {
    //         oldColor = text.color;
    //     }

    //     float start = fadeIn ? 0 : oldColor.a;
    //     float end = fadeIn ? oldColor.a : 0;

    //     while (counter < seconds) {
    //         counter += Time.deltaTime;
    //         float alpha = Mathf.Lerp(start, end, counter / seconds);
    //         if (image != null) {
    //             image.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
    //         } else if (text != null) {
    //             text.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
    //         }
    //     }

    //     yield return null;
    //     obj.SetActive(false);
    // }
}
