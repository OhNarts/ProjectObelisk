using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public enum GameState { Plan, Combat, PostCombat }

#region EventArgs
public class OnGameStateChangedArgs : EventArgs {
    private GameState _oldState; public GameState OldState{get => _oldState;}
    private GameState _newState; public GameState NewState{get => _newState;}
    private bool _triggeredByRevert; public bool TriggeredByRevert{get => _triggeredByRevert;}
    public OnGameStateChangedArgs(GameState oldState, GameState newState, bool TriggeredByRevert = false) {
        _oldState = oldState;
        _newState = newState;
        _triggeredByRevert = TriggeredByRevert;
    }
}

public class OnGamePauseChangeArgs : EventArgs {
    private bool _paused; public bool Paused {get => _paused;}
    public OnGamePauseChangeArgs(bool paused) {
        _paused = paused;
    }
}
#endregion

public sealed class GameManager : MonoBehaviour
{
    #region Singleton Stuff
    private static readonly object key = new object();
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) { Debug.LogError("Game Manager was null"); }
            return _instance;
        }
    }
    #endregion
    
    #region Events
    public static event EventHandler<OnGameStateChangedArgs> OnGameStateChanged;
    public static event EventHandler<OnGamePauseChangeArgs> OnGamePauseChange;
    #endregion
    
    [SerializeField] private GameState _currentState; 
    public static GameState CurrentState { 
        get => _instance._currentState;
        set {
            var oldState = _instance._currentState;
            _instance._currentState = value;
            OnGameStateChanged?.Invoke(_instance,
            new OnGameStateChangedArgs (oldState, _instance._currentState, _instance._revertTriggered));
            _instance._revertTriggered = false;
        } 
    }

    private bool _paused; public static bool Paused {get => _instance._paused;}
    public static void Pause() {
        Time.timeScale = 0;
        _instance._paused = true;
        OnGamePauseChange?.Invoke(_instance, new OnGamePauseChangeArgs(true));
    }

    public static void UnPause() {
        Time.timeScale = 1;
        _instance._paused = false;
        OnGamePauseChange?.Invoke(_instance, new OnGamePauseChangeArgs(false));
    }

    private bool _revertTriggered;
    private Level currLevel;

    private void Awake()
    {
        lock (key)
        {
            if (_instance == null)
            {
                _instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
        CurrentState = GameState.PostCombat;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        _revertTriggered = true;
        GameManager.CurrentState = GameState.PostCombat;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currLevel = GameObject.FindGameObjectsWithTag("Level")[0].GetComponent<Level>();
    }
}
