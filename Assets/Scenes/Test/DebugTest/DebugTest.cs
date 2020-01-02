using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebugTest : MonoBehaviour {
    private void Awake()
    {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        float border = 50f;
        Bounds bound = new Bounds(Vector3.zero, new Vector3(border, .2f, border));

        NavMeshBuilder.CollectSources(bound, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, sources);
         NavMesh.AddNavMeshData(NavMeshBuilder.BuildNavMeshData(NavMesh.GetSettingsByIndex(0), sources, bound, Vector3.zero, Quaternion.identity));
    }
    private void Start()
    {
        GetComponent<NavMeshAgent>().SetDestination(new Vector3(20,0,0));
    }
}
