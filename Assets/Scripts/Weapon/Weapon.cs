using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponType { Gun, Melee }
public enum AmmoType { Rifle, Pistol, Energy, Shotgun, NONE }

public abstract class Weapon : MonoBehaviour
{

    [Header("Attack Point")]
    // Where the attack comes from (i.e. where bullets come from or where melee hits) 
    [SerializeField] protected Transform _attackPoint;

    [Header("Weapon Info")]
    [SerializeField] private WeaponItem _weaponItem; public WeaponItem WeaponItem { get => _weaponItem; }
    // The damage an attack does
    [SerializeField] protected float _damage;
    [SerializeField] protected WeaponType _weaponType;

    [Header("Ammo Costs/Types")]
    // [SerializeField] protected int _ammoCost1; public int AmmoCost1 { get => _ammoCost1; }
    // [SerializeField] protected AmmoType _ammoType1; public AmmoType AmmoType1 { get => _ammoType1; }
    [SerializeField] protected int _ammoAmount1; public int AmmoAmount1 { get => _ammoAmount1; }
    // [SerializeField] protected int _ammoCost2; public int AmmoCost2 { get => _ammoCost2; }
    // [SerializeField] protected AmmoType _ammoType2; public AmmoType AmmoType2 { get => _ammoType2; }
    [SerializeField] protected int _ammoAmount2; public int AmmoAmount2 { get => _ammoAmount2; }

    protected GameObject _holder = null; public GameObject Holder { get => _holder; }

    public void InitializeWeapon(int ammoAmount1, int ammoAmount2) {
        _holder = null;
        transform.parent = null;
        transform.GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        _ammoAmount1 = ammoAmount1;
        _ammoAmount2 = ammoAmount2;
    }

    /// <summary>
    /// Picks up this weapon and puts it in the proper position
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="equipPos"></param>
    public void PickUpWeapon(GameObject holder, Transform equipPos)
    {
        _holder = holder;
        transform.parent = equipPos;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(_holder.transform.forward);
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Drops this weapon
    /// </summary>
    public virtual void DropWeapon()
    {
        _holder = null;
        transform.parent = null;
        transform.GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<Rigidbody>().isKinematic = false;
    }


    // Use ammo defaults to false because player is the only
    // case where ammo is going to be used
    public virtual void Fire1(bool useAmmo = false) { }
    public virtual void Fire1Stop(bool useAmmo = false) { }

    public virtual void Fire2(bool useAmmo = false) { }
    public virtual void Fire2Stop(bool useAmmo = false) { }

}

