using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, Interactable
{
    [SerializeField] public UnityEvent onEnter;
    [SerializeField] public UnityEvent onInteract;
    [SerializeField] public UnityEventDoor onDoorInteract;

    // The point outside a door where the player waits before entering
    [SerializeField] private Transform waitPoint;
    public void Interact(PlayerController player)
    {
        onInteract?.Invoke();
        onDoorInteract?.Invoke(this);
    }

    /// <summary>
    /// The method called on this door when the player enters the planning stage
    /// </summary>
    /// <param name="player"></param>
    public void PlanStageStart(PlayerController player)
    {
        player.transform.position = waitPoint.position;

    }

    public void EnterDoor()
    {
        onEnter?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
