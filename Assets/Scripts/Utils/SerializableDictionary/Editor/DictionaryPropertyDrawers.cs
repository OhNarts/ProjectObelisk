using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Add a custom property drawer for each dictionary you want to serialize
/// </summary>

[CustomPropertyDrawer(typeof(DoorRoomDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

