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

    private Vector3 velocity;

    // The point that the player should look at
    private Vector3 lookPt;

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

        _healthHandler.MaxHealth = PlayerInfo.Instance.MaxHealth;
        _healthHandler.Health = PlayerInfo.Instance.Health;
        followObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Replace these by subscribing to game manager stuff
        if (GameManager.Instance.CurrentState != GameState.Plan)
        {
            transform.LookAt(lookPt);
            _rb.velocity = velocity;
        } 
        if (GameManager.Instance.CurrentState != GameState.Combat) {
            if (_equippedWeapon != null) {
                _equippedWeapon.DropWeapon();
                PlayerInfo.Instance.AddToAmmo(_equippedWeapon.WeaponItem.AmmoType1, _equippedWeapon.AmmoAmount1);
                PlayerInfo.Instance.AddWeapon(_equippedWeapon.WeaponItem);
                Destroy(_equippedWeapon.gameObject);
                _equippedWeapon = null;
                PlayerInfo.Instance.CurrWeapon = _equippedWeapon;
            }
        }
        if (followObject != null) {
            var gotoPt = new Vector3(lookPt.x, lookPt.y + 5, lookPt.z);
            followObject.transform.position = lookPt;
        }
    }

    public void PlanStateStart()
    {
        _input.SwitchCurrentActionMap("Planning");
    }

    public void CombatStart(CallbackContext callback)
    {
        if (callback.canceled) return;
        _input.SwitchCurrentActionMap("Combat");
        OnCombatStart?.Invoke(this, EventArgs.Empty);
    }

    #region Health
    public void OnDeath()
    {
        _onPlayerDeath.Invoke();
    }

    public void OnPlayerHealthChange()
    {
        PlayerInfo.Instance.Health = _healthHandler.Health;
    }

    #endregion

    #region Movement
    public void Move(CallbackContext context)
    {
        // Rotates the input matrix to the camera's rotation & normalize
        Vector2 rawInput = context.ReadValue<Vector2>();
        Vector3 changedInput = new Vector3(rawInput.x, 0, rawInput.y);

        float matrixY = _camera.transform.parent.rotation.eulerAngles.y;

        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, matrixY, 0));
        var rotatedInput = matrix.MultiplyPoint3x4(changedInput);
        rotatedInput.Normalize();
        // Applies the velocity
        velocity = (rotatedInput * _speed);
    }

    public void Look(CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        var ray = _camera.ScreenPointToRay(rawInput);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, lookLayers))
        {
            groundMousePt = hit.point;
            lookPt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
    }
    #endregion

    #region Interactable/Weapon
    [SerializeField] private GameObject _uiCanvas;
    [SerializeField] private Transform _weaponPos;
    [SerializeField] private float _maxPickUpDistance;
    [SerializeField] private float interactableRadius;
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

        Collider[] colliders = Physics.OverlapSphere(groundMousePt, interactableRadius);

        // prioritizes weapons over interactables
        Interactable interactable = null;
        Debug.Log(colliders.Length);
        foreach(Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                Weapon wep = collider.transform.GetComponent<Weapon>();
        
                // Skip over weapons that already have an owner
                if (wep.Holder != null) { continue; }

                // If in the postCombat stage, then just add it to the weapons
                if (GameManager.Instance.CurrentState == GameState.PostCombat) {
                    // Add the amount of ammo the weapon had into the ammo dictionary
                    PlayerInfo.Instance.AddToAmmo(wep.WeaponItem.AmmoType1, wep.AmmoAmount1);
                    PlayerInfo.Instance.AddWeapon(wep.WeaponItem);
                    Destroy(wep.gameObject);
                    return;
                }

                if (_equippedWeapon != null)
                {
                    _equippedWeapon.DropWeapon();
                }
                wep.PickUpWeapon(gameObject, _weaponPos);
                _equippedWeapon = wep;
                PlayerInfo.Instance.CurrWeapon = wep;
                return;
            }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                Debug.Log(collider.transform.name);
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

            if (PlayerInfo.Instance.Ammo[item.AmmoType1] < item.AmmoCost1) return;

            GameObject Instance = Instantiate(item.gameObject);
            Weapon weapon = Instance.GetComponent<Weapon>(); 
            PlayerInfo.Instance.AddToAmmo(item.AmmoType1, -item.AmmoCost1);
            weapon.InitializeWeapon(item.AmmoCost1, item.AmmoCost2);
            followObject = Instance;
        } else if (context.canceled) {
            followObject = null;
        }
    }
    #endregion
}
