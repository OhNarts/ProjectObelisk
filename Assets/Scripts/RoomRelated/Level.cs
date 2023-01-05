using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Room[] rooms;

    void Awake()
    {
        foreach (Room room in rooms)
        {
            room.OnRoomEnterAttempt += OnRoomEnterAttempt;
        }    
    }

    private void OnRoomEnterAttempt (object sender, OnRoomEnterAttemptArgs e)
    {

    }
}
