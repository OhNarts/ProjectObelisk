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
        PlayerState.OnPlayerStateRevert += OnPlayerHealthChanged;
    }

    void Disable() {
        PlayerState.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        PlayerState.OnPlayerStateRevert -= OnPlayerHealthChanged;
    }

    private void OnPlayerHealthChanged(object sender, EventArgs e) {
        _textMesh.text = PlayerState.Health.ToString();
    }
}
