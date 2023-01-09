using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region Event args
public class OnDoorInteractArgs : EventArgs
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

    private Vector3 otherWaitPoint;
    private PlayerController player;
    private float rotateDegrees;
    private bool openedPreviously; public bool OpenedPreviously { get => openedPreviously; }

    void Awake()
    {
        player = null;
        rotateDegrees = 90;
        openedPreviously = true;
    }

    public void Interact(PlayerController player)
    {
        OnDoorInteract?.Invoke(this, new OnDoorInteractArgs(player));
    }

    /// <summary>
    /// The method called on this door when the player enters the planning stage
    /// </summary>
    /// <param name="player"></param>
    public void PlanStageStart(PlayerController player)
    {
        this.player = player;

        // We know that the door only has two waitpoints
        // Check which is closer and set the player's position to that one
        bool lowerDistCheck = Vector3.Distance(waitPoint1.position, player.transform.position) <
            Vector3.Distance(waitPoint2.position, player.transform.position);
        (Vector3, Vector3) waitPts = lowerDistCheck ? 
            (waitPoint1.position, waitPoint2.position) : 
            (waitPoint2.position, waitPoint1.position);
        player.transform.position = waitPts.Item1;
        otherWaitPoint = waitPts.Item2;
        // Make sure that the door opens in the right direction
        rotateDegrees *= lowerDistCheck ? -1 : 1;
    }

    public void EnterDoor()
    {
        OpenDoor();
        player.transform.position = otherWaitPoint;
        player = null;
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
