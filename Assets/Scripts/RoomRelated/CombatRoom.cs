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

    public event EventHandler OnCombatStart;

    [SerializeField] private TransformGameObjectDictionary _enemySpawnPoints;
    [SerializeField] private NavMeshData _navMesh;
    [SerializeField] private GameObject _boundaryColliders;
    [SerializeField] private RoomReward _rewardObject;
    [SerializeField] private AmmoRewardDictionary _reward;
    private NavMeshDataInstance _navMeshInstance;
    private bool _roomCompleted; public bool RoomCompleted {get => _roomCompleted;}
    private List<EnemyController> _aliveEnemies;
    private List<Weapon> _droppedWeapons;
    private PlayerController _player;
    private GameObject _cameraHolder;
    private bool _planning;

    void OnEnable()
    {
        _navMesh.position = transform.position;
        _navMeshInstance = NavMesh.AddNavMeshData(_navMesh);
        _aliveEnemies = new List<EnemyController>();
        _droppedWeapons = new List<Weapon>();
        _boundaryColliders.SetActive(false);
        _roomCompleted = false;
        _player = null;
        _planning = false;
        _rewardObject.gameObject.SetActive(false);

        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDisable() {
        // Clean up nav mesh data
        NavMesh.RemoveNavMeshData(_navMeshInstance);
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    public void PlanRoom(PlayerController player, GameObject cameraHolder)
    {
        Reset();
        InitializeEnemies();
        _player = player;
        _cameraHolder = cameraHolder;
        SetCameraPos(cameraHolder);
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
            foreach (EnemyController enemy in _aliveEnemies) {
                enemy.Target = _player.transform;
            }
        }

        _player = null;
        _cameraHolder = null;
        OnCombatStart?.Invoke(this, EventArgs.Empty);
    }

    public void Reset() {
        // Remove alive enemies and place in new ones
        // That way their health resets
        while (_aliveEnemies.Count != 0) {
            var currentEnemy = _aliveEnemies[0];
            _aliveEnemies.RemoveAt(0);
            Destroy(currentEnemy.gameObject);
        }
        while (_droppedWeapons.Count != 0) {
            var currentWeapon = _droppedWeapons[0];
            _droppedWeapons.RemoveAt(0);
            Destroy(currentWeapon.gameObject);
        }
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
        _aliveEnemies.Remove(enemy);
        _droppedWeapons.Add(enemy.EquippedWeapon);
        if (_aliveEnemies.Count == 0) RoomFinish();
    }

    private void RoomFinish()
    {
        _roomCompleted = true;
        _player = null;
        GameManager.CurrentState = GameState.PostCombat;
        _rewardObject.gameObject.SetActive(true);
        _rewardObject.Reward = _reward;
        OnRoomFinish?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeEnemies() {
        foreach (Transform enemyTransform in _enemySpawnPoints.Keys)
        {
            GameObject enemyInstance = Instantiate(_enemySpawnPoints[enemyTransform],
            enemyTransform.position, Quaternion.identity);
            EnemyController enemyControllerInstance = enemyInstance.GetComponent<EnemyController>(); 
            enemyControllerInstance.onEnemyDeath.AddListener(OnEnemyDeath);
            _aliveEnemies.Add(enemyControllerInstance);
        }
    }

}
