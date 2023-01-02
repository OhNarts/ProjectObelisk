using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region Event args
public class OnDoorInteractArgs
{
    public PlayerController Player { get; }
    public OnDoorInteractArgs(PlayerController player) { Player = player; }
}

#endregion

[Serializable]
public class Door : MonoBehaviour, Interactable
{
    [SerializeField] public UnityEvent onEnter;

    public delegate void OnDoorInteractHandler(object sender, OnDoorInteractArgs args);
    public event OnDoorInteractHandler OnDoorInteract;

    // The point outside a door where the player waits before entering
    [SerializeField] private Transform[] waitPoints;

    private bool opened;
    private PlayerController player;

    void Awake()
    {
        opened = false;
        player = null;
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
        //this.player.transform.position = waitPoint.position;

    }

    public void EnterDoor()
    {

        player = null;

        onEnter?.Invoke();
    }
}
