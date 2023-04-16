using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private HealthHandler _healthHandler; public HealthHandler HealthHandler { get => _healthHandler; }
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private UnityEvent _onPlayerDeath;
    [SerializeField] private PlayerInput _input;
    private LayerMask lookLayers;

    private Vector3 _velocity;

    // The point that the player should look at
    private Vector3 _lookPt;

    // The point on the ground that the mouse is over
    private Vector3 _groundMousePt;

    // The object that should follow the mouse pointer
    private Weapon _followWeapon;
    private string _currentActionMap;

    [Header("EXPOSED FOR DEBUG")]
    [SerializeField]private List<Weapon> _placedWeapons;

    private void Start()
    {
        lookLayers = LayerMask.GetMask("Ground") |
        LayerMask.GetMask("Weapon") |
        LayerMask.GetMask("Shootable") |
        LayerMask.GetMask("Interactable") |
        LayerMask.GetMask("Enemy");

        _healthHandler.MaxHealth = PlayerState.MaxHealth;
        _healthHandler.Health = PlayerState.Health;

        _followWeapon = null;
        _rolling = false;
        _lastRolled = -1;
        _placedWeapons = new List<Weapon>();

        PlayerState.Position = transform.position;
        PlayerState.OnPlayerStateRevert += RevertPlayer;
        GameManager.OnGameStateChanged += OnGameStateChange;
        GameManager.OnGamePauseChange += OnGamePauseChange;
    }

    private void OnDisable() {
        PlayerState.OnPlayerStateRevert -= RevertPlayer;
        GameManager.OnGameStateChanged -= OnGameStateChange;
        GameManager.OnGamePauseChange -= OnGamePauseChange;
    }
    
    void Update()
    {
        if (GameManager.Paused) return;
        if (GameManager.CurrentState != GameState.Plan)
        {
            transform.LookAt(_lookPt);
            if (!_rolling) _rb.velocity = _velocity;
        } 
        if (_followWeapon != null) {
            var gotoPt = new Vector3(_lookPt.x, _lookPt.y + .5f, _lookPt.z);
            _followWeapon.transform.position = gotoPt;
        }
    }

    public void CombatStart(CallbackContext callback)
    {
        if (!callback.started) return;
        SwitchActionMap("Combat");
        GameManager.CurrentState = GameState.Combat;
    }
    
    public void RevertPlayer(object sender, EventArgs e) {
        _healthHandler.MaxHealth = PlayerState.MaxHealth;
        _healthHandler.Health = PlayerState.Health;
        transform.position = PlayerState.Position;
        
        _equippedWeapon?.DropWeapon();
        _equippedWeapon = null;

        while (_placedWeapons.Count != 0) {
            Weapon currWeapon = _placedWeapons[0];
            _placedWeapons.Remove(currWeapon);
            currWeapon.OnWeaponDestroyed -= OnWeaponDestroyed;
            Destroy(currWeapon.gameObject);
        }
    }

    private void OnGameStateChange(object sender, OnGameStateChangedArgs e) {
        switch (e.NewState) {
            case GameState.Combat:
                SwitchActionMap("Combat");
                _rb.isKinematic = false;
                break;
            case GameState.Plan:
                SwitchActionMap("Planning");
                _rb.velocity = Vector3.zero;
                _rb.isKinematic = true;
                break;
            case GameState.PostCombat:
                SwitchActionMap("Combat");
                Debug.Log("Game Reverted");
                _rb.isKinematic = false;
                if (e.TriggeredByRevert) break;
                if (EquippedWeapon != null) 
                {
                    EquippedWeapon.DropWeapon();
                    PlayerState.AddToAmmo(EquippedWeapon.WeaponItem.AmmoType1, EquippedWeapon.AmmoAmount1);
                    PlayerState.AddWeapon(EquippedWeapon.WeaponItem);
                    Destroy(EquippedWeapon.gameObject);
                    EquippedWeapon = null;
                    PlayerState.CurrentWeapon = EquippedWeapon;
                }
                if (e.OldState != GameState.Combat)
                {
                    while (_placedWeapons.Count != 0) {
                        Weapon currWeapon = _placedWeapons[0];
                        _placedWeapons.Remove(currWeapon);
                        PlayerState.AddToAmmo(currWeapon.WeaponItem.AmmoType1, currWeapon.AmmoAmount1);
                        Destroy(currWeapon.gameObject);
                    }
                }
                _placedWeapons = new List<Weapon>();
                break;
        }
    }

    private void OnGamePauseChange (object sender, OnGamePauseChangeArgs e) {
        if (GameManager.Paused) {
            // Don't switch action maps here so that pause is never recorded
            _input.SwitchCurrentActionMap("Pause");
        } else {
            SwitchActionMap(_currentActionMap);
        }
    }

    public void CancelPlanState(CallbackContext callback) {
        if (!callback.started) return;
        if (GameManager.CurrentState == GameState.Plan) 
        {
            GameManager.CurrentState = GameState.PostCombat;
        }
    }

    public void SwitchActionMap(String actionMap) {
        _currentActionMap = actionMap;
        _input.SwitchCurrentActionMap(actionMap);
    }

    #region Health
    public void OnDeath()
    {
        PlayerState.RevertToLastRoom();
        _onPlayerDeath?.Invoke();
    }

    public void OnPlayerHealthChange()
    {
        PlayerState.Health = _healthHandler.Health;
    }
    #endregion

    #region Movement
    private bool _rolling = false;
    private float _lastRolled;
    [SerializeField] private float _rollDuration;
    [SerializeField] private float _rollSpeed;
    [SerializeField] private float _rollCooldown;
    [SerializeField] private float _rollScale;

    public void Move(CallbackContext context)
    {
        if (_rolling) return;
        // Rotates the input matrix to the camera's rotation & normalize
        Vector2 rawInput = context.ReadValue<Vector2>();
        Vector3 changedInput = new Vector3(rawInput.x, 0, rawInput.y);

        float matrixY = _camera.transform.parent.rotation.eulerAngles.y;

        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, matrixY, 0));
        var rotatedInput = matrix.MultiplyPoint3x4(changedInput);
        rotatedInput.Normalize();
        // Applies the velocity
        _velocity = (rotatedInput * _speed);
    }

    public void Look(CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        var ray = _camera.ScreenPointToRay(rawInput);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, lookLayers))
        {
            _groundMousePt = hit.point;
            _lookPt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
    }

    public void Roll(CallbackContext context) {
        if (!context.started || _rolling || 
        Time.fixedTime - _lastRolled < _rollCooldown) return;
        StartCoroutine(RollSequence());
    }

    public void Drop(CallbackContext context) {
        if (!context.started) return;
        if (EquippedWeapon != null) {
            EquippedWeapon.DropWeapon();
            EquippedWeapon = null;
            // PlayerState.CurrentWeapon = _equippedWeapon;
        }
    }
    public void Throw(CallbackContext context) {
        if (!context.started) return;
        if (EquippedWeapon != null) {
            EquippedWeapon.ThrowWeapon();
            EquippedWeapon = null;
        }
    }
    private IEnumerator RollSequence() {
        _lastRolled = Time.fixedTime;
        _rolling = true;
        _healthHandler.IsInvincible = true;
        Vector3 moveDir = _velocity.normalized;
        _rb.velocity = moveDir * _rollSpeed;
        transform.localScale = new Vector3( transform.localScale.x,
                                            transform.localScale.y * _rollScale,
                                            transform.localScale.z);
        yield return new WaitForSeconds(_rollDuration);
        transform.localScale = new Vector3( transform.localScale.x,
                                            transform.localScale.y / _rollScale,
                                            transform.localScale.z);
        _rb.velocity = _velocity;
        _rolling = false;
        _healthHandler.IsInvincible = false;
    }
    #endregion

    #region Interactable/Weapon
    [SerializeField] private GameObject _uiCanvas;
    [SerializeField] private Transform _weaponPos;
    [SerializeField] private float _maxPickUpDistance;
    [SerializeField] private float _interactableRadius;
    [SerializeField] private LayerMask _interactableMask;
    private Weapon _equippedWeapon;
    public Weapon EquippedWeapon {
        private set {
            _equippedWeapon = value;
            PlayerState.CurrentWeapon = _equippedWeapon;
        }
        get => _equippedWeapon;}

    [Header("Attack Properties")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRange;
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private float _damage;
    public AmmoDictionary Ammo {get => PlayerState.Ammo; }

    
    public void Fire1(CallbackContext context)
    {
      
        if (EquippedWeapon == null)  {
            Melee(context);
            return;
        }
        if (context.started) {
            Debug.Log("started");
            EquippedWeapon.Fire1Start(true);
        }
        else if (context.canceled)
        {
            Debug.Log("cancelled");
            EquippedWeapon.Fire1Stop(true);
            
        } else if (context.performed) {
            Debug.Log("held");
            EquippedWeapon.Fire1Held(true);
        }
    }
    public void Fire2(CallbackContext context)
    {
        if (EquippedWeapon == null) return;
        if (context.started)
        {
            EquippedWeapon.Fire2(true);
        }
        else if (context.canceled)
        {
            EquippedWeapon.Fire2Stop(true);
        }
    }

    //To visualize the hitbox for melee attack
    public void OnDrawGizmos() {
        if (_attackPoint is null) {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    }

    public void Melee(CallbackContext context) {
        if (!context.started) return;
        DamageInfo damageInfo = new DamageInfo() {
            damage = _damage,
            attacker = gameObject,
            knockbackValue = 4f
        };

        Debug.Log("melee");

        Collider[] colliders = Physics.OverlapSphere(_attackPoint.position, _attackRange, _attackMask);
        foreach(Collider collider in colliders) {
            Transform hitTransform = collider.transform.root;
            HealthHandler hitHealth = hitTransform.GetComponent<HealthHandler>();
            Debug.Log(hitHealth != null);
            if (hitHealth != null) {
                hitHealth.Damage(damageInfo);
                var enemy = collider.GetComponent<EnemyController>();
                if (enemy != null && !hitHealth.IsInvincible) {
                    enemy.Knockback(gameObject.transform.position, damageInfo.knockbackValue);
                }
            }
        }
    }

    public void Interact(CallbackContext context)
    {
        // make sure that this is only called once
        if (!context.started) return;

        List<Weapon> weaponsPointedAt = GetWeaponsPointedAt();
        List<Weapon> weaponsAround = GetWeaponsAround();

        if (weaponsPointedAt.Count != 0) {
            if ((Vector3.Distance(_groundMousePt, transform.position) < _maxPickUpDistance)) {
                InteractWithWeapons(weaponsPointedAt);
                return;
            }
        }

        if (weaponsAround.Count != 0) {
            InteractWithWeapons(weaponsAround);
            return;
        }


        Interactable interactable = GetInteractableNearPoint(transform.position);

        // returns if the position the mouse is over is too far to interact
        if (interactable == null && (Vector3.Distance(_groundMousePt, transform.position) < _maxPickUpDistance)) {
            interactable = GetInteractableNearPoint(_groundMousePt);
        }

        interactable?.Interact(this);
    }

    private Interactable GetInteractableNearPoint(Vector3 position) {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _interactableRadius);
        Interactable interactable = null;
        foreach(Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                interactable = collider.transform.GetComponent<Interactable>();
            }
        }
        return interactable;
    }
    #endregion

    # region Inventory

    public void RewardAmmo(AmmoType type, int amount) {
        PlayerState.AddToAmmo(type, amount);
    }

    public void GiveWeapon(Weapon weapon) {
        PlayerState.AddWeapon(weapon.WeaponItem);
    }

    /// <summary>
    /// Spawns a weapon into the world
    /// </summary>
    /// <param name="context"></param>
    public void AddToWorld(CallbackContext context) {
        if (context.started){   
            var graphicCaster = _uiCanvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> clickResults = new List<RaycastResult>();
            graphicCaster.Raycast(new PointerEventData(EventSystem.current) {
                position = Mouse.current.position.ReadValue()
            }, clickResults);

            if (clickResults.Count != 0) {
                InventorySlot chosenSlot = null;
                foreach(var result in clickResults) {
                    chosenSlot = result.gameObject.GetComponent<InventorySlot>();
                    if (chosenSlot != null) break;
                }
                if (chosenSlot == null) return;
                var item = chosenSlot.Weapon;

                if (PlayerState.Ammo[item.AmmoType1] < item.AmmoCost1) return;

                GameObject Instance = Instantiate(item.gameObject);
                _followWeapon = Instance.GetComponent<Weapon>();
                Rigidbody rb = _followWeapon.GetComponent<Rigidbody>();
                rb.freezeRotation = true;
                _placedWeapons.Add(_followWeapon);
                _followWeapon.OnWeaponDestroyed += OnWeaponDestroyed;
                PlayerState.AddToAmmo(item.AmmoType1, -item.AmmoCost1);
                _followWeapon.InitializeWeapon(item.AmmoCost1, item.AmmoCost2);
                _followWeapon.OnDrag();
            } else {
                List<Weapon> weaponsPointedAt = GetWeaponsPointedAt();
                if (weaponsPointedAt.Count != 0) {
                    _followWeapon = weaponsPointedAt[0];
                    _followWeapon.OnDrag();
                }
            }
        } else if (context.canceled) {
            if (_followWeapon != null && !_followWeapon.CanPlace) WeaponPlanRemove(_followWeapon);
            else _followWeapon.OnDrop();
            _followWeapon = null;
        }
    }

    /// <summary>
    /// Removes an already placed weapon during the planning state.
    /// </summary>
    /// <param name="context"></param>
    public void RemoveFromWorld(CallbackContext context) {
        if (!context.started) return;
        List<Weapon> wepsPointedAt = GetWeaponsPointedAt();
        foreach (Weapon weapon in wepsPointedAt) {
            if (_placedWeapons.Contains(weapon)) {
                _placedWeapons.Remove(weapon);
                weapon.OnWeaponDestroyed -= OnWeaponDestroyed;
                PlayerState.AddToAmmo(weapon.WeaponItem.AmmoType1,
                weapon.AmmoAmount1);
                if (weapon.isBuffed) {
                    weapon.buffRegion.restrictArea = false;
                }
                Destroy(weapon.gameObject);
            }
        }
    }

    private void OnWeaponDestroyed(object sender, EventArgs e) {
        _placedWeapons.Remove((Weapon)sender);
    }

    /// <summary>
    /// Helper method that correctly removes a weapon from the world that the player placed during the plan stage
    /// </summary>
    /// <param name="context"></param>
    private void WeaponPlanRemove(Weapon weapon) {
        if (_placedWeapons.Contains(weapon)) {
            _placedWeapons.Remove(weapon);
            weapon.OnWeaponDestroyed -= OnWeaponDestroyed;
        } else return;
        PlayerState.AddToAmmo(weapon.WeaponItem.AmmoType1, weapon.AmmoAmount1);
        PlayerState.AddWeapon(weapon.WeaponItem);
        Destroy(weapon.gameObject);
    }

    /// <summary>
    /// Helper method that gets a list of weapons that the player's cursor is over
    /// </summary>
    /// <returns>The list of weapons</returnsZ>
    private List<Weapon> GetWeaponsPointedAt() {
        Collider[] colliders = Physics.OverlapSphere(_groundMousePt, _interactableRadius);
        List<Weapon> weapons = new List<Weapon>();

        foreach (Collider collider in colliders) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
                weapons.Add(collider.transform.root.GetComponent<Weapon>());
            }
        }
        return weapons;
    }

    /// <summary>
    /// Helper method that gets a list of weapons that the player is around
    /// </summary>
    /// <returns>The list of weapons</returnsZ>
    private List<Weapon> GetWeaponsAround() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _interactableRadius);
        List<Weapon> weapons = new List<Weapon>();

        foreach (Collider collider in colliders) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
                weapons.Add(collider.transform.root.GetComponent<Weapon>());
            }
        }
        return weapons;
    }

    /// <summary>
    /// Helper method to interact with the weapons pointed at or nearby.
    /// </summary>
    /// <param name="weaponsList"></param>
    private void InteractWithWeapons(List<Weapon> weaponsList) {
        foreach (Weapon wep in weaponsList) {
            if (wep.Holder != null) { continue; }

            // If in the postCombat stage, then just add it to the weapons
            if (GameManager.CurrentState == GameState.PostCombat) {
                // Add the amount of ammo the weapon had into the ammo dictionary
                PlayerState.AddToAmmo(wep.WeaponItem.AmmoType1, wep.AmmoAmount1);
                PlayerState.AddWeapon(wep.WeaponItem);
                Destroy(wep.gameObject);
                return;
            }
            Rigidbody rb = wep.GetComponent<Rigidbody>();
            rb.freezeRotation = false;

            if (EquippedWeapon != null)
            {
                EquippedWeapon.DropWeapon();
            }
            wep.PickUpWeapon(gameObject, _weaponPos);
            
            EquippedWeapon = wep;
            // PlayerState.CurrentWeapon = wep;
            return;
        }
        
    }
    #endregion

    public void Pause(CallbackContext context) {
        //if (context.started) Application.Quit();
        if (!context.started) return;
        if (GameManager.Paused) GameManager.UnPause();
        else GameManager.Pause();
    }
}
