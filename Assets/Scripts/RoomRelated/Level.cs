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

    [SerializeField] private Room _previousRoom;
    [SerializeField] private Room _currentRoom;

    void Awake()
    {
        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt += OnRoomEnterAttempt;
        }   
        _previousRoom = null;
    }

    void Start() {
        _currentRoom = _rooms[0];
        _currentRoom.SetCameraPos(_cameraHolder);
        _currentRoom.Occupied = true;
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
    }

    private void OnDisable()
    {
        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt -= OnRoomEnterAttempt;
        }
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        foreach (var room in _rooms) {
            if (room.Occupied && room.GetType() == typeof(CombatRoom)) {
                ((CombatRoom)room).Reset();
                break;
            }
        }
        _currentRoom.Occupied = false;
        _currentRoom = _previousRoom;
        _currentRoom.SetCameraPos(_cameraHolder);
        _currentRoom.Occupied = true;
    }

    private void OnRoomEnterAttempt (object sender, EventArgs e)
    {
        if (GameManager.CurrentState == GameState.Combat || GameManager.CurrentState == GameState.Plan) return;

        Room room = (Room)sender;
        OnRoomEnterAttemptArgs args = (OnRoomEnterAttemptArgs)e;
        if (sender.GetType() == typeof(CombatRoom) && !((CombatRoom)sender).RoomCompleted)
        {
            CombatRoom combatRoom = (CombatRoom)sender;
            combatRoom.PlanRoom(args.Player, _cameraHolder);
            combatRoom.OnCombatStart += OnCombatStart;
            PlayerState.Position = args.Player.transform.position;
            PlayerState.SaveAsLastRoom();
        } else
        {
            UpdateToCurrentRoom(room);
            room.Enter(args.Player, _cameraHolder);
        }
    }

    private void OnCombatStart(object sender, EventArgs e) {
        UpdateToCurrentRoom((Room)sender);
        // Unsubscribe itself so that it doesn't get called again by
        // the same room, unless you attempt to enter that room
        // again after a revert
        ((CombatRoom)sender).OnCombatStart -= OnCombatStart;
    }

    private void UpdateToCurrentRoom(Room room) {
        _previousRoom = _currentRoom;
        _currentRoom = room;
    }
}
