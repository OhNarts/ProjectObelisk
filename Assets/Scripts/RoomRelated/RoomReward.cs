using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReward : MonoBehaviour, Interactable
{
    private AmmoRewardDictionary _reward; public AmmoRewardDictionary Reward {get => _reward; set {
        _reward = value;
    }}
    void Interactable.Interact(PlayerController player)
    {
        Debug.Log("Interacted");
        foreach (var ammoType in _reward.Keys) {
            player.RewardAmmo(ammoType, _reward[ammoType][0]);
        }
        gameObject.SetActive(false);
    }
}
