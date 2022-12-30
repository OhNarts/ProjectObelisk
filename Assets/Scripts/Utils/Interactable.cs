using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    /// <summary>
    /// Logic for what happens when the player hits the interact button on something.
    /// Assumes that the player is the only one that is going to interact.
    /// </summary>
    /// <param name="player"></param>
    abstract public void Interact(PlayerController player);
}
