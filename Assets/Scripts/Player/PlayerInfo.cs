using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Info")]

public class PlayerInfo : ScriptableObject
{
    [SerializeField] public AmmoDictionary Ammo;
    [SerializeField] public float MaxHealth;
    [SerializeField] public float Health;
}
