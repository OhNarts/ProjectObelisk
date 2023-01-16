using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// This file is here for all the dictionaries you want to be serializable

[Serializable]
public class DoorRoomDictionary : SerializableDictionary<Door, Room> {}

[Serializable]
public class AmmoDictionary : SerializableDictionary<AmmoType, int> {}

[Serializable]
public class AmmoUIDictionary : SerializableDictionary<AmmoType, AmmoSlotUI> {}
