using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySerializable : MonoBehaviour
{
    [System.Serializable] public struct EnemyData
    {
        public string enemyType;
        public bool needsCover;
        public float distToAttack;
        public Weapon weapon; public Weapon EquippedWeapon {get => weapon;}
        public Transform equipPos;
    }
}
