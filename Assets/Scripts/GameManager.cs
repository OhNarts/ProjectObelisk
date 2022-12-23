using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private GameObject Player;

    private PlayerInfo _playerInfo;
    public PlayerInfo playerInfo
    {
        get
        {
            return _playerInfo;
        }
    }
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Player Died");
    }
}
