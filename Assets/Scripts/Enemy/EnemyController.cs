using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyState { Idle, Chase, Attack, Stunned, Hide }
public class EnemyController : MonoBehaviour
{
    public UnityEventEnemy onEnemyDeath;

    private EnemyState currState;
    private NavMeshAgent agent;
    private AmmoDictionary ammo;
    private float stun;
    private float hideTimer;
    
    [SerializeField] private HealthHandler healthHandler;
    [SerializeField] private GameObject healthBar;
    private Camera mainCamera;

    [SerializeField] private EnemySerializable.EnemyData enemyData;
    [SerializeField] private float distToAttack;
    [SerializeField] private Weapon weapon; public Weapon EquippedWeapon {get => weapon;}
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private Transform equipPos;

    [SerializeField] private Transform _target;

    //[SerializeField] private bool knockbacked;
    [SerializeField] private float knockbackTime;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody[] ragdollRigidBodies;
    private bool isDead;
    public Transform Target
    {
        set
        {
            _target = value;
            //agent.enabled = true;
        }
    }

    void Awake()
    {
        DisableRagdoll();
        currState = EnemyState.Idle;
        agent = transform.GetComponent<NavMeshAgent>();
        if (weaponObject != null) {
            GameObject weaponInstance = Instantiate(weaponObject, equipPos.position, transform.rotation);
            weapon = weaponInstance.GetComponent<Weapon>();
        }
        if (weapon != null) weapon.PickUpWeapon(gameObject, equipPos);
        CreateHealthBar();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Paused) return;
        if(isDead) return;
        switch (currState)
        {
            case EnemyState.Idle:
                if (_target != null)
                {
                    if (enemyData.needsCover)
                    {
                        //Debug.Log("Enemy needs cover");
                        Vector3 targetCoverNode = new Vector3(0.0f, 0.0f, 0.0f);
                        bool foundCover = CoverFinder.Instance.FindCover(transform, _target, ref targetCoverNode);
                        //bool foundCover = CoverFinder.Instance.FindCover(gameObject.transform, _target, targetCoverNode);
                        //Debug.Log("found cover? " + foundCover);
                        //Debug.Log("target cover node: " + targetCoverNode);
                        if (foundCover)
                        {
                            MoveToCover(targetCoverNode);
                            break;
                        }
                    }

                    currState = EnemyState.Chase;
                }
                break;

            case EnemyState.Chase:
                Chase();
                if (Vector3.Distance(_target.position, transform.position) < distToAttack)
                    currState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                Attack();
                if (Vector3.Distance(_target.position, transform.position) > distToAttack)
                    currState = EnemyState.Chase;
                break;
            case EnemyState.Stunned:
                Stunned(stun);
                stun -= Time.deltaTime;
                if (stun <= 0.0f) {
                    currState = EnemyState.Idle;
                }
                break;
            case EnemyState.Hide:
                Hide();
                hideTimer -= Time.deltaTime;
                
                // if AI was in cover and timer has run out, go back to chasing
                if (hideTimer <= 0.0f) 
                {
                    // go back to chasing
                    currState = EnemyState.Chase;
                }
                break;
        }
        // Fix Health Bar Direction
        if (healthBar.activeSelf) {
            healthBar.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }

    #region Machine States

    private void Chase()
    {
        agent.isStopped = false;
        transform.LookAt(_target);
        agent.SetDestination(_target.position);
        animator.SetBool("IsWalking", true);
    }

    private void Attack()
    {
        agent.isStopped = true;
        transform.LookAt(_target);
        if (weapon != null) {
            weapon.Fire1Start();
            weapon.Fire1Stop();
        }
        animator.SetBool("IsWalking", false);
    }

    public void Stunned(float stunTime) 
    {
        if (currState != EnemyState.Stunned) {
            stun = stunTime;
            currState = EnemyState.Stunned;
        }
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
    }

    // navigates to cover node
    public void MoveToCover(Vector3 targetCoverPos) 
    {
        // Debug.Log("Moving to cover");
        if (agent.destination != targetCoverPos)
        {
            agent.SetDestination(targetCoverPos);
        }
        animator.SetBool("IsWalking", true);
        agent.isStopped = false;
        if (Vector3.Distance(targetCoverPos, transform.position) < 2.0f)
        {
            Hide();
        }
    }

    // Currently just keeps track of time spent hiding at cover node
    public void Hide()
    {
        animator.SetBool("IsWalking", false);
        if (currState != EnemyState.Hide)
        {
            // initialize hideTimer
            hideTimer = 2.0f;
            currState = EnemyState.Hide;
        }
        
        // stretch goal: peek
    }
    #endregion

    /// <summary>
    /// Handles this enemy dieing
    /// </summary>
    public void Die()
    {
        //weapon.DropWeapon();
        // Destroy(weapon.gameObject);
        // // weapon = null;
        // transform.GetComponent<Collider>().enabled = false;
        // agent.isStopped = true;

        // // Temp, can make ragdoll here instead of destroy
        // isDead = true;
        // currState = EnemyState.Stunned;
        // healthBar.SetActive(false);

        // EnableRagdoll();
        // GameManager.OnGameStateChanged += OnGameStateChange;
        
        onEnemyDeath?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnGameStateChange(object sender, OnGameStateChangedArgs e) {
        GameManager.OnGameStateChanged -= OnGameStateChange;
        Destroy(gameObject);
    }

    private void CreateHealthBar() {
        if (healthBar == null) return;
        /* if (healthBar.TryGetComponent<FaceCamera>(out FaceCamera faceCamera)) {
            faceCamera = GameObject.FindWithTag("MainCamera");
        } */
        if (healthHandler != null && healthHandler.Health < healthHandler.MaxHealth) {
            healthBar.SetActive(true);
        } else {
            healthBar.SetActive(false);
        }
        UpdateHealthBar();
    }

    public void UpdateHealthBar() {
        if (healthHandler == null || healthBar == null) return;
        if (healthHandler.Health < healthHandler.MaxHealth) healthBar.SetActive(true);
        healthBar.GetComponentInChildren<Slider>(true).value = healthHandler.Health / healthHandler.MaxHealth;
    }

    public void Knockback(Vector3 bulletPos, float knockbackValue) {
        //knockbacked = true;
        StartCoroutine(PerformKnockback(bulletPos, knockbackValue));
    }

    private IEnumerator PerformKnockback(Vector3 bulletPos, float knockbackValue)
    {
        //agent.enabled = false;
        //rb.isKinematic = false;

        Vector3 dir = (transform.position - bulletPos).normalized;
        agent.velocity = dir * knockbackValue;
        // or rb.velocity = dir * knockbackVel; --> with rigidbody initialized

        yield return new WaitForSeconds(knockbackTime);

        //agent.enabled = true;
        //rb.isKinematic = true;

        //knockbacked = false;
    }

    private void EnableRagdoll() {
        animator.enabled = false;
        foreach(Rigidbody rb in ragdollRigidBodies) {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    private void DisableRagdoll(){
        animator.enabled = true;
        foreach(Rigidbody rb in ragdollRigidBodies) {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
}
