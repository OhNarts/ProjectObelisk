using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    [SerializeField] public UnityEventRoom onRoomEnterAttempt;
    
    [SerializeField] private Transform camHolderPos;
    [SerializeField] private DoorRoomDictionary adjacentRooms;

    // Change to increase the distance the camera can be from the room
    [SerializeField] private float camSize;

    private bool occupied;

    private Door doorAttemptedEnter;

    // Start is called before the first frame update

    void Awake()
    {
        occupied = false;
        doorAttemptedEnter = null;

        foreach (Door door in adjacentRooms.Keys)
        {
            
        }
    }

    private void OnDoorInteract( Door door )
    {
        // Since the player is trying to enter the door from an occupied room, check if this is occupied
        // returns if this is occupied so can call onEnterAttempt on the unoccupied room
        if (occupied) return;
        onRoomEnterAttempt?.Invoke(this);
        doorAttemptedEnter = door;
    }

    public virtual void Enter(PlayerController player)
    {
        doorAttemptedEnter.EnterDoor();
    }
}
