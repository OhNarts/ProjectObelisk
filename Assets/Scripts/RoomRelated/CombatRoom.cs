using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatRoom : Room
{
    public delegate void OnRoomFinishHandler(object source, EventArgs e);
    public event OnRoomFinishHandler OnRoomFinish;

    public delegate void OnRoomPlanStartHandler(object source, EventArgs e);
    public event OnRoomPlanStartHandler OnRoomPlanStart;

    [SerializeField] private List<EnemyController> enemies;
    [SerializeField] private NavMeshData _navMesh;
    private NavMeshDataInstance _navMeshInstance;


    private int aliveEnemyCount;

    void OnEnable()
    {
        base.InitializeRoom();

        _navMesh.position = transform.position;
        _navMeshInstance = NavMesh.AddNavMeshData(_navMesh);

        aliveEnemyCount = enemies.Count;
        foreach (EnemyController enemy in enemies)
        {
            enemy.onEnemyDeath.AddListener(OnEnemyDeath);
        }
    }

    void OnDisable() {
        // Clean up nav mesh data
        NavMesh.RemoveNavMeshData(_navMeshInstance);
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
            Debug.Log("Room Finished");
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
        doorAttemptedEnter.CloseDoor();
    }

    private void RoomFinish()
    {
        OnRoomFinish?.Invoke(this, EventArgs.Empty);
    }

}
