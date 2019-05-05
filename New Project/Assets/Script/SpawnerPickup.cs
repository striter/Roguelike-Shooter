using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPickup : SpawnerBase
{
    public List<enum_Pickup> L_SpawnItems;
    public override void Spawn()
    {
        for (int i = 0; i < L_SpawnItems.Count; i++)
        {
            EntityManager.SpawnPickup<PickupBase>(L_SpawnItems[i], transform);
            transform.position = TCommon.RandomPositon(transform.position);
        }
    }
}
