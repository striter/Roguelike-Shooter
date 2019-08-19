using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class SFXEntitySpawner : SFXParticles {
    public int I_EntitySpawnID;
    public void Play(int _sourceID, enum_EntityFlag _flag,Func<DamageBuffInfo> _damageInfoOverride,Action<EntityBase> OnSpawn=null)
    {
        base.Play(_sourceID);
        EntityBase entity= ObjectManager.SpawnEntity(I_EntitySpawnID,transform.position, _flag,OnSpawn);
        entity.m_EntityInfo.AddDamageOverride(_damageInfoOverride);
    }
}
