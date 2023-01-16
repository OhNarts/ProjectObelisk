using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Inventory/Weapon Item")]
public class WeaponItem : ScriptableObject
{
    public string WeaponName;
    public int AmmoCost1;
    public AmmoType AmmoType1;
    public int AmmoCost2;
    public AmmoType AmmoType2;
    public GameObject gameObject;
    public Sprite Sprite;
}
