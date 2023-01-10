using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Info")]

public class PlayerInfo : ScriptableSingleton<PlayerInfo>
{
    [SerializeField] public AmmoDictionary Ammo;
    [SerializeField] public float MaxHealth;
    [SerializeField] public float Health;
    [SerializeField] public Weapon[] Inventory;
}
