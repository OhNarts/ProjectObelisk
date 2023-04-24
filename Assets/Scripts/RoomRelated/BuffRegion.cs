using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { Damage, Knockback }

public class BuffRegion : MonoBehaviour 
{
    public bool restrictArea;
    public BuffType buffType;

    private void Awake() {
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
    }

    private void OnCollisionStay(Collision other) {
        if (!restrictArea) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
                Weapon wep = other.transform.root.GetComponent<Weapon>();
                if (wep.Holder == null) {
                    restrictArea = true;
                    wep.isBuffed = true;
                    wep.buffRegion = GetComponent<BuffRegion>();
                }
            }
        }
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
            Weapon wep = other.transform.root.GetComponent<Weapon>();
            if (wep.Holder == null) {
                restrictArea = false;
                wep.isBuffed = false;
                wep.buffRegion = null;
            }
        }
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        restrictArea = false;
    }
}
