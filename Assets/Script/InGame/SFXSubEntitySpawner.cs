using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class SFXSubEntitySpawner : SFXParticles {
    public int I_EntitySpawnID;
    public bool B_ApplyDOTOnSpawn = false;
    public void Play(int _sourceID, enum_EntityFlag _flag, Func<DamageDeliverInfo> _damageInfoOverride,int spawnerID,Action<EntityCharacterBase> OnSpawn=null)
    {
        base.Play(_sourceID);
        EntityCharacterBase entity= GameObjectManager.SpawnSubCharacter(I_EntitySpawnID,transform.position,spawnerID, _flag);
        OnSpawn?.Invoke(entity);
        entity.m_CharacterInfo.AddDamageOverride(_damageInfoOverride);
        if (B_ApplyDOTOnSpawn)
            entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Common,DamageDeliverInfo.BuffInfo(-1,SBuff.CreateSubEntityDOTBuff(1f,20f))));
    }
}
