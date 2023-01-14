using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    public delegate void OnCombatStartHandler(object sender, EventArgs e);
    public event OnCombatStartHandler OnCombatStart;
    [SerializeField] private HealthHandler healthHandler; public HealthHandler HealthHandler { get => healthHandler; }
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private UnityEvent _onPlayerDeath;
    [SerializeField] private PlayerInput input;
    private LayerMask lookLayers;

    private Vector3 velocity;

    // The point that the player should look at
    private Vector3 lookPt;

    // The point on the ground that the mouse is over
    private Vector3 groundMousePt;

    private void Start()
    {
        lookLayers = LayerMask.GetMask("Ground") |
        LayerMask.GetMask("Weapon") |
        LayerMask.GetMask("Shootable") |
        LayerMask.GetMask("Interactable");

        // currAmmo = PlayerInfo.instance.Ammo;
        healthHandler.MaxHealth = PlayerInfo.Instance.MaxHealth;
        healthHandler.Health = PlayerInfo.Instance.Health;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(lookPt);
        _rb.velocity = velocity;
    }

    public void PlanStateStart()
    {
        input.SwitchCurrentActionMap("Planning");
    }

    public void CombatStart(CallbackContext callback)
    {
        if (callback.canceled) return;
        input.SwitchCurrentActionMap("Combat");
        OnCombatStart?.Invoke(this, EventArgs.Empty);
    }

    #region Health
    public void OnDeath()
    {
        _onPlayerDeath.Invoke();
    }

    public void OnPlayerHealthChange()
    {
        PlayerInfo.Instance.Health = healthHandler.Health;
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
    [SerializeField] private Transform _weaponPos;
    [SerializeField] private float _maxPickUpDistance;
    [SerializeField] private float interactableRadius;
    [SerializeField] private LayerMask _interactableMask;
    // [SerializeField] private AmmoDictionary currAmmo; public AmmoDictionary CurrentAmmo { get => currAmmo; }
    private Weapon equippedWeapon;
    public Weapon EquippedWeapon
    {
        get
        {
            return equippedWeapon;
        }
    }

    public void Fire1(CallbackContext context)
    {
        if (equippedWeapon == null) return;
        if (context.started)
        {
            equippedWeapon.Fire1(true);
        } else if (context.canceled)
        {
            equippedWeapon.Fire1Stop(true);
        }
    }

    public void Fire2(CallbackContext context)
    {
        if (equippedWeapon == null) return;
        if (context.started)
        {
            equippedWeapon.Fire2(true);
        }
        else if (context.canceled)
        {
            equippedWeapon.Fire2Stop(true);
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
                    PlayerInfo.Instance.Ammo[wep.WeaponItem.AmmoType1] += wep.AmmoAmount1;
                    PlayerInfo.Instance.Weapons.Add(wep.WeaponItem);
                    Destroy(wep.gameObject);
                    return;
                }

                if (equippedWeapon != null)
                {
                    equippedWeapon.DropWeapon();
                }
                wep.PickUpWeapon(gameObject, _weaponPos);
                equippedWeapon = wep;
                return;
            }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                interactable = collider.transform.GetComponent<Interactable>();
            }
        }
        interactable?.Interact(this);
    }

    public void AddToWorld(CallbackContext context) {
        if (!context.started) return;
        WeaponItem item = null;
        // For now just get the first item in the list
        // Will change once UI works
        foreach (var wep in PlayerInfo.Instance.Weapons) {
            item = wep;
            break;
        }
        if (item == null) return;
        GameObject Instance = Instantiate(item.gameObject);
        Weapon weapon = Instance.GetComponent<Weapon>(); 
        PlayerInfo.Instance.Ammo[item.AmmoType1] -= item.AmmoCost1;
        weapon.InitializeWeapon(item.AmmoCost1, item.AmmoCost2);

    }
    #endregion
}
