using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour, Interactable
{
    [SerializeField] float healAmount = 25;
    /// <summary>
    /// Logic for what happens when the player hits the interact button on something.
    /// In this case, calls the players' HealthHandler to Heal them if they are below maximum health.
    /// </summary>
    /// <param name="player"></param>
    public void Interact(PlayerController player) {
        //Check to see if the player is below max health
        if (player.HealthHandler.Health < player.HealthHandler.MaxHealth) {
            //Get the HealthHandler to heal the player
            player.HealthHandler.Heal(healAmount);
            //Get rid of medkit somehow
            GameObject.Destroy(this.gameObject);
        }
    }
}
