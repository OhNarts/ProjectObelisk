using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public enum GameState { Plan, Combat, PostCombat, Pause }

public sealed class GameManager : MonoBehaviour
{
    #region Singleton Stuff
    private static readonly object key = new object();
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

    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject CameraHolder;

    private GameState currentState; public GameState CurrentState { get => currentState; }
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


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currLevel = GameObject.FindGameObjectsWithTag("Level")[0].GetComponent<Level>();
        foreach (Room room in currLevel.Rooms)
        {
            if (room.GetType().Equals(typeof(CombatRoom)))
            {
                CombatRoom cr = (CombatRoom)room;
                cr.OnRoomFinish += OnRoomFinish;
                cr.OnRoomEnterAttempt += OnRoomEnterAttempt;
            }
        }

        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        player.InitializePlayer(playerInfo);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        playerInfo = new PlayerInfo()
        {
            PlayerAmmo = player.CurrentAmmo,
            PlayerHealth = player.HealthHandler.Health,
            MaxPlayerHealth = player.HealthHandler.MaxHealth
        };

        foreach (Room room in currLevel.Rooms)
        {
            if (room.GetType().Equals(typeof(CombatRoom)))
            {
                ((CombatRoom)room).OnRoomFinish -= OnRoomFinish;
            }
        }
        if (currLevel.NextScene != null)
        {
            SceneManager.LoadScene(currLevel.NextScene.name, LoadSceneMode.Single);
        }
    }

    private void OnRoomFinish(object sender, EventArgs e)
    {
        currentState = GameState.PostCombat;
    }

    private void OnRoomEnterAttempt(object sender, EventArgs e)
    {
        currentState = GameState.Plan;
        player.PlanStateStart();
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Player Died");
    }
}
