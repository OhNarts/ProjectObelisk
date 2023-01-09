using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : Room
{
    public delegate void OnRoomFinishHandler(object source, EventArgs e);
    public event OnRoomFinishHandler OnRoomFinish;

    public delegate void OnRoomPlanStartHandler(object source, EventArgs e);
    public event OnRoomPlanStartHandler OnRoomPlanStart;

    [SerializeField] private List<EnemyController> enemies;

    private int aliveEnemyCount;

    void Awake()
    {
        base.InitializeRoom();
        aliveEnemyCount = enemies.Count;
        foreach (EnemyController enemy in enemies)
        {
            enemy.onEnemyDeath.AddListener(OnEnemyDeath);
        }
    }

    public void PlanRoom(PlayerController player, GameObject cameraHolder)
    {
        SetCameraPos(cameraHolder);
        doorAttemptedEnter.PlanStageStart(player);
        OnRoomPlanStart?.Invoke(this, EventArgs.Empty);
    }

    private void OnEnemyDeath(EnemyController enemy)
    {
        if (--aliveEnemyCount == 0)
        {
            RoomFinish();
        }
    }

    public override void Enter(PlayerController player, GameObject cameraHolder)
    {
        base.Enter(player, cameraHolder);
        foreach (EnemyController enemy in enemies)
        {
            enemy.Target = player.transform;
        }
    }

    private void RoomFinish()
    {
        OnRoomFinish?.Invoke(this, EventArgs.Empty);
    }

}
