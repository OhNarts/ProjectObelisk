using System;
using System.Collections;
using UnityEngine;

public class TutorialSequence : MonoBehaviour
{
    [SerializeField] private TutorialUI _tutorialUI;
    [SerializeField] private string _message;
    [SerializeField] private float _secondsBeforeMessage;
    private IEnumerator _tutorialRoutine;
    private Room _room;

    void Awake() {
        _room = gameObject.GetComponent<Room>();
        _room.OnRoomEnter += OnRoomEnter;
        _room.OnRoomExit += OnRoomExit;
        _tutorialRoutine = TutorialRoutine();
    }

    private void OnRoomEnter(object sender, EventArgs e) {
        StartCoroutine(_tutorialRoutine);
    }
    
    private void OnRoomExit(object sender, EventArgs e) {
        StopCoroutine(_tutorialRoutine);
        _tutorialUI.Message = null;
    }

    private IEnumerator TutorialRoutine() 
    {
        yield return new WaitForSeconds(_secondsBeforeMessage);
        _tutorialUI.Message = _message;
    }
}
