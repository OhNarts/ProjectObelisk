using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;

    void Start() {
        _textMesh.text = PlayerInfo.Instance.Health.ToString();
        PlayerInfo.Instance.OnPlayerHealthChanged += OnPlayerHealthChanged;
    }

    void Disable() {
        PlayerInfo.Instance.OnPlayerHealthChanged -= OnPlayerHealthChanged;
    }

    private void OnPlayerHealthChanged(object sender, EventArgs e) {
        _textMesh.text = ((OnPlayerHealthChangedArgs)e).NewHealth.ToString();
    }
}
