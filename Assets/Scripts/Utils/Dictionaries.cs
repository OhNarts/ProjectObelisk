using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// This file is here for all the dictionaries you want to be serializable

[Serializable]
public class AmmoTypeFloatDictionary : SerializableDictionary<AmmoType, float> {}

[Serializable]
public class IntListStorage : SerializableDictionary.Storage<List<int>> {}

[Serializable]
public class AmmoRewardDictionary : SerializableDictionary<AmmoType, List<int>, IntListStorage> {}

[Serializable]
public class DoorRoomDictionary : SerializableDictionary<Door, Room> {}

[Serializable]
public class AmmoDictionary : SerializableDictionary<AmmoType, int> { 
    public AmmoDictionary CreateCopy() {
        AmmoDictionary copy = new AmmoDictionary();
        foreach (var key in Keys) {
            copy.Add(key, this[key]);
        }
        return copy;
    }
}

[Serializable]
public class AmmoUIDictionary : SerializableDictionary<AmmoType, AmmoSlotUI> {}

[Serializable]
public class TransformGameObjectDictionary : SerializableDictionary<Transform, GameObject> {}

[Serializable]
public class WeaponHashSet : SerializableHashSet<WeaponItem> { 
    public WeaponHashSet CreateCopy() {
        WeaponHashSet copy = new WeaponHashSet();
        foreach (var weapon in this) {
            copy.Add(weapon);
        }
        return copy;
    }
}

[Serializable]
public class TransformWeaponDictionary : SerializableDictionary<Transform, Weapon> {}