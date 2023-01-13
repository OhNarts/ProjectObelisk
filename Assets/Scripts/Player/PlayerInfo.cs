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
    public HashSet<Item> Inventory;

    public void Initialize() {
        Inventory = new HashSet<Item>();
    }

    public void AddToInventory(Item item) {
        // // Return if the inventory is too full
        // if (currIndex >= Inventory.Length) return;
        // Inventory[currIndex++] = item;
        Inventory.Add(item);
    }
}
