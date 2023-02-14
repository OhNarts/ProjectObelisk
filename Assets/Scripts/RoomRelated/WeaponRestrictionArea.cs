using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRestrictionArea : MonoBehaviour
{
    // TODO: Make this more efficient
    [SerializeField] private Collider _collider;
    [SerializeField] private List<AmmoType> _restrictedWeaponTypes;

    void Start() {
        _collider.enabled = false;
        GameManager.OnGameStateChanged += AreaEnableSwitch;
    }
    
    void OnDisable(){
        GameManager.OnGameStateChanged -= AreaEnableSwitch;
    }


    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Weapon")) return;
        Weapon weapon = other.transform.root.GetComponent<Weapon>();
        if (_restrictedWeaponTypes.Contains(weapon.WeaponItem.AmmoType1)) {
            weapon.CanPlace = false;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
            Weapon weapon = other.transform.root.GetComponent<Weapon>();
            if (_restrictedWeaponTypes.Contains(weapon.WeaponItem.AmmoType1) && !weapon.CanPlace) {
                weapon.CanPlace = true;
            }
        }
    }

    // Ensures that the collider is only on in the plan stage
    private void AreaEnableSwitch(object sender, OnGameStateChangedArgs e) {
        _collider.enabled = e.NewState == GameState.Plan;
    }
 
 }
