﻿using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterGameBase : EntityCharacterBase {

    public override enum_EntityType m_ControllType => enum_EntityType.GameEntity;
    public float F_BaseDamage;
    public float F_Spread;
    public int m_SpawnerEntityID { get; private set; }
    public bool m_IsSubEntity => m_SpawnerEntityID != -1;
    public ExpireGameCharacterBase m_Perk { get; private set; }

    public EntityCharacterGameBase OnCharacterGameActivate(enum_EntityFlag _flag, ExpireGameCharacterBase perk, int _spawnerID = -1)
    {
        m_Perk = perk;
        m_SpawnerEntityID = _spawnerID;
        OnEntityActivate(_flag);

        m_CharacterInfo.AddExpire(perk);
        m_Health.OnHealthMultiplierChange(1f + perk.m_MaxHealthMultiplierAdditive);
        return this;
    }

    public override void OnPoolItemRecycle()
    {
        base.OnPoolItemRecycle(); 
        m_Perk = null;
    }
}
