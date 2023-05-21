using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] private DoorRoomTupleDictionary _roomDictionary;
    [SerializeField] private GameObject _cameraHolder;
    [SerializeField] private Room _startRoom;
    public TransformWeaponDictionary BaseWeapons;

    [Header("Exposed for Debug")]
    [SerializeField] private Room _previousRoom;
    [SerializeField] private Room _currentRoom;
    private HashSet<Room> _rooms;
    private CombatRoom _planningRoom;
    private List<Weapon> _baseWeapons;
    

    void Awake()
    {
        _rooms = new HashSet<Room>();
        // Initialize the rooms (There's probably a more efficient way to do this)
        foreach (Door door in _roomDictionary.Keys) {
            RoomTuple roomTuple = _roomDictionary[door];
            _rooms.Add(roomTuple.Room1);
            _rooms.Add(roomTuple.Room2);
            door.OnDoorInteract += roomTuple.Room1.OnDoorInteract;
            door.OnDoorInteract += roomTuple.Room2.OnDoorInteract;
        }

        _previousRoom = null;
        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt += OnRoomEnterAttempt;
        }   
        GameManager.OnGameStateChanged += OnGameStateChange;
    }

    void Start() {
        _planningRoom = null;
        _currentRoom = _startRoom;
        _currentRoom.SetCameraPos(_cameraHolder);
        _currentRoom.Occupied = true;
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
        _baseWeapons = new List<Weapon>();
        InitializeWeapons();
    }

    private void OnDestroy()
    {
        foreach (Door door in _roomDictionary.Keys) {
            RoomTuple roomTuple = _roomDictionary[door];
            door.OnDoorInteract -= roomTuple.Room1.OnDoorInteract;
            door.OnDoorInteract -= roomTuple.Room2.OnDoorInteract;
        }

        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt -= OnRoomEnterAttempt;
        }
        GameManager.OnGameStateChanged -= OnGameStateChange;
        PlayerState.OnPlayerStateRevert -= OnPlayerStateRevert;
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
            _currentRoom = _startRoom;
            _previousRoom = null;
            DestroyWeapons();
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
        _previousRoom.Exit();
        _currentRoom = room;
    }
    
    private void InitializeWeapons() {
        foreach (KeyValuePair<Transform, Weapon> weaponKVP in BaseWeapons) {
            Weapon weaponInstance = Instantiate(weaponKVP.Value, weaponKVP.Key.position, Quaternion.identity);
            _baseWeapons.Add(weaponInstance);
        }
    }
    
    private void DestroyWeapons() {
        while (_baseWeapons.Count != 0) {
            var currentWeapon = _baseWeapons[0];
            _baseWeapons.RemoveAt(0);
            if (currentWeapon == null) continue;
            else{
                Destroy(currentWeapon.gameObject);
            }
        }
    }
}
