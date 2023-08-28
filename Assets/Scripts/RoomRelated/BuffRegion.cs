using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public enum BuffType { Damage, Knockback }

public class BuffRegion : MonoBehaviour 
{
    public bool restrictArea;
    public BuffType buffType;

    public Material availableMat;
    public Material unavailableMat;
    public MeshRenderer availableMesh;
    [SerializeField] private Weapon currWep;

    private void Awake() {
        PlayerState.OnPlayerStateRevert += OnPlayerStateRevert;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy() {
        PlayerState.OnPlayerStateRevert -= OnPlayerStateRevert;
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        UnrestrictArea();
    }

    private void OnTriggerEnter(Collider other) {
        if (GameManager.CurrentState != GameState.Plan) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Weapon")) return;
        Weapon wep = other.transform.root.GetComponent<Weapon>();
        if (wep != currWep && currWep != null) {
            wep.CanPlace = false;
            return;
        }
        RestrictArea(wep); 
    }

    private void OnTriggerExit(Collider other) {
        if (GameManager.CurrentState != GameState.Plan) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Weapon")) return;
        Weapon wep = other.transform.root.GetComponent<Weapon>();
        if (wep == currWep) {
            UnrestrictArea();
        }
        wep.CanPlace = true;
    }

    private void RestrictArea(Weapon wep) {
        currWep = wep;
        currWep.isBuffed = true;
        wep.buffRegion = this;
        availableMesh.material = unavailableMat;
        currWep.OnWeaponDestroyed += OnWeaponDestroyed;
    }

    private void UnrestrictArea() {
        if (currWep != null) {
            currWep.isBuffed = false;
            currWep.buffRegion = null;
            currWep.OnWeaponDestroyed -= OnWeaponDestroyed;
        }
        currWep = null;
        availableMesh.material = availableMat;
    }

    private void OnWeaponDestroyed(object sender, EventArgs e) {
        UnrestrictArea();
    }

    private void OnPlayerStateRevert(object sender, OnPlayerStateRevertArgs e) {
        UnrestrictArea();
    }

    private void OnGameStateChanged(object sender, OnGameStateChangedArgs e) {
        if (GameManager.CurrentState == GameState.Plan) {
            availableMesh.gameObject.SetActive(true);
            return;
        }
        if (GameManager.CurrentState == GameState.PostCombat) {
            UnrestrictArea();
        } 
        availableMesh.gameObject.SetActive(false);
    }
}
