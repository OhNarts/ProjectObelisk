using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public DamageInfo damageInfo;
    public bool destroyOnContact = true;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Bullet")) return;
        if (destroyOnContact) Destroy(gameObject);
        Transform hitTransform = collider.transform;
        HealthHandler hitHealth = hitTransform.GetComponent<HealthHandler>();
        if (hitHealth != null)
        {
            hitHealth.Damage(damageInfo);
            var enemy = collider.GetComponent<EnemyController>();
            if (enemy != null && !hitHealth.IsInvincible) {
                enemy.Knockback(damageInfo.attackerPosition, damageInfo.knockbackValue);
            }
        }
    }
}
