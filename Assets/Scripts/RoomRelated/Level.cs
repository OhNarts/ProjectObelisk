using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] private Room[] _rooms; public Room[] Rooms { get => _rooms; }
    [SerializeField] private Scene _nextScene; public Scene NextScene { get => _nextScene; }

    [SerializeField] private GameObject _cameraHolder;

    void Awake()
    {
        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt += OnRoomEnterAttempt;
        }    
        
    }

    void Start() {
        _rooms[0].SetCameraPos(_cameraHolder);
        _rooms[0].Occupied = true;
    }

    private void OnDisable()
    {
        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt -= OnRoomEnterAttempt;
        }
    }

    private void OnRoomEnterAttempt (object sender, EventArgs e)
    {
        Debug.Log("Room Enter Attempt");
        if (GameManager.Instance.CurrentState == GameState.Combat || 
        GameManager.Instance.CurrentState == GameState.Plan)
            return;

        Room room = (Room)sender;
        OnRoomEnterAttemptArgs args = (OnRoomEnterAttemptArgs)e;
        Debug.Log(sender.GetType());
        if (sender.GetType() == typeof(CombatRoom) && !((CombatRoom)sender).RoomCompleted)
        {
            Debug.Log("Plan Enter");
            ((CombatRoom)sender).PlanRoom(args.Player, _cameraHolder);
        } else
        {
            Debug.Log("Normal Enter");
            room.Enter(args.Player, _cameraHolder);
        }
    }
}
