using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : ScriptableObject
{
    public Dictionary<AmmoType, int> PlayerAmmo;
    public float PlayerHealth;
}
