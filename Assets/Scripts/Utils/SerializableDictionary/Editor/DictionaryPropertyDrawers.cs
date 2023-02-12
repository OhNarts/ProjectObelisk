using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Add a custom property drawer for each dictionary you want to serialize
/// </summary>

[CustomPropertyDrawer(typeof(AmmoRewardDictionary))]
[CustomPropertyDrawer(typeof(AmmoTypeFloatDictionary))]
[CustomPropertyDrawer(typeof(TransformGameObjectDictionary))]
[CustomPropertyDrawer(typeof(WeaponHashSet))]
[CustomPropertyDrawer(typeof(DoorRoomDictionary))]
[CustomPropertyDrawer(typeof(AmmoDictionary))]
[CustomPropertyDrawer(typeof(AmmoUIDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

