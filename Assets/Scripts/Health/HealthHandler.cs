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

    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    private bool healthInitialized = false;
    [SerializeField] private float health;
    public float Health
    {
        get { return health; }
        set
        {
            if (value <= maxHealth) health = value;
            else { health = maxHealth; }
            healthInitialized = true;
        }
    }

    public HealthHandler(float maxHealth, float health)
    {
        this.maxHealth = maxHealth;
        this.health = health;
    }


    // Start is called before the first frame update
    void Awake()
    {
        if (!healthInitialized) health = maxHealth;
    }

    /// <summary>
    /// Damages this health handler
    /// </summary>
    /// <param name="info"></param>
    public void Damage(DamageInfo info)
    {
        health -= info.damage;
        onHealthChange?.Invoke();
        onDamage?.Invoke(info);
        if (health <= 0)
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
        float diff = health + amount - maxHealth;
        float amountHealed;
        if (diff < 0)
        {
            amountHealed = amount;
        } else
        {
            amountHealed = maxHealth - health;
        }
        health += amountHealed;
        onHealthChange?.Invoke();
        onHeal?.Invoke(amountHealed);
    }
}

public struct DamageInfo 
{
    public float damage;
    public GameObject attacker;
}
