using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private HealthHandler healthHandler; public HealthHandler HealthHandler { get => healthHandler; }
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private UnityEvent _onPlayerDeath;
    [SerializeField] private Transform _aimPoint;
    [SerializeField] private PlayerInput input;
    private LayerMask lookLayers;

    private Vector3 velocity;

    // The point that the player should look at
    private Vector3 lookPt;

    // The point on the ground that the mouse is over
    private Vector3 groundMousePt;
    
    public void InitializePlayer(PlayerInfo info)
    {
        currAmmo = info.PlayerAmmo;
        healthHandler.MaxHealth = info.MaxPlayerHealth;
        healthHandler.Health = info.PlayerHealth;
        _aimPoint.parent = null;
    }

    private void Start()
    {
        lookLayers = LayerMask.GetMask("Ground") |
        LayerMask.GetMask("Weapon") |
        LayerMask.GetMask("Shootable") |
        LayerMask.GetMask("Interactable");

        currAmmo = new Dictionary<AmmoType, int>();
        currAmmo.Add(AmmoType.Pistol, 100);
        currAmmo.Add(AmmoType.Shotgun, 100);
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

    public void CombatStart()
    {
        input.SwitchCurrentActionMap("Combat");
    }

    public void PlanStateEnd()
    {

    }

    #region Health
    public void OnDeath()
    {
        _onPlayerDeath.Invoke();
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
    [SerializeField] private Dictionary<AmmoType, int> currAmmo; public Dictionary<AmmoType, int> CurrentAmmo { get => currAmmo; }
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
            equippedWeapon.Fire1(currAmmo);
        } else if (context.canceled)
        {
            equippedWeapon.Fire1Stop(currAmmo);
        }
    }

    public void Fire2(CallbackContext context)
    {
        if (equippedWeapon == null) return;
        if (context.started)
        {
            equippedWeapon.Fire2(currAmmo);
        }
        else if (context.canceled)
        {
            equippedWeapon.Fire2Stop(currAmmo);
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
                var wep = collider.transform.GetComponent<Weapon>();
                if (wep.holder != null) { continue; }
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


    #endregion

    #region Debug
    public void OnPlayerHit()
    {
        Debug.Log("Ouch");
    }
    #endregion
}
