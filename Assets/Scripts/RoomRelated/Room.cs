using System;
using System.ComponentModel;
using UnityEngine;

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

    [Header("EXPOSED FOR DEBUG")]
    [SerializeField] private bool _occupied; public bool Occupied 
    { 
        get => _occupied; 
        set 
            {
                _occupied = value;
            } 
    }

    protected Door _doorAttemptedEnter;

    void Awake()
    {
        _occupied = false;
        _doorAttemptedEnter = null;

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
        //doorAttemptedEnter.CloseDoor();
    }

    public virtual void Exit() {
        _occupied = false;
    }

    public void SetCameraPos(GameObject cameraHolder) {
        cameraHolder.transform.position = _camHolderPosRot.position;
        cameraHolder.transform.rotation = _camHolderPosRot.rotation;
        Camera.main.orthographicSize = _cameraSize;
    }
}
