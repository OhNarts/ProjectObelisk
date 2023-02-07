using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.CurrentState == GameState.Plan)
        {
            GetComponent<MeshCollider>().enabled = false;
        }
        if (GameManager.CurrentState != GameState.Plan)
        {
            GetComponent<MeshCollider>().enabled = true;
        }
    }
}

