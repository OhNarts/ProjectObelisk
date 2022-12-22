using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, Chase, Attack }
public class EnemyStateMachine : MonoBehaviour
{
    private EnemyState currState;
    private NavMeshAgent agent;
    private Coroutine shootRoutine;

    [SerializeField] private float distToAttack;

    [SerializeField] private Transform _target;
    public Transform Target
    {
        set
        {
            _target = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        currState = EnemyState.Idle;
        agent = transform.GetComponent<NavMeshAgent>();
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
                Shoot();
                //if (Vector3.Distance(_target.position, transform.position) > distToAttack ||
                //    Physics.Linecast(transform.position, _target.position))
                if (Vector3.Distance(_target.position, transform.position) > distToAttack)
                    currState = EnemyState.Chase;
                break;
        }
        Debug.Log(currState);
    }

    private void Chase()
    {
        agent.isStopped = false;
        transform.LookAt(_target);
        agent.SetDestination(_target.position);
    }

    private void Shoot()
    {
        agent.isStopped = true;
        transform.LookAt(_target);
    }
    
}
