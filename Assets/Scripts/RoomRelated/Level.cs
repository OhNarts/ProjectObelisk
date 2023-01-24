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
        if (GameManager.CurrentState == GameState.Combat || GameManager.CurrentState == GameState.Plan) return;

        PlayerState.SaveAsLastRoom();
        Room room = (Room)sender;
        OnRoomEnterAttemptArgs args = (OnRoomEnterAttemptArgs)e;
        if (sender.GetType() == typeof(CombatRoom) && !((CombatRoom)sender).RoomCompleted)
        {
            ((CombatRoom)sender).PlanRoom(args.Player, _cameraHolder);
        } else
        {
            room.Enter(args.Player, _cameraHolder);
        }
    }
}
