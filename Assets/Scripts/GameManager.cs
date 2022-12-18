using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private GameObject Player;

    public static readonly playerDataStruct playerData;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}

public struct playerDataStruct 
{
    float currHealth;
    float maxHealth;
    Dictionary<AmmoType, int> currAmmo;
}
