using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXEntitySpawner : SFXParticles {
    public int I_EntitySpawnID;

    public void Play(int _sourceID)
    {
        base.Play(_sourceID);

        ObjectManager.SpawnEntity(I_EntitySpawnID,transform.position).OnActivate();
    }
}
