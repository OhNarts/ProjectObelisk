using System;
using System.ComponentModel;
using UnityEngine;
using System.Collections.Generic;

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
   public event EventHandler<OnRoomEnterAttemptArgs> OnRoomEnterAttempt;
   public event EventHandler OnRoomEnter;
   public event EventHandler OnRoomExit;


    [Header("Edit in level creation")]
    [SerializeField] private DoorRoomDictionary adjacentRooms;

    [Header("Edit in room creation")]
    // Change to increase the distance the camera can be from the room
    [SerializeField] private float _cameraSize;
    [SerializeField] private Transform _camHolderPosRot;

    /* TODO: disable colliders of all children in previous room when planning in current room
     * 
     * Figure out a way to keep track of previous room
     * Make sure we're in planning phase in current room
     * Find and track colliders in children of previous room (floor and wall parts)
     * Disable collider in each part
     * 
     * Bonus: make player kinematic during planning phase
     */

    private bool _occupied; public bool Occupied 
    { 
        get => _occupied; 
        set 
            {
                _occupied = value;
                if (_occupied) {
                    OnRoomEnter?.Invoke(this, EventArgs.Empty);
                }
            } 
    }

    protected Door _doorAttemptedEnter;

    // Add a private function that will subscribe to the gamemanager.onGameStateChanged and check for
    // If it's in planning stage and then setActive to false if in planning stage
    // Else set true
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    void Awake()
    {
        _occupied = false;
        _doorAttemptedEnter = null;

        foreach (Door door in adjacentRooms.Keys)
        {
            door.OnDoorInteract += OnDoorInteract;
        }
    }

    private void OnDestroy()
    {
        foreach (Door door in adjacentRooms.Keys)
        {
            door.OnDoorInteract -= OnDoorInteract;
        }
    }

    private void OnDoorInteract( object sender, EventArgs e )
    {
        if (_occupied || ((HandledEventArgs)e).Handled) return;
        Door door = (Door)sender;
        _doorAttemptedEnter = door;
        ((HandledEventArgs)e).Handled = true;
        OnRoomEnterAttempt?.Invoke(this, new OnRoomEnterAttemptArgs(((OnDoorInteractArgs)e).Player, door));
    }

    public virtual void Enter(PlayerController player, GameObject cameraHolder)
    {
        SetCameraPos(cameraHolder);
        _occupied = true;
        adjacentRooms[_doorAttemptedEnter].Exit();
        _doorAttemptedEnter.EnterDoor();
        OnRoomEnter?.Invoke(this, EventArgs.Empty);
        //doorAttemptedEnter.CloseDoor();
    }

    public virtual void Exit() {
        _occupied = false;
        OnRoomExit?.Invoke(this, EventArgs.Empty);
    }

    public void SetCameraPos(GameObject cameraHolder) {
        cameraHolder.transform.position = _camHolderPosRot.position;
        cameraHolder.transform.rotation = _camHolderPosRot.rotation;
        Camera.main.orthographicSize = _cameraSize;
    }

    private void OnGameStateChanged(object sender, OnGameStateChangedArgs e) 
    {
        if (GameManager.CurrentState == GameState.Plan)
        {
            gameObject.SetActive(false);
        } else if(GameManager.CurrentState == GameState.PostCombat)
        {
            gameObject.SetActive(true);
        }
    } 
}
