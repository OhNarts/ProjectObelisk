using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverFinder : MonoBehaviour
{

    private static CoverFinder _instance;
    public static CoverFinder Instance
    {
        get
        {
            if (_instance == null) { Debug.LogError("Cover Finder was null"); }
            return _instance;
        }
    }

    GameObject[] nodeList;
    void Awake() 
    {
        _instance = this;
        nodeList = GameObject.FindGameObjectsWithTag("CoverNode");
    }

    // will find a node closest to currentPos that obscures line of sight from target pos
    public bool FindCover(Transform currentPos, Transform targetPos, ref Vector3 returnNodePos)
    {
        // Debug.Log("trying to find cover");
        // brute force: raycast from every cover node starting with closest to find line of sight
        PriorityQueue<GameObject, float> nodeDists = GetNodeDists(currentPos);

        // Iterates through Priority Queue in order of priority
        for (int nodeIndex = 0; nodeIndex < nodeDists.Count(); nodeIndex++)
        {
            GameObject currentNode = nodeDists.ElementAt(nodeIndex).Item1;
            // for each node, check that the node itself is considered hidden from the target
            bool hidden = CheckHidden(currentNode, targetPos);

            if (hidden)
            {
                returnNodePos = currentNode.transform.position;
                return true;
            }
        }
        return false;
    }

    PriorityQueue<GameObject, float> GetNodeDists(Transform currentPos)
    {
        PriorityQueue<GameObject, float> nodeDists = new PriorityQueue<GameObject, float>();

        foreach (GameObject node in nodeList)
        {
            Vector3 distVector = currentPos.position - node.transform.position; 
            float dist = distVector.magnitude;
            nodeDists.Enqueue(node, dist);
        }

        return nodeDists;
    }

    // if raycast hits Not Player, consider hidden
    bool CheckHidden(GameObject node, Transform targetPos)
    {
        Vector3 direction = targetPos.position - node.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(node.transform.position, direction, out hit))
        {
            if (hit.collider.tag != "Player")
            {
                return true;
            }
        }
        return false;
    }
}
