using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

#region Event Args
public class OnRoomEnterAttemptArgs : EventArgs
{
    private PlayerController _player; public PlayerController Player { get => _player; }
    private Door _door; public Door Door { get => _door; }
    public OnRoomEnterAttemptArgs(PlayerController player, Door door)
    {
        _player = player;
        _door = door;
    }
}

#endregion


[Serializable]
public class Room : MonoBehaviour
{
    public delegate void OnRoomEnterAttemptHandler(object sender, EventArgs e);
    public event OnRoomEnterAttemptHandler OnRoomEnterAttempt;

    [Header("Edit in level creation")]
    [SerializeField] private DoorRoomDictionary adjacentRooms;

    [Header("Edit in room creation")]
    // Change to increase the distance the camera can be from the room
    [SerializeField] private float _cameraSize;
    [SerializeField] private Transform _camHolderPosRot;

    private bool _occupied; public bool Occupied 
    { 
        get => _occupied; 
    set 
        {
            _occupied = value;
        } 
    }

    protected Door doorAttemptedEnter;

    // Start is called before the first frame update

    void Awake()
    {
        InitializeRoom();
    }

    private void OnDisable()
    {
        foreach (Door door in adjacentRooms.Keys)
        {
            door.OnDoorInteract -= OnDoorInteract;
        }
    }

    protected void InitializeRoom() {
        _occupied = false;
        doorAttemptedEnter = null;

        foreach (Door door in adjacentRooms.Keys)
        {
            door.OnDoorInteract += OnDoorInteract;
        }
    }

    private void OnDoorInteract( object sender, EventArgs e )
    {
        // Since the player is trying to enter the door from an occupied room, check if this is occupied
        // returns if this is occupied so can call onEnterAttempt on the unoccupied room
        if (_occupied) return;
        Door door = (Door)sender;
        doorAttemptedEnter = door;
        OnRoomEnterAttempt?.Invoke(this, new OnRoomEnterAttemptArgs(((OnDoorInteractArgs)e).Player, door));
    }

    public virtual void Enter(PlayerController player, GameObject cameraHolder)
    {
        SetCameraPos(cameraHolder);
        _occupied = true;
        doorAttemptedEnter.EnterDoor();
    }

    public void SetCameraPos(GameObject cameraHolder) {
        cameraHolder.transform.position = _camHolderPosRot.position;
        cameraHolder.transform.rotation = _camHolderPosRot.rotation;
        Camera.main.orthographicSize = _cameraSize;
    }
}
