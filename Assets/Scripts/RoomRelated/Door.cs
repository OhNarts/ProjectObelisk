using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

#region Event args
public class OnDoorInteractArgs : HandledEventArgs
{
    private PlayerController _player; public PlayerController Player { get => _player; }
    public OnDoorInteractArgs(PlayerController player)
    {
        _player = player;
    }
}

#endregion

[Serializable]
public class Door : MonoBehaviour, Interactable
{
    [SerializeField] public UnityEvent onEnter;

    public delegate void OnDoorInteractHandler(object sender, EventArgs args);
    public event OnDoorInteractHandler OnDoorInteract;

    [SerializeField] private Transform hinge;
    // The point outside a door where the player waits before entering
    [SerializeField] private Transform waitPoint1;
    [SerializeField] private Transform waitPoint2;
    private PlayerController _player;
    private float rotateDegrees;
    
    private (Vector3, Vector3) waitPts;

    private bool openedPreviously; public bool OpenedPreviously { get => openedPreviously; }

    void Awake()
    {
        _player = null;
        rotateDegrees = 90;
        openedPreviously = true;
    }

    public void Interact(PlayerController player)
    {
        _player = player;

        bool lowerDistCheck = Vector3.Distance(waitPoint1.position, player.transform.position) <
            Vector3.Distance(waitPoint2.position, player.transform.position);

        waitPts = lowerDistCheck ? 
            (waitPoint1.position, waitPoint2.position) : 
            (waitPoint2.position, waitPoint1.position);

        OnDoorInteract?.Invoke(this, new OnDoorInteractArgs(player));
    }

    /// <summary>
    /// The method called on this door when the player enters the planning stage
    /// </summary>
    /// <param name="player"></param>
    public void PlanStageStart(PlayerController player)
    {
        _player = player;
         player.transform.position = waitPts.Item1;
        // Make sure that the door opens in the right direction
        //rotateDegrees *= lowerDistCheck ? -1 : 1;
    }

    public void EnterDoor()
    {
        //OpenDoor();
        _player.transform.position = waitPts.Item2;
        _player = null;
        onEnter?.Invoke();
        openedPreviously = true;
    }

    public void CloseDoor()
    {
        transform.RotateAround(hinge.position, Vector3.up, -rotateDegrees);
    }

    public void OpenDoor()
    {
        transform.RotateAround(hinge.position, Vector3.up, rotateDegrees);
    }
}
