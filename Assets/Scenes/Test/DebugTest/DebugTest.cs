using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebugTest : MonoBehaviour {
    private void Start()
    {
        GetComponent<NavMeshAgent>().SetDestination(new Vector3(20,0,0));
    }
}
