using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { Damage, Knockback }

public class BuffRegion : MonoBehaviour 
{
    public bool restrictArea;
    public BuffType buffType;

    public Material availableMat;
    public Material unavailableMat;
    public MeshRenderer availableMesh;

    private void Awake() {
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnCollisionStay(Collision other) {
        if (!restrictArea) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
                Weapon wep = other.transform.root.GetComponent<Weapon>();
                if (wep.Holder == null) {
                    restrictArea = true;
                    wep.isBuffed = true;
                    wep.buffRegion = GetComponent<BuffRegion>();
                    availableMesh.material = unavailableMat;
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
                availableMesh.material = availableMat;
            }
        }
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        restrictArea = false;
    }

    private void OnGameStateChanged(object sender, OnGameStateChangedArgs e) {
        if (GameManager.CurrentState == GameState.Plan) {
            availableMesh.gameObject.SetActive(true); 
        } else  {
            availableMesh.gameObject.SetActive(false);
        }
    }
}
