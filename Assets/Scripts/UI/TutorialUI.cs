using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    private string _message; 
    /// <summary>
    /// The message that this tutorial panel displays (set to null to make this component inactive)
    /// </summary>
    /// <value></value>
    public string Message {get => _message; set {
        _message = value;
        if (_message == null) {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        _textMesh.text = _message;
    }}
    [SerializeField] private TextMeshProUGUI _textMesh;
}
