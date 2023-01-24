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
    public delegate void OnCombatStartHandler(object sender, EventArgs e);
    public event OnCombatStartHandler OnCombatStart;
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
    private Vector3 groundMousePt;

    // The object that should follow the mouse pointer
    private GameObject followObject;

    private void Start()
    {
        lookLayers = LayerMask.GetMask("Ground") |
        LayerMask.GetMask("Weapon") |
        LayerMask.GetMask("Shootable") |
        LayerMask.GetMask("Interactable");

        _healthHandler.MaxHealth = PlayerState.MaxHealth;
        _healthHandler.Health = PlayerState.Health;

        PlayerState.OnPlayerStateRevert += RevertPlayer;

        followObject = null;
        _rolling = false;
        _lastRolled = -1;
    }

    private void OnDisable() {
        PlayerState.OnPlayerStateRevert -= RevertPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Replace these by subscribing to game manager stuff
        if (GameManager.CurrentState != GameState.Plan)
        {
            transform.LookAt(_lookPt);
            if (!_rolling) _rb.velocity = _velocity;
        } 
        if (GameManager.CurrentState != GameState.Combat) {
            if (_equippedWeapon != null) {
                _equippedWeapon.DropWeapon();
                PlayerState.AddToAmmo(_equippedWeapon.WeaponItem.AmmoType1, _equippedWeapon.AmmoAmount1);
                PlayerState.AddWeapon(_equippedWeapon.WeaponItem);
                Destroy(_equippedWeapon.gameObject);
                _equippedWeapon = null;
                PlayerState.CurrentWeapon = _equippedWeapon;
            }
        }
        if (followObject != null) {
            var gotoPt = new Vector3(_lookPt.x, _lookPt.y + 5, _lookPt.z);
            followObject.transform.position = _lookPt;
        }
    }

    public void PlanStateStart()
    {
        _input.SwitchCurrentActionMap("Planning");
    }

    public void CombatStart(CallbackContext callback)
    {
        if (!callback.started) return;
        _input.SwitchCurrentActionMap("Combat");
        GameManager.CurrentState = GameState.Combat;
        //OnCombatStart?.Invoke(this, EventArgs.Empty);
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

    public void RevertPlayer(object sender, EventArgs e) {
        _healthHandler.MaxHealth = PlayerState.MaxHealth;
        _healthHandler.Health = PlayerState.Health;
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
            groundMousePt = hit.point;
            _lookPt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
    }

    public void Roll(CallbackContext context) {
        if (!context.started || _rolling || 
        Time.fixedTime - _lastRolled < _rollCooldown) return;
        StartCoroutine(RollSequence());
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
    private Weapon _equippedWeapon; public Weapon EquippedWeapon {get => _equippedWeapon;}
    

    public void Fire1(CallbackContext context)
    {
        if (_equippedWeapon == null) return;
        if (context.started)
        {
            _equippedWeapon.Fire1(true);
        } else if (context.canceled)
        {
            _equippedWeapon.Fire1Stop(true);
        }
    }

    public void Fire2(CallbackContext context)
    {
        if (_equippedWeapon == null) return;
        if (context.started)
        {
            _equippedWeapon.Fire2(true);
        }
        else if (context.canceled)
        {
            _equippedWeapon.Fire2Stop(true);
        }
    }

    public void Interact(CallbackContext context)
    {
        // make sure that this is only called once
        if (!context.started) return;
        // returns if the position the mouse is over is too far to interact
        if (!(Vector3.Distance(groundMousePt, transform.position) < _maxPickUpDistance)) return;

        Collider[] colliders = Physics.OverlapSphere(groundMousePt, _interactableRadius);

        // prioritizes weapons over interactables
        Interactable interactable = null;
        foreach(Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                Weapon wep = collider.transform.GetComponent<Weapon>();
        
                // Skip over weapons that already have an owner
                if (wep.Holder != null) { continue; }

                // If in the postCombat stage, then just add it to the weapons
                if (GameManager.CurrentState == GameState.PostCombat) {
                    // Add the amount of ammo the weapon had into the ammo dictionary
                    PlayerState.AddToAmmo(wep.WeaponItem.AmmoType1, wep.AmmoAmount1);
                    PlayerState.AddWeapon(wep.WeaponItem);
                    Destroy(wep.gameObject);
                    return;
                }

                if (_equippedWeapon != null)
                {
                    _equippedWeapon.DropWeapon();
                }
                wep.PickUpWeapon(gameObject, _weaponPos);
                _equippedWeapon = wep;
                PlayerState.CurrentWeapon = wep;
                return;
            }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                interactable = collider.transform.GetComponent<Interactable>();
            }
        }
        interactable?.Interact(this);
    }
    #endregion

    # region Inventory

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
            if (clickResults.Count == 0) return;
            InventorySlot chosenSlot = null;
            foreach(var result in clickResults) {
                chosenSlot = result.gameObject.GetComponent<InventorySlot>();
                if (chosenSlot != null) break;
            }
            if (chosenSlot == null) return;
            var item = chosenSlot.Weapon;

            if (PlayerState.Ammo[item.AmmoType1] < item.AmmoCost1) return;

            GameObject Instance = Instantiate(item.gameObject);
            Weapon weapon = Instance.GetComponent<Weapon>(); 
            PlayerState.AddToAmmo(item.AmmoType1, -item.AmmoCost1);
            weapon.InitializeWeapon(item.AmmoCost1, item.AmmoCost2);
            followObject = Instance;
        } else if (context.canceled) {
            followObject = null;
        }
    }
    #endregion

    public void Quit(CallbackContext context) {
        if (context.started) Application.Quit();
    }
}
