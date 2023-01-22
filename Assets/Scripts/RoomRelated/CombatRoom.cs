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

    [SerializeField] private List<EnemyController> _enemies;
    [SerializeField] private NavMeshData _navMesh;
    [SerializeField] private GameObject _boundaryColliders;
    private NavMeshDataInstance _navMeshInstance;
    private bool _roomCompleted; public bool RoomCompleted {get => _roomCompleted;}
    private HashSet<EnemyController> _deadEnemySet;

    private int aliveEnemyCount;

    void OnEnable()
    {
        //base.InitializeRoom();

        _navMesh.position = transform.position;
        _navMeshInstance = NavMesh.AddNavMeshData(_navMesh);

        aliveEnemyCount = _enemies.Count;
        foreach (EnemyController enemy in _enemies)
        {
            enemy.onEnemyDeath.AddListener(OnEnemyDeath);
        }
        _boundaryColliders.SetActive(false);
        _roomCompleted = false;
        _deadEnemySet = new HashSet<EnemyController>();
    }

    void OnDisable() {
        // Clean up nav mesh data
        //base.DeInitializeRoom();
        NavMesh.RemoveNavMeshData(_navMeshInstance);
    }

    public void PlanRoom(PlayerController player, GameObject cameraHolder)
    {
        SetCameraPos(cameraHolder);
        _doorAttemptedEnter.PlanStageStart(player);
        _boundaryColliders.SetActive(true);
        //GameManager.CurrentState = GameState.Plan;
        OnRoomPlanStart?.Invoke(this, EventArgs.Empty);
    }

    private void OnEnemyDeath(EnemyController enemy)
    {
        _deadEnemySet.Add(enemy);
        if (_deadEnemySet.Count == _enemies.Count)
        {
            RoomFinish();
        }
    }

    public override void Enter(PlayerController player, GameObject cameraHolder)
    {
        base.Enter(player, cameraHolder);
        _boundaryColliders.SetActive(false);
        if (!_roomCompleted)
        {
            foreach (EnemyController enemy in _enemies)
            {
                enemy.Target = player.transform;
            }
            //doorAttemptedEnter.CloseDoor();
        }
    }

    private void RoomFinish()
    {
        _roomCompleted = true;
        //GameManager.CurrentState = GameState.PostCombat;
        OnRoomFinish?.Invoke(this, EventArgs.Empty);
    }

}
