using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

