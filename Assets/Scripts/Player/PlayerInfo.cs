using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Info")]

public class PlayerInfo : ScriptableSingleton<PlayerInfo>
{
    public AmmoDictionary Ammo;
    public float MaxHealth;
    public float Health;
    public HashSet<WeaponItem> Weapons;
    // public HashSet<Item> Inventory;

    // public void Initialize() {
    //     Inventory = new HashSet<Item>();
    // }

    // public void AddToInventory(Item item) {
    //     Inventory.Add(item);
    // }
}
