using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public enum GameState { Plan, Combat, PostCombat, Pause }

#region EventArgs
public class OnGameStateChangedArgs : EventArgs {
    private GameState _oldState; public GameState OldState{get => _oldState;}
    private GameState _newState; public GameState NewState{get => _newState;}
    public OnGameStateChangedArgs(GameState oldState, GameState newState) {
        _oldState = oldState;
        _newState = newState;
    }
}
#endregion

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
    
    #region Events
    public delegate void OnGameStateChangedHandler(object sender, EventArgs e);
    public event OnGameStateChangedHandler OnGameStateChanged;
    #endregion

    [SerializeField] private PlayerController _player; public static PlayerController Player {get => instance._player; }
    [SerializeField] private GameObject _cameraHolder; public static GameObject CameraHolder {get => instance._cameraHolder;}
    [SerializeField] private GameState _currentState; 
    public static GameState CurrentState { 
        get => instance._currentState;
        set {
            var oldState = instance._currentState;
            instance._currentState = value;
            instance.OnGameStateChanged?.Invoke(instance,
            new OnGameStateChangedArgs (oldState, instance._currentState));
        } 
    }
    private Level currLevel;
    private CombatRoom room;

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
        _currentState = GameState.PostCombat;
        //PlayerState.Instance.Reset();
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

        _player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        _player.OnCombatStart += (object sender, EventArgs e) =>
        {
            // _currentState = GameState.Combat;
            CurrentState = GameState.Combat;
            room.Enter(_player, _cameraHolder);

        };
    }

    private void OnSceneUnloaded(Scene scene)
    {
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
        // _currentState = GameState.PostCombat;
        //ChangeState(GameState.PostCombat);
        CurrentState = GameState.PostCombat;
    }

    private void OnRoomEnterAttempt(object sender, EventArgs e)
    {
        // _currentState = GameState.Plan;
        //ChangeState(GameState.Plan);
        CurrentState = GameState.Plan;
        room = (CombatRoom)sender;
        _player.PlanStateStart();
        room.OnRoomEnterAttempt -= OnRoomEnterAttempt;
    }

    // public void ChangeState(GameState state) {
    //     GameState oldState = _currentState;
    //     _currentState = state;
    //     OnGameStateChanged?.Invoke(this,
    //     new OnGameStateChangedArgs(oldState, _currentState));
    // }
}
