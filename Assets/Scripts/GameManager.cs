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
    private bool _triggeredByRevert; public bool TriggeredByRevert{get => _triggeredByRevert;}
    public OnGameStateChangedArgs(GameState oldState, GameState newState, bool TriggeredByRevert = false) {
        _oldState = oldState;
        _newState = newState;
        _triggeredByRevert = TriggeredByRevert;
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
    public static event EventHandler<OnGameStateChangedArgs> OnGameStateChanged;
    #endregion

    [SerializeField] private GameState _currentState; 
    public static GameState CurrentState { 
        get => instance._currentState;
        set {
            var oldState = instance._currentState;
            instance._currentState = value;
            OnGameStateChanged?.Invoke(instance,
            new OnGameStateChangedArgs (oldState, instance._currentState, instance._revertTriggered));
            instance._revertTriggered = false;
        } 
    }
    private bool _revertTriggered;
    private Level currLevel;

    private void Awake()
    {
        lock (key)
        {
            if (instance == null)
            {
                instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
        CurrentState = GameState.PostCombat;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        _revertTriggered = true;
        GameManager.CurrentState = GameState.PostCombat;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currLevel = GameObject.FindGameObjectsWithTag("Level")[0].GetComponent<Level>();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (currLevel.NextScene != null)
        {
            SceneManager.LoadScene(currLevel.NextScene.name, LoadSceneMode.Single);
        }
    }
}
