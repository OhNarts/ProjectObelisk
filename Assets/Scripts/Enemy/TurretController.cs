using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurretController : MonoBehaviour
{

    public UnityEventEnemy onEnemyDeath;
    private EnemyState currState;
    private float stun;
    [SerializeField] Transform _target;
    [SerializeField] private float distToAttack;
    [SerializeField] private float fireRate;

    [SerializeField] private Weapon head;

    [SerializeField] private HealthHandler healthHandler;
    [SerializeField] private GameObject healthBar;
    private Camera mainCamera;

    public Transform Target
    {
        set
        {
            _target = value;
        }
    }

    private void Awake()
    {
        currState = EnemyState.Idle;
        CreateHealthBar();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Paused) return;
        switch (currState)
        {
            case EnemyState.Idle:
                Idle();
                if (Vector3.Distance(_target.position, transform.position) < distToAttack)
                {
                    currState = EnemyState.Attack;
                }
                break;
            case EnemyState.Attack:
                Attack();
                if (Vector3.Distance(_target.position, transform.position) > distToAttack)
                    currState = EnemyState.Idle;
                break;
        }

        // Fix Health Bar Direction
        if (healthBar.activeSelf)
        {
            healthBar.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }

    private void Attack()
    {
        transform.LookAt(_target);
        head.Fire1Start();
    }

    private void Idle()
    {
        head.Fire1Stop();
    }

    public void Die()
    {
        // Temp, can make ragdoll here instead of destroy
        Destroy(gameObject);
    }

    private void CreateHealthBar()
    {
        if (healthBar == null) return;
        /* if (healthBar.TryGetComponent<FaceCamera>(out FaceCamera faceCamera)) {
            faceCamera = GameObject.FindWithTag("MainCamera");
        } */
        if (healthHandler != null && healthHandler.Health < healthHandler.MaxHealth)
        {
            healthBar.SetActive(true);
        }
        else
        {
            healthBar.SetActive(false);
        }
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthHandler == null || healthBar == null) return;
        if (healthHandler.Health < healthHandler.MaxHealth) healthBar.SetActive(true);
        healthBar.GetComponentInChildren<Slider>(true).value = healthHandler.Health / healthHandler.MaxHealth;
    }
}
