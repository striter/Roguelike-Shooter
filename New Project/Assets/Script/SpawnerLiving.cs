using System.Collections.Generic;
using UnityEngine;

public class SpawnerLiving : SpawnerBase
{
    public List<enum_LivingType> L_SpawnItems;
    public override void Spawn()
    {
        for (int i = 0; i < L_SpawnItems.Count; i++)
        {
            EntityManager.SpawnLiving<LivingBase>(L_SpawnItems[i], transform);
        }
    }
}
