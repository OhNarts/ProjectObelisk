using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public enum GameState { Plan, Infiltrate, PostInfiltrate, Pause }

public sealed class GameManager : MonoBehaviour
{
    private static readonly Object key = new Object();
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) { Debug.LogError("Game Manager was null"); }
            return instance;
        }
    }

    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject CameraHolder;

    private PlayerInfo playerInfo;
    public PlayerInfo PlayerInfo
    {
        get
        {
            return playerInfo;
        }
    }

    private GameState currState;
    
    private void Awake()
    {
        lock (key)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        
    }

    private void Update()
    {
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

    private void OnDoorInteract()
    {
        
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Player Died");
    }
}
