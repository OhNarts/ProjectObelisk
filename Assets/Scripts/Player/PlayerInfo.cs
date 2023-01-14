using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Info")]

public class PlayerInfo : ScriptableObject
{
    public AmmoDictionary Ammo;
    public float MaxHealth;
    public float Health;
    public HashSet<WeaponItem> Weapons;
    
    private static readonly object key = new object();
    private static PlayerInfo instance = null;
    public static PlayerInfo Instance
    {
        get 
        {
            lock (key)
            {
                if (instance == null)
                {
                    instance = Resources.Load<PlayerInfo>("PlayerInfo");
                }
            }
            return instance;
        }
    }
    

}
