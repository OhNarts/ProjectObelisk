using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    public DamageInfo damageInfo;
    public float speed;

    private void FixedUpdate()
    {
        transform.position += Time.deltaTime * speed * transform.forward;
    }

    public void OnTriggerEnter(Collider collider)
    {

        Transform hitTransform = collider.transform;
        HealthHandler hitHealth = hitTransform.GetComponent<HealthHandler>();
        if (hitHealth != null)
        {
            hitHealth.Damage(damageInfo);
        }    
        Destroy(gameObject);
    }
}
