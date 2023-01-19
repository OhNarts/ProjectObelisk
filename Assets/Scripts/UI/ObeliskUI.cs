using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _combatUI;
    [SerializeField] private GameObject[] _planningUI;

    void Start() {
        if (GameManager.CurrentState == GameState.Combat) {
            ShowCombatUI();
        } else {
            ShowPlanningUI();
        }
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDisable() {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(object sender, EventArgs e) {
        OnGameStateChangedArgs args = (OnGameStateChangedArgs) e;
        if (args.NewState == GameState.Combat) {
            ShowCombatUI();
        } else {
            ShowPlanningUI();
        }
    }

    private void ShowCombatUI() {
        foreach (GameObject comUI in _combatUI) {
            comUI.SetActive(true);
        }
        foreach (GameObject planUI in _planningUI) {
            planUI.SetActive(false);
        }
    }

    private void ShowPlanningUI() {
        foreach (GameObject comUI in _combatUI) {
            comUI.SetActive(false);
        }
        foreach (GameObject planUI in _planningUI) {
            planUI.SetActive(true);
        }
    }
}
