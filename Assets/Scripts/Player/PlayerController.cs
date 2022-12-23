using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private UnityEvent _onPlayerDeath;
    [SerializeField] private Transform _aimPoint;
    private Vector3 _movementInput;
    private LayerMask _lookLayers;

    private void Start()
    {
        _lookLayers = LayerMask.GetMask("Ground") |
        LayerMask.GetMask("Weapon") |
        LayerMask.GetMask("Shootable");

        currAmmo = new Dictionary<AmmoType, int>();
        currAmmo.Add(AmmoType.Pistol, 100);
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Move();
        Shoot();
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }
    
    #region Health
    public void OnDeath()
    {
        _onPlayerDeath.Invoke();
    }

    #endregion

    #region Movement
    private void Move()
    {
        // Rotates the input matrix to the camera's rotation & normalize
        _movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var rotatedInput = matrix.MultiplyPoint3x4(_movementInput);
        rotatedInput.Normalize();

        // Applies the velocity
        _rb.velocity = (rotatedInput * _speed);
    }

    private void Look()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _lookLayers))
        {
            var lookPt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookPt);

            // Moves the aim point
            lookPt.y = hit.point.y;
            _aimPoint.position = lookPt;
        }
    }
    #endregion

    #region Weapons/Inventory
    [SerializeField] private Transform _weaponPos;
    [SerializeField] private float _maxPickUpDistance;
    [SerializeField] private LayerMask _weaponMask;
    [SerializeField] private Dictionary<AmmoType, int> currAmmo;
    private Weapon equippedWeapon;

    private void Shoot()
    {
        if (equippedWeapon == null) return;
        if (Input.GetButtonDown("Fire1"))
        {
            equippedWeapon.Fire1(currAmmo);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            equippedWeapon.Fire1Stop(currAmmo);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            equippedWeapon.Fire2(currAmmo);
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            equippedWeapon.Fire2Stop(currAmmo);
        }
    }

    private void PickUp()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _weaponMask)) return;

        Transform weapon = hit.transform.root;
        var wep = weapon.GetComponent<Weapon>();
        if (!(Vector3.Distance(weapon.position, transform.position) < _maxPickUpDistance
            && wep.holder == null)) return;

        if (equippedWeapon != null)
        {
            equippedWeapon.DropWeapon();
        }
        wep.PickUpWeapon(gameObject, _weaponPos);
        equippedWeapon = wep;
    }


    #endregion

    #region Debug
    public void OnPlayerHit()
    {
        Debug.Log("Ouch");
    }
    #endregion
}
