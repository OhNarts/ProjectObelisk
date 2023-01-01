using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : Room
{
    [SerializeField] private UnityEventRoom onRoomFinish;
    [SerializeField] private List<EnemyController> enemies;
    private int aliveEnemyCount;

    private void Awake()
    {
        aliveEnemyCount = enemies.Count;
        foreach (EnemyController enemy in enemies)
        {
            enemy.onEnemyDeath.AddListener(OnEnemyDeath);
        }
    }

    private void OnEnemyDeath(EnemyController enemy)
    {
        if (--aliveEnemyCount == 0)
        {
            RoomFinish();
        }
    }

    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        foreach (EnemyController enemy in enemies)
        {
            enemy.Target = player.transform;
        }
    }

    private void RoomFinish()
    {
        onRoomFinish?.Invoke(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
