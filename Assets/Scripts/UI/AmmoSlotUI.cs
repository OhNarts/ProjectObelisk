using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Image image;
    private string _text; public string Text {
        get {
            return _text;
        } set {
            _text = value;
            textMesh.text = _text;
        }
    }

    private Color _color; public Color Color {
        get {
            return _color;
        } set {
            _color = value;
            image.color = _color;
        }
    }
}
