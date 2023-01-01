using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// This file is here as a place to put all the unity events that need to be serializable


[Serializable]
public class UnityEventObjectObject : UnityEvent<System.Object, System.Object> { }

[Serializable]
public class UnityEventFloat : UnityEvent<float> { }

[Serializable]
public class UnityEventInt : UnityEvent<int> { }

[Serializable]
public class UnityEventDamage : UnityEvent<DamageInfo> { }

[Serializable]
public class UnityEventRoom : UnityEvent<Room> { }

[Serializable]
public class UnityEventDoor : UnityEvent<Door> { }

[Serializable]
public class UnityEventEnemy : UnityEvent<EnemyController> { }
