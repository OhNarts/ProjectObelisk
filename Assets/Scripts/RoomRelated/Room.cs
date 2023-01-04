using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Room : MonoBehaviour
{
    [SerializeField] public UnityEventRoom onRoomEnterAttempt;

    [Header("Edit in level creation")]
    [SerializeField] private DoorRoomDictionary adjacentRooms;

    [Header("Edit in room creation")]
    // Change to increase the distance the camera can be from the room
    [SerializeField] private float camSize;
    [SerializeField] private Transform camHolderPosRot;

    private bool occupied;

    private Door doorAttemptedEnter;

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

    private void OnDoorInteract( object sender, OnDoorInteractArgs e )
    {
        // Since the player is trying to enter the door from an occupied room, check if this is occupied
        // returns if this is occupied so can call onEnterAttempt on the unoccupied room
        if (occupied) return;
        Door door = (Door)sender;
        onRoomEnterAttempt?.Invoke(this);
        doorAttemptedEnter = door;
    }

    public virtual void Enter(PlayerController player)
    {
        doorAttemptedEnter.EnterDoor();
    }
}
