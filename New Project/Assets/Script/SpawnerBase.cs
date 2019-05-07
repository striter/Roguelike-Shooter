using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBase : MonoBehaviour {
    public bool B_SpawnOnStart = true;
    private void Start()
    {
        if (B_SpawnOnStart)
            OnSpawn();
    }
    protected virtual void OnSpawn()
    {

    }
}
