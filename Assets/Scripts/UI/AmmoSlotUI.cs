using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private string _text; public string Text {
        get {
            return _text;
        } set {
            _text = value;
            textMesh.text = _text;
        }
    }
}
