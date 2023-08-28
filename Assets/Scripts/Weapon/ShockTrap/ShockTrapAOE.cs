using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockTrapAOE : MonoBehaviour
{
    public HashSet<EnemyController> _enemiesInside;

    void Start() {
        _enemiesInside = new HashSet<EnemyController>();
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            _enemiesInside.Add(collider.transform.root.GetComponent<EnemyController>());
        }
    }

    void OnTriggerExit(Collider collider) {
         if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            _enemiesInside.Remove(collider.transform.root.GetComponent<EnemyController>());
        }
    }

}
