using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public DamageInfo damageInfo;

    public void OnTriggerEnter(Collider collider)
    {
        Destroy(gameObject);
        Transform hitTransform = collider.transform;
        HealthHandler hitHealth = hitTransform.GetComponent<HealthHandler>();
        if (hitHealth != null)
        {
            hitHealth.Damage(damageInfo);
        }    
    }
}
