using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    public static readonly Object key = new Object();
    public static GameManager instance;
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
        //CameraHolder.transform.position = Player.transform.position;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

    public void OnPlayerDeath()
    {
        Debug.Log("Player Died");
    }
}
