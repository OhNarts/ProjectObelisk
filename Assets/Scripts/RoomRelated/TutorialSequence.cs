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

    void Start() {
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

    // void Start() {
    //     _tutorialRoutine = TutorialRoutine();
    //     StartCoroutine(_tutorialRoutine);
    // }

    // public override void Enter(PlayerController player, GameObject cameraHolder)
    // {
    //     base.Enter(player, cameraHolder);
    //     _tutorialRoutine = TutorialRoutine();
    //     StartCoroutine(_tutorialRoutine);
    // }

    // public override void Exit()
    // {
    //     base.Exit();
    //     StopCoroutine(_tutorialRoutine);
    //     _tutorialUI.Message = null;
    // }

    private IEnumerator TutorialRoutine() 
    {
        // Very bad not very good actually, need to change
        yield return new WaitForSeconds(_secondsBeforeMessage);
        _tutorialUI.Message = _message;
    }
}
