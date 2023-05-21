using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRestrictionArea : MonoBehaviour
{
    // TODO: Make this more efficient
    [SerializeField] private Collider _collider;
    [SerializeField] private List<AmmoType> _restrictedWeaponTypes;
    [SerializeField] private BuffRegion buffRegion;

    void Start() {
        _collider.enabled = false;
        GameManager.OnGameStateChanged += AreaEnableSwitch;
    }
    
    void OnDestroy(){
        GameManager.OnGameStateChanged -= AreaEnableSwitch;
    }


    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Weapon")) return;
        Weapon weapon = other.transform.root.GetComponent<Weapon>();
        if (buffRegion.restrictArea) {
            weapon.CanPlace = false;
            Debug.Log("Cannot place more than one weapon here.");
        }
        // if (_restrictedWeaponTypes.Contains(weapon.WeaponItem.AmmoType1)) {
        //     weapon.CanPlace = false;
        //     Debug.Log(weapon.WeaponItem.AmmoType1 + " type restricted in this area.");
        // }
    }

    void OnTriggerExit(Collider other) {
        Debug.Log("Collider exited.");
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
            Weapon weapon = other.transform.root.GetComponent<Weapon>();
            weapon.CanPlace = true;
            // if (_restrictedWeaponTypes.Contains(weapon.WeaponItem.AmmoType1) && !weapon.CanPlace) {
            //     weapon.CanPlace = true;
            // }
        }
    }

    // Ensures that the collider is only on in the plan stage
    private void AreaEnableSwitch(object sender, OnGameStateChangedArgs e) {
        _collider.enabled = e.NewState == GameState.Plan;
    }
 
 }
