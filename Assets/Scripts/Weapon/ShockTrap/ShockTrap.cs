using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockTrap : Weapon
{

    [Header("ShockTrap Specific")]
    [SerializeField] private float shockTime;
    [SerializeField] private float shockRadius;

    [SerializeField] private GameObject _closedModel;
    [SerializeField] private GameObject _openModel;
    [SerializeField] private Sound _shockSound;

    private void Awake() {
        CloseTrap();
    }

    public override void OnPlanDrag() {
        base.OnPlanDrag();
        CloseTrap();
    }

    public override void OnPlanDrop() {
        base.OnPlanDrop();
        OpenTrap();
    }

    // public override void Fire1Start(bool useAmmo = false) {
        
    // }

    public void Shock() {
        AudioManager.Play(_shockSound);

        Destroy(gameObject);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockRadius);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.CompareTag("Enemy")) {
                hitCollider.transform.GetComponent<EnemyController>().Stunned(shockTime);
            }
        }

    }

    private void OpenTrap() {
        _closedModel.SetActive(false);
        _openModel.SetActive(true);
    }

    private void CloseTrap() {
        _closedModel.SetActive(true);
        _openModel.SetActive(false);
    }
}
