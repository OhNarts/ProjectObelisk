using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : ScriptableObject
{
    [SerializeField] public Dictionary<AmmoType, int> PlayerAmmo;
    [SerializeField] public float MaxPlayerHealth;
    [SerializeField] public float PlayerHealth;
}
