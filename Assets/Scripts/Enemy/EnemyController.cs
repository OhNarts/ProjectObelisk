using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, Chase, Attack }
public class EnemyController : MonoBehaviour
{
    private EnemyState currState;
    private NavMeshAgent agent;
    private Dictionary<AmmoType, int> ammo;
    

    [SerializeField] private float distToAttack;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform equipPos;
    [SerializeField] private Transform _target;
    public Transform Target
    {
        set
        {
            _target = value;
        }
    }

    void Awake()
    {
        currState = EnemyState.Idle;
        agent = transform.GetComponent<NavMeshAgent>();
        weapon.PickUpWeapon(gameObject, equipPos);
        ammo = new Dictionary<AmmoType, int>();
        ammo.Add(weapon.AmmoType1, 1000000000);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case EnemyState.Idle:
                if (_target != null) currState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                Chase();
                if (Vector3.Distance(_target.position, transform.position) < distToAttack)
                    currState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                Attack();
                //if (Vector3.Distance(_target.position, transform.position) > distToAttack ||
                //    Physics.Linecast(transform.position, _target.position))
                if (Vector3.Distance(_target.position, transform.position) > distToAttack)
                    currState = EnemyState.Chase;
                break;
        }
    }

    #region Machine States

    private void Chase()
    {
        agent.isStopped = false;
        transform.LookAt(_target);
        agent.SetDestination(_target.position);
    }

    private void Attack()
    {
        agent.isStopped = true;
        transform.LookAt(_target);
        weapon.Fire1(ammo);
        weapon.Fire1Stop(ammo);
    }
    #endregion

    /// <summary>
    /// Handles this enemy dieing
    /// </summary>
    public void Die()
    {
        weapon.DropWeapon();

        Destroy(gameObject);
        // Temp, can make ragdoll here instead of destroy
    }
}
