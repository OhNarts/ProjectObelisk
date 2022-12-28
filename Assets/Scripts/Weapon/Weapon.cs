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
    // The damage an attack does
    [SerializeField] protected float _damage;
    [SerializeField] protected WeaponType _weaponType;

    [Header("Ammo Costs/Types")]
    [SerializeField] protected int _ammoCost1;
    [SerializeField] protected AmmoType _ammoType1;
    public AmmoType AmmoType1
    {
        get
        {
            return _ammoType1;
        }
    }

    [SerializeField] protected int _ammoCost2;
    [SerializeField] protected AmmoType _ammoType2;
    public AmmoType AmmoType2
    {
        get
        {
            return _ammoType2;
        }
    }

    protected GameObject _holder = null;
    public GameObject holder
    {
        get
        {
            return _holder;
        }
    }

    /// <summary>
    /// Picks up this weapon and puts it in the proper position
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="equipPos"></param>
    public virtual void PickUpWeapon(GameObject holder, Transform equipPos)
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


    // Abstract methods, override these to get a weapon to fire
    // Pass the ammo dictionary so that the methods can subtract the correct ammo amount when used
    public virtual void Fire1(Dictionary<AmmoType, int> ammo) { }
    public virtual void Fire1Stop(Dictionary<AmmoType, int> ammo) { }

    public virtual void Fire2(Dictionary<AmmoType, int> ammo) { }
    public virtual void Fire2Stop(Dictionary<AmmoType, int> ammo) { }
}

