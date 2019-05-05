using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof( SpawnerBase))]
public class LivingSpawnerContainer : LivingBase
{
    public override enum_LivingType E_Type => enum_LivingType.ContainerChips;
    SpawnerBase m_attachedSpawner;
    protected override void Awake()
    {
        base.Awake();
        m_attachedSpawner = GetComponent<SpawnerBase>();
    }
    protected override void OnDead()
    {
        m_attachedSpawner.Spawn();
        switch (E_Type)
        {
            case enum_LivingType.ContainerChips:
                for (int i = 0; i < 3; i++)
                {
                    Transform chips;
                    chips=EntityManager.SpawnLiving<LivingBase>(enum_LivingType.ContainerChips,null).transform;
                    chips.position = transform.position;
                    chips.rotation = TCommon.RandomRotation();
                    chips.position = TCommon.RandomPositon(transform.position);
                }
                break;
        }
        Destroy(this.gameObject);
    }
}
