using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] private Room[] _rooms; public Room[] Rooms { get => _rooms; }
    [SerializeField] private GameObject _cameraHolder;

    private Room _previousRoom;
    private Room _currentRoom;
    private CombatRoom _planningRoom;
    public TransformWeaponDictionary BaseWeapons;

    void Awake()
    {
        _previousRoom = null;
        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt += OnRoomEnterAttempt;
        }   
        GameManager.OnGameStateChanged += OnGameStateChange;
    }

    void Start() {
        _planningRoom = null;
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
        GameManager.OnGameStateChanged -= OnGameStateChange;
    }

    private void OnGameStateChange(object sender, OnGameStateChangedArgs e) {
        if (e.OldState == GameState.Plan && e.NewState == GameState.PostCombat) {
            _currentRoom?.SetCameraPos(_cameraHolder);
            if (_planningRoom != null) _planningRoom.OnCombatStart -= OnCombatStart;
        }
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        foreach (var room in _rooms) {
            if (room.GetType() != typeof(CombatRoom)) continue;
            CombatRoom combatRoom = (CombatRoom)room;
            if (combatRoom.Occupied || 
            (e.RevertType == OnPlayerStateRevertArgs.PlayerRevertType.LevelStart)) {
                ((CombatRoom)room).Reset();
            }
        }

        _currentRoom.Occupied = false;

        if (e.RevertType == OnPlayerStateRevertArgs.PlayerRevertType.LevelStart) {
            _currentRoom = _rooms[0];
            _previousRoom = null;
            InitializeWeapons();
        } else {
            _currentRoom = _previousRoom;
        }

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
            _planningRoom = combatRoom;
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
        _planningRoom = null;
        // Unsubscribe itself so that it doesn't get called again by
        // the same room, unless you attempt to enter that room
        // again after a revert
        ((CombatRoom)sender).OnCombatStart -= OnCombatStart;
    }

    private void UpdateToCurrentRoom(Room room) {
        _previousRoom = _currentRoom;
        _currentRoom = room;
    }
    
    private void InitializeWeapons() {
        foreach (KeyValuePair<Transform, Weapon> weaponKVP in BaseWeapons) {
            Weapon weaponInstance = Instantiate(weaponKVP.Value, weaponKVP.Key.position, Quaternion.identity);
        }
    }
}
