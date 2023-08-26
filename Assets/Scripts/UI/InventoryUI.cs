using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _inventorySlots;
    private int _currSlot;
    [SerializeField] private GameObject wepNotif;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private TextMeshProUGUI ammo;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private Image wepSprite;

    private Dictionary<string, int> dict = new Dictionary<string, int>() {
        {"Pistol", 0}, {"ShotGun", 1}, {"SMG", 2},
        {"ShockTrap", 3}, {"Railgun", 4}, {"DoubleBarrelShotgun", 5}
    };

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
        
        title.text = "NEW ITEM - " + (weapon.WeaponName == "DoubleBarrelShotgun"
            ? "Double Barrel Shotgun" : weapon.WeaponName) + "!";
        title.color = weapon.Color;
        string[] valueArray = ChangeText(weapon.WeaponName);

        desc.text = valueArray[0]; desc.color = weapon.Color;
        ammo.text = "AMMO: " + weapon.AmmoCost1; ammo.color = weapon.Color;
        damage.text = "DAMAGE: " + valueArray[1]; damage.color = weapon.Color;
        wepSprite.sprite = weapon.Sprite; wepSprite.color = weapon.Color;
        
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

    private string[] ChangeText(string weaponName) {
        string[] descriptions = {"Shoots one bullet at a time.", "Shoots three bullets at a time.", "Shoots bullets when mouse held.",
            "Stuns a nearby area of enemies when shot at.",
            "Shoots one massive bullet that goes through walls. Mouse needs to be held for a while and released to fire.",
            "Shoots five bullets at a time."};
        string[] dmgs = {"Normal", "High", "Low", "None", "High", "High"};

        int i = dict[weaponName];

        return new string[] {descriptions[i], dmgs[i]};
    }
}
