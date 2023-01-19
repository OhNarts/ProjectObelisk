using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;

    void OnEnable() {
        _textMesh.text = PlayerState.Health.ToString();
        PlayerState.OnPlayerHealthChanged += OnPlayerHealthChanged;
    }

    void Disable() {
        PlayerState.OnPlayerHealthChanged -= OnPlayerHealthChanged;
    }

    private void OnPlayerHealthChanged(object sender, EventArgs e) {
        _textMesh.text = ((OnPlayerHealthChangedArgs)e).NewHealth.ToString();
    }
}
