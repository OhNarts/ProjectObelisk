using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    [SerializeField] private UnityEventRoom OnRoomFinish;
    [SerializeField] private List<EnemyController> enemies;
    [SerializeField] private List<Door> doors;
    [SerializeField] private Transform camHolderPos;

    // Change to increase the distance the camera can be from the room
    [SerializeField] private float camSize;

    private int aliveEnemyCount;

    // Start is called before the first frame update

    void Awake()
    {
        aliveEnemyCount = enemies.Count;
        foreach (EnemyController enemy in enemies)
        {
            enemy.HealthHandler.onDeath.AddListener(OnEnemyDeath);
        }
        foreach (Door door in doors)
        {

        }
    }

    private void OnEnemyDeath()
    {
        if (--aliveEnemyCount == 0)
        {
            RoomFinish();
        }
    }

    private void RoomFinish()
    {

    }

    

    public void Enter(PlayerController player)
    {
        foreach(EnemyController enemy in enemies)
        {
            enemy.Target = player.transform;
        }
    }
}
