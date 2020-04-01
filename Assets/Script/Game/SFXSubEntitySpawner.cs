﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class SFXSubEntitySpawner : SFXWeaponBase {
    public bool B_SpawnAtTarget = false;
    public int I_EntitySpawnID;
    public int I_DelayIndicator = 0;
    public float F_DelayDuration = 0;
    float m_StartHealth;
    Action<EntityCharacterBase> OnSpawn;
    EntityCharacterBase m_Spawner;
    Vector3 m_targetPos;
    public void Play(EntityCharacterBase character,Vector3 target, float startHealth, Action<EntityCharacterBase> _OnSpawn)
    {
        base.PlayUncontrolled(character.m_EntityID,1f,F_DelayDuration);
        m_Spawner = character;
        m_targetPos = target;
        m_StartHealth = startHealth;
        OnSpawn = _OnSpawn;
        if (I_DelayIndicator > 0)
            GameObjectManager.SpawnIndicator(I_DelayIndicator, transform.position, Vector3.up).PlayUncontrolled(character.m_EntityID,F_DelayDuration);
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        EntityCharacterBase entity = GameObjectManager.SpawnEntitySubCharacter(I_EntitySpawnID, NavigationManager.NavMeshPosition(transform.position), m_targetPos, m_Spawner.m_Flag, m_Spawner.m_EntityID, m_StartHealth);
        OnSpawn?.Invoke(entity);
    }

}
