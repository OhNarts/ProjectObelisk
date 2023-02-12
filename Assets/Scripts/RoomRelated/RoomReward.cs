using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReward : MonoBehaviour, Interactable
{
    [SerializeField] private AmmoRewardDictionary _reward; public AmmoRewardDictionary Reward {get => _reward;}
    private string _roomName; public string RoomName {get => _roomName;
    set  {
        _roomName = value;
        ErrorCheck();
    }}

    void Interactable.Interact(PlayerController player)
    {
        foreach (var ammoType in _reward.Keys) {
            // Check for the upper bound
            if (player.Ammo[ammoType] > _reward[ammoType][1]) continue;
            player.RewardAmmo(ammoType, _reward[ammoType][0]);
            while (player.Ammo[ammoType] % _reward[ammoType][0] != 0) {
                player.RewardAmmo(ammoType, 1);
            }
        }
        gameObject.SetActive(false);
    }
    
    private void ErrorCheck() {
        foreach (AmmoType ammoType in _reward.Keys) {
            if (ammoType == AmmoType.NONE) continue;
            string errorMessage = null;
            var bounds = _reward[ammoType];
            if (bounds.Count != 2 && bounds.Count != 0) {
                errorMessage = "Ammo Type " + ammoType.ToString() + " does not have both upper and lower bounds.";
            } else if (bounds[1] <= bounds[0]){
                errorMessage = "The bounds of " + ammoType.ToString() + " are backwards.";
            }else if (bounds[1] % bounds[0] != 0) {
                errorMessage = "The bounds of " + ammoType.ToString() + " are not divisible.";
            }
            
            if (errorMessage != null) {
                Debug.LogError ("Error in " + _roomName + ". " + errorMessage);
            }
        }
    }
}
