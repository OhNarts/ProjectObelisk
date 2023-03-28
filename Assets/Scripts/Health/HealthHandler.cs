using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HealthHandler : MonoBehaviour
{
    [SerializeField] public UnityEventDamage onDamage;
    [SerializeField] public UnityEventFloat onHeal;
    [SerializeField] public UnityEvent onHealthChange;
    [SerializeField] public UnityEvent onDeath;
    [SerializeField] public UnityEventDamage onHit;
    [SerializeField] private float _maxHealth;
    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    private bool healthInitialized = false;
    [SerializeField] private float _health;
    public float Health
    {
        get { return _health; }
        set
        {
            if (value <= _maxHealth) _health = value;
            else { _health = _maxHealth; }
            healthInitialized = true;
        }
    }

    [SerializeField] private bool _isInvincible = false;
    public bool IsInvincible {
        get => _isInvincible;
        set {
            _isInvincible = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (!healthInitialized) _health = _maxHealth;
    }

    /// <summary>
    /// Damages this health handler
    /// </summary>
    /// <param name="info"></param>
    public virtual void Damage(DamageInfo info)
    {
        onHit?.Invoke(info);
        if (_isInvincible) return;
        _health -= info.damage;
        onHealthChange?.Invoke();
        onDamage?.Invoke(info);
        if (_health <= 0)
        {
            onDeath?.Invoke();
        }
    }

    /// <summary>
    /// Heals this health handler
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        float diff = _health + amount - _maxHealth;
        float amountHealed;
        if (diff < 0)
        {
            amountHealed = amount;
        } else
        {
            amountHealed = _maxHealth - _health;
        }
        _health += amountHealed;
        onHealthChange?.Invoke();
        onHeal?.Invoke(amountHealed);
    }
}

public struct DamageInfo 
{
    public float damage;
    public GameObject attacker;
    public Vector3 attackerPosition;
    public AmmoType ammoType;
    public float knockbackValue;
}
