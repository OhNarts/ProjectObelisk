using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _inventorySlots;
    private int _currSlot;
    [SerializeField] private GameObject wepNotif;

    void Start() {
        InitializeSlots();
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
        wepNotif.SetActive(false);
    }

    void OnEnable() {
        PlayerState.OnPlayerWeaponsChanged += OnPlayerWeaponsChanged;
    }

    void OnDisable() {
        PlayerState.OnPlayerWeaponsChanged -= OnPlayerWeaponsChanged;
    }

    private void OnPlayerWeaponsChanged(object sender, EventArgs e) {
        AddWeaponSlot(((OnPlayerWeaponsChangedArgs)e).WeaponAdded);
    }

    private void AddWeaponSlot(WeaponItem weapon) {
        GameObject slot = _inventorySlots[_currSlot++];
        slot.GetComponent<InventorySlot>().Weapon = weapon;
        slot.SetActive(true);
        
        TextMeshProUGUI notifText = wepNotif.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        notifText.text = ChangeText(weapon.WeaponName);
        notifText.color = weapon.Color;
        wepNotif.SetActive(true);
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        if (e.RevertType == OnPlayerStateRevertArgs.PlayerRevertType.LevelStart) { 
            InitializeSlots();
        }
    }

    // TODO: Probably a more efficient way to do this
    private void InitializeSlots() {
        // Deactivate all the slots
        foreach(GameObject slot in _inventorySlots) {
            slot.SetActive(false);
        }
        _currSlot = 0;

        // Initialize the slots that shoudl be active
        foreach (var weapon in PlayerState.Weapons) {
            GameObject slot = _inventorySlots[_currSlot++];
            slot.GetComponent<InventorySlot>().Weapon = weapon;
            slot.SetActive(true);
        }
    }

    private string ChangeText(string weaponName) {
        string finalText;
        switch(weaponName) {
            case "Pistol": 
                finalText = "NEW WEAPON - Pistol!\n\nShoots one bullet at a time.\nAMMO: 10 ammo\nDAMAGE: Normal"; break;
            case "ShotGun": 
                finalText = "NEW WEAPON - Shotgun!\n\nShoots three bullets at a time.\nAMMO: 5 ammo\nDAMAGE: High"; break;
            case "SMG": 
                finalText = "NEW WEAPON - SMG!\n\nShoots bullets when mouse held.\nAMMO: 20 ammo\nDAMAGE: Low"; break;
            case "ShockTrap":
                finalText = "NEW GADGET - Shock Trap!\n\nStuns a nearby area of enemies when shot at.\nAMMO: 5 ammo\nDAMAGE: None"; break;
            case "Railgun": 
                finalText = "NEW WEAPON - Railgun!\n\nShoots one massive bullet that goes through walls.\nAMMO: 5 ammo\nDAMAGE: High"; break;
            case "DoubleBarrelShotgun": 
                finalText = "NEW WEAPON - Double Barrel Shotgun!\n\nShoots five bullets at a time.\nAMMO: 2 ammo\nDAMAGE: High"; break;
            default: finalText = "Pick Weapon"; break;
        }

        return finalText;
    }
}
