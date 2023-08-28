using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponType { Gun, Melee }
public enum AmmoType { Explosive, Pistol, Energy, Shotgun, NONE }

#region Event Args
public class OnWeaponAmmoChangedArgs : EventArgs {
    private int _oldAmount; public int OldAmount {get => _oldAmount;}
    private int _newAmount; public int NewAmount {get => _newAmount;}
    public OnWeaponAmmoChangedArgs(int oldAmount, int newAmount) {
        _oldAmount = oldAmount;
        _newAmount = newAmount;
    }
}
#endregion

public abstract class Weapon : MonoBehaviour
{

    [Header("Attack Point")]
    // Where the attack comes from (i.e. where bullets come from or where melee hits) 
    [SerializeField] protected Transform _attackPoint; public Transform AttackPoint { get => _attackPoint; }

    [Header("Weapon Info")]
    [SerializeField] private WeaponItem _weaponItem; public WeaponItem WeaponItem { get => _weaponItem; }
    [SerializeField] private List<MeshCollider> _colliders; public List<MeshCollider> Colliders{get => _colliders;}
    [SerializeField] private ExclusionZone _exclusionZone;
    private float _exclusionRadius = 5f;
    [SerializeField] private string _animationBoolName; public string AnimationBoolName{get => _animationBoolName;}
    [SerializeField] protected Sound soundWhenFired;
    [SerializeField] protected Sound soundWhenFireStopped;
    [SerializeField] protected float _damage;
    [SerializeField] protected float _buffDamage;
    [SerializeField] protected WeaponType _weaponType;
    [SerializeField] protected float _thrownDamage;
    [SerializeField] private float _thrownSpeed;
    [SerializeField] private float _thrownStunDuration;
    [SerializeField] private float knockbackVelocity;
    [SerializeField] private float buffKnockbackVelocity;
    public bool isBuffed;
    [HideInInspector] public BuffRegion buffRegion;

    [Header("Ammo Costs/Types")]
    [SerializeField] protected int _ammoAmount1; public int AmmoAmount1 { 
        get => _ammoAmount1;
        protected set {
            int oldAmount = _ammoAmount1;
            _ammoAmount1 = value;
            OnWeaponAmmoChanged?.Invoke(this,
            new OnWeaponAmmoChangedArgs(oldAmount, _ammoAmount1));
        } 
    }

    private Vector3 _scale;

    protected GameObject _holder = null; public GameObject Holder { get => _holder; }
    protected bool _isProjectile;

    private bool _canPlace = true; public bool CanPlace {get => _canPlace; 
    set {
        _canPlace = value;
    }}

    private void Start() {
        InitializeSound(soundWhenFired);
        InitializeSound(soundWhenFireStopped);
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    #region Events
    public EventHandler<OnWeaponAmmoChangedArgs> OnWeaponAmmoChanged;
    public EventHandler OnWeaponDestroyed;
    #endregion

    /// <summary>
    /// Initializes this weapon with the given ammo ammounts
    /// WARNING: AMMO AMOUNT 2 IS NOT YET IMPLEMENTED
    /// </summary>
    /// <param name="ammoAmount1"></param>
    /// <param name="ammoAmount2"></param>
    public void InitializeWeapon(int ammoAmount1, int ammoAmount2 = 0) {
        _holder = null;
        transform.parent = null;
        foreach (MeshCollider collider in _colliders) {
            collider.enabled = true;
        }
        transform.GetComponent<Rigidbody>().isKinematic = false; 
        _scale = transform.localScale;   
        _ammoAmount1 = ammoAmount1;
        _isProjectile = false;
        _exclusionZone.Radius = _exclusionRadius;
        _exclusionZone.gameObject.SetActive(false);
        _exclusionZone.ExclusionWeapon = this;
    }

    /// <summary>
    /// Picks up this weapon and puts it in the proper position
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="equipPos"></param>
    public void PickUpWeapon(GameObject holder, Transform equipPos)
    {
        _scale = transform.localScale;
        _holder = holder;
        gameObject.layer = _holder.layer;
        Debug.Log("lossy = " + transform.lossyScale + "relative = " + transform.localScale);
        transform.SetParent(equipPos, true);
        Debug.Log("After: lossy = " + transform.lossyScale + "relative = " + transform.localScale);
        // transform.parent = equipPos;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(_holder.transform.forward);
        foreach (MeshCollider collider in _colliders) {
            collider.enabled = false;
            collider.gameObject.layer = _holder.layer;
        }
        if (_isProjectile) {
            Debug.Log("Weapon was Projectile!!!");
        }
        _isProjectile = false;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;
        //transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.localScale = _scale;
    }

    /// <summary>
    /// Drops this weapon
    /// </summary>
    public virtual void DropWeapon()
    {
        DetatchHolder();
        if (AmmoAmount1 == 0) {
            Destroy(gameObject);
        }
    }
    public virtual void ThrowWeapon() {
        DetatchHolder();
        _isProjectile = true;
        GetComponent<Rigidbody>().velocity = _attackPoint.forward * _thrownSpeed;
    }

    private void DetatchHolder() {
        _holder = null;
        transform.SetParent(null, true);

        gameObject.layer = LayerMask.NameToLayer("Weapon");

        foreach (MeshCollider collider in _colliders) {
            collider.enabled = true;
            collider.gameObject.layer = LayerMask.NameToLayer("Weapon");
        }

        transform.localScale = _scale;

        transform.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void OnTriggerEnter(Collider collider) {
        if (!_isProjectile) return;
        if (collider.gameObject.layer == 9) return;
        if (collider.gameObject.name == "Player") return;

        _isProjectile = false;
        GetComponent<Collider>().enabled = false;
        Transform hitTransform = collider.transform;
        HealthHandler hitHealth = hitTransform.GetComponent<HealthHandler>();
        if (collider.CompareTag("Enemy")) {
            collider.transform.GetComponent<EnemyController>().Stunned(_thrownStunDuration);
        }
        if (AmmoAmount1 == 0) {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        OnWeaponDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnPlanDrag() {
        _exclusionZone.gameObject.SetActive(true);
    }

    public virtual void OnPlanDrop() {
        _exclusionZone.gameObject.SetActive(true);
    }

    private void OnGameStateChanged(object sender, OnGameStateChangedArgs e) {
        if (e.NewState == GameState.Plan) return;
        _exclusionZone.gameObject.SetActive(false);
    }

    // Use ammo defaults to false because player is the only
    // case where ammo is going to be used
    public virtual void Fire1Start(bool useAmmo = false) {
        PlaySound(soundWhenFired);
    }
    public virtual void Fire1Stop(bool useAmmo = false) {
        PlaySound(soundWhenFireStopped);
    }
    public virtual void Fire1Held(bool useAmmo = false) { }

    public virtual void Fire2(bool useAmmo = false) { }
    public virtual void Fire2Stop(bool useAmmo = false) { }

    protected DamageInfo CreateDamageInfo() {
        float newDamage = _damage;
        float newKnockback = knockbackVelocity;
        if (isBuffed) {
            switch(buffRegion.buffType) {
                case BuffType.Damage:
                    newDamage = _buffDamage;
                    break;
                case BuffType.Knockback:
                    newKnockback = buffKnockbackVelocity;
                    break;
                default:
                    break;
            }
        }
        return new DamageInfo {
            damage = newDamage,
            attacker = _holder,
            attackerPosition = new Vector3(_holder.transform.position.x, _holder.transform.position.y, _holder.transform.position.z),
            ammoType = _weaponItem.AmmoType1,
            knockbackValue = newKnockback
        };
    }

    protected void PlaySound(Sound sound) {
        var isEnemy = _holder.GetComponent<EnemyController>();
        sound.position = Camera.main.transform.position;
        if (isEnemy != null) 
            sound.volume = 0.1f;
        
        AudioManager.Play(sound);
            // sound.source.Play();

    }

    private void InitializeSound(Sound sound) {
        sound.source = gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
    } 
}

