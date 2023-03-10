using System;
using System.Collections;
using UnityEngine;
public class CombatTutorialSequence : MonoBehaviour {
    [SerializeField] private string _planMessage;
    [SerializeField] private string _combatMessage;
    [SerializeField] private string _postCombatMessage;
    [SerializeField] private TutorialUI _tutorialUI;
    [SerializeField] private CombatRoom _combatRoom;

    void Awake() {
        GameManager.OnGameStateChanged += OnGameStateChanged;
        _combatRoom.OnRoomExit += OnRoomExit;
    }

    void OnDestroy() {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(object sender, OnGameStateChangedArgs e) {
        // Kinda jank, but hey it works
        if (!_combatRoom.isActiveAndEnabled) return;
        string toDisplay;
        switch(e.NewState) {
            case GameState.Plan:
                toDisplay = _planMessage;
                break;
            case GameState.Combat:
                toDisplay = _combatMessage;
                break;
            case GameState.PostCombat:
                toDisplay = _postCombatMessage;
                break;
            default:
                toDisplay = null;
                break;
        }
        _tutorialUI.Message = toDisplay;
    }

    private void OnRoomExit(object sender, EventArgs e) {
        _tutorialUI.Message = null;
    }
}