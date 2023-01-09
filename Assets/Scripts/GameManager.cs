using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public enum GameState { Plan, Infiltrate, PostInfiltrate, Pause }

public sealed class GameManager : MonoBehaviour
{
    #region Singleton Stuff
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
    #endregion

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

    private GameState _currState; public GameState CurrentState { get => _currState; }
    private Level currLevel;
    
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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void Update()
    {
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //
        Level currLevel = GameObject.FindGameObjectsWithTag("Level")[0].GetComponent<Level>() ;
        foreach (Room room in currLevel.Rooms)
        {
            if (room.GetType().Equals(typeof(CombatRoom)))
            {
                
            }
        }

    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

    public void OnPlayerDeath()
    {
        Debug.Log("Player Died");
    }
}
