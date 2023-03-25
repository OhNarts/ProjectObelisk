using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurretController : MonoBehaviour
{
    private EnemyState currState;
    private float stun;
    [SerializeField] Transform _target;
    [SerializeField] private float distToAttack;
    [SerializeField] private float fireRate;

    [SerializeField] private Weapon head;

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
}
