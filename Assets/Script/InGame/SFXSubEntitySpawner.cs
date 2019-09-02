using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class SFXSubEntitySpawner : SFXParticles {
    public int I_EntitySpawnID;
    public void Play(int _sourceID, enum_EntityFlag _flag, Func<DamageDeliverInfo> _damageInfoOverride,int spawnerID,Action<EntityBase> OnSpawn=null)
    {
        base.Play(_sourceID);
        EntityAISub entity= ObjectManager.SpawnSubEntity(I_EntitySpawnID,transform.position,spawnerID, _flag);
        OnSpawn?.Invoke(entity);
        entity.m_SubInfo.AddDamageOverride(_damageInfoOverride);
    }
}
