using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [SerializeField] private AmmoType bulletType;
    [SerializeField] private HealthHandler handler;

    public void DestroyWall() {
        Destroy(gameObject);
    }

    public void HitWall(DamageInfo info) {
        Debug.Log("Wall hit" + info.ammoType.ToString());
        if (info.ammoType == bulletType) {
            Debug.Log("Good hit.");
            handler.IsInvincible = false;
        } else {
            handler.IsInvincible = true;
        }
    }
}
