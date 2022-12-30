using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private HealthHandler healthHandler;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private UnityEvent _onPlayerDeath;
    [SerializeField] private Transform _aimPoint;
    private LayerMask lookLayers;

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
        LayerMask.GetMask("Shootable");

        currAmmo = new Dictionary<AmmoType, int>();
        currAmmo.Add(AmmoType.Pistol, 100);
        currAmmo.Add(AmmoType.Shotgun, 100);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(lookPt);
        //Look();
        //Move();
        //Shoot();
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    PickUp();
        //}
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
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var rotatedInput = matrix.MultiplyPoint3x4(changedInput);
        rotatedInput.Normalize();

        // Applies the velocity
        _rb.velocity = (rotatedInput * _speed);
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
    [SerializeField] private Dictionary<AmmoType, int> currAmmo;
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
        equippedWeapon.Fire1(currAmmo);
    }

    public void Fire2(CallbackContext context)
    {
        if (equippedWeapon == null) return;
        equippedWeapon.Fire2(currAmmo);
    }

    //private void Shoot()
    //{
    //    if (equippedWeapon == null) return;
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        equippedWeapon.Fire1(currAmmo);
    //    }
    //    else if (Input.GetButtonUp("Fire1"))
    //    {
    //        equippedWeapon.Fire1Stop(currAmmo);
    //    }

    //    if (Input.GetButtonDown("Fire2"))
    //    {
    //        equippedWeapon.Fire2(currAmmo);
    //    }
    //    else if (Input.GetButtonUp("Fire2"))
    //    {
    //        equippedWeapon.Fire2Stop(currAmmo);
    //    }
    //}

    //private void PickUp()
    //{
    //    var ray = _camera.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;

    //    if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _interactableMask)) return;

    //    Transform weapon = hit.transform.root;
    //    var wep = weapon.GetComponent<Weapon>();
    //    if (!(Vector3.Distance(weapon.position, transform.position) < _maxPickUpDistance
    //        && wep.holder == null)) return;

    //    if (equippedWeapon != null)
    //    {
    //        equippedWeapon.DropWeapon();
    //    }
    //    wep.PickUpWeapon(gameObject, _weaponPos);
    //    equippedWeapon = wep;
    //}

    public void Interact(CallbackContext context)
    {
        // returns if the position the mouse is over is too far to interact
        if (!(Vector3.Distance(groundMousePt, transform.position) < _maxPickUpDistance)) return;

        Collider[] colliders = Physics.OverlapSphere(groundMousePt, interactableRadius);
        Debug.Log(colliders.Length);

        // prioritizes weapons over interactables
        Interactable interactable = null;
        foreach(Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                var wep = collider.transform.GetComponent<Weapon>();
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
