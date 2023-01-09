using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region Event Args
public class OnRoomEnterAttemptArgs : EventArgs
{
    private PlayerController _player; public PlayerController Player { get => _player; }
    public OnRoomEnterAttemptArgs(PlayerController player)
    {
        _player = player;
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
    [SerializeField] private float camSize;
    [SerializeField] private Transform camHolderPosRot;

    private bool occupied;

    protected Door doorAttemptedEnter;

    // Start is called before the first frame update

    void Awake()
    {
        occupied = false;
        doorAttemptedEnter = null;

        foreach (Door door in adjacentRooms.Keys)
        {
            door.OnDoorInteract += OnDoorInteract;
        }
    }

    private void OnDisable()
    {
        foreach (Door door in adjacentRooms.Keys)
        {
            door.OnDoorInteract -= OnDoorInteract;
        }
    }

    private void OnDoorInteract( object sender, EventArgs e )
    {
        // Since the player is trying to enter the door from an occupied room, check if this is occupied
        // returns if this is occupied so can call onEnterAttempt on the unoccupied room
        if (occupied) return;
        Door door = (Door)sender;
        OnRoomEnterAttempt?.Invoke(this, new OnRoomEnterAttemptArgs(((OnDoorInteractArgs)e).Player));
        doorAttemptedEnter = door;
    }

    public virtual void Enter(PlayerController player)
    {
        doorAttemptedEnter.EnterDoor();
    }
}
