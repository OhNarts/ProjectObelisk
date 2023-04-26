using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReward : MonoBehaviour, Interactable
{
    [SerializeField] private AmmoRewardDictionary _ammoReward; public AmmoRewardDictionary AmmoReward {get => _ammoReward;}
    [SerializeField] private Weapon[] _weaponRewards; public Weapon[] WeaponRewards {get => _weaponRewards;}
    [SerializeField] private float _healthReward;
    private string _roomName; public string RoomName {get => _roomName;
    set  {
        _roomName = value;
        ErrorCheck();
    }}

    void Interactable.Interact(PlayerController player)
    {
        foreach (var ammoType in _ammoReward.Keys) {
            // Check for the upper bound
            if (player.Ammo.ContainsKey(ammoType) && player.Ammo[ammoType] > _ammoReward[ammoType][1]) continue;
            player.RewardAmmo(ammoType, _ammoReward[ammoType][0]);
            Debug.Log("Player ammo = "+ PlayerState.Ammo[ammoType]);
            while (player.Ammo[ammoType] % _ammoReward[ammoType][0] != 0) {
                player.RewardAmmo(ammoType, 1);
            }
        }
        foreach (var weapon in _weaponRewards) {
            player.GiveWeapon(weapon);
        }
        player.HealthHandler.Heal(_healthReward);
        gameObject.SetActive(false);
    }
    
    private void ErrorCheck() {
        foreach (AmmoType ammoType in _ammoReward.Keys) {
            if (ammoType == AmmoType.NONE) continue;
            string errorMessage = null;
            var bounds = _ammoReward[ammoType];
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
