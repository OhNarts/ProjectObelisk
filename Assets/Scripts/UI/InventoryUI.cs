using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    // Very hacky way of doing this
    [SerializeField] private GameObject[] _inventorySlots;
    private int _currSlot;
    void Awake() {
        PlayerInfo.Instance.OnPlayerWeaponsChanged += OnPlayerWeaponsChanged;
        foreach(GameObject slot in _inventorySlots) {
            slot.SetActive(false);
        }
        _currSlot = 0;
    }

    void OnDisable() {
        PlayerInfo.Instance.OnPlayerWeaponsChanged -= OnPlayerWeaponsChanged;
    }

    private void OnPlayerWeaponsChanged(object sender, EventArgs e) {
        AddWeaponSlot(((OnPlayerWeaponsChangedArgs)e).WeaponAdded);
    }

    private void AddWeaponSlot(WeaponItem weapon) {
        GameObject slot = _inventorySlots[_currSlot++];
        slot.GetComponent<InventorySlot>().Weapon = weapon;
        slot.SetActive(true);
    }
}
