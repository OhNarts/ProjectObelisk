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
    private PlayerController _player;
    private GameObject _cameraHolder;
    private bool _planning;

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
        _player = null;
        _planning = false;

        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDisable() {
        // Clean up nav mesh data
        NavMesh.RemoveNavMeshData(_navMeshInstance);
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    public void PlanRoom(PlayerController player, GameObject cameraHolder)
    {
        _player = player;
        _cameraHolder = cameraHolder;
        SetCameraPos(cameraHolder);
        _player.PlanStateStart();
        _doorAttemptedEnter.PlanStageStart(_player);
        _boundaryColliders.SetActive(true);
        _planning = true;
        GameManager.CurrentState = GameState.Plan;
        OnRoomPlanStart?.Invoke(this, EventArgs.Empty);
    }

    public void CombatEnter() { 
        base.Enter(_player, _cameraHolder);
        _boundaryColliders.SetActive(false);
        if (!_roomCompleted)
        {
            foreach (EnemyController enemy in _enemies)
            {
                enemy.Target = _player.transform;
            }
            //doorAttemptedEnter.CloseDoor();
        }

        _player = null;
        _cameraHolder = null;
    }

    private void OnGameStateChanged(object sender, EventArgs e) {
        OnGameStateChangedArgs args = (OnGameStateChangedArgs)e;
        if (_planning &&
            args.OldState == GameState.Plan && 
            args.NewState == GameState.Combat) {
                CombatEnter();
                _planning = false;
            } 
    }

    private void OnEnemyDeath(EnemyController enemy)
    {
        _deadEnemySet.Add(enemy);
        if (_deadEnemySet.Count == _enemies.Count)
        {
            RoomFinish();
        }
    }

    private void RoomFinish()
    {
        _roomCompleted = true;
        _player = null;
        GameManager.CurrentState = GameState.PostCombat;
        OnRoomFinish?.Invoke(this, EventArgs.Empty);
    }

}
