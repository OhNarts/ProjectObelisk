using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockTrap : Weapon
{

    [Header("ShockTrap Specific")]
    [SerializeField] private float shockTime;
    [SerializeField] private float shockRadius;

    public override void Fire1Start(bool useAmmo = false) {
        return;
    }

    public void Shock() {
        Destroy(gameObject);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockRadius);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.CompareTag("Enemy")) {
                hitCollider.transform.GetComponent<EnemyController>().Stunned(shockTime);
            }
        }

    }
}
