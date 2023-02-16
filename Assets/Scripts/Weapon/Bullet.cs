using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public DamageInfo damageInfo;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 9) return;

        
        Destroy(gameObject);
        Transform hitTransform = collider.transform;
        HealthHandler hitHealth = hitTransform.GetComponent<HealthHandler>();
        if (hitHealth != null)
        {
            hitHealth.Damage(damageInfo);
        }
    }
}
