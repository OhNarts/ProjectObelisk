using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _inventorySlots;
    private int _currSlot;
    void Start() {
        InitializeSlots();
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
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
}
