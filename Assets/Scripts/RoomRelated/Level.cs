using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] private Room[] _rooms; public Room[] Rooms { get => _rooms; }
    [SerializeField] private Scene nextScene; public Scene NextScene { get => nextScene; }

    void Awake()
    {
        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt += OnRoomEnterAttempt;
        }    
    }

    private void OnDisable()
    {
        foreach (Room room in _rooms)
        {
            room.OnRoomEnterAttempt -= OnRoomEnterAttempt;
        }
    }

    private void OnRoomEnterAttempt (object sender, EventArgs e)
    {
        if (GameManager.Instance.CurrentState == GameState.Combat || GameManager.Instance.CurrentState == GameState.Plan)
            return;
      
        if (sender.GetType() == typeof(CombatRoom))
        {
            ((CombatRoom)sender).PlanRoom(((OnRoomEnterAttemptArgs)e).Player);
        }
    }
}
