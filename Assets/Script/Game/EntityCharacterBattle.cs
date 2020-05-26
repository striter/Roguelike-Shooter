using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterBattle : EntityCharacterBase {

    public override enum_EntityType m_ControllType => enum_EntityType.BattleEntity;
    public float F_BaseDamage;
    public float F_Spread;
    public int m_SpawnerEntityID { get; private set; }
    public bool m_IsSubEntity => m_SpawnerEntityID != -1;
    public ExpireBattleCharacterBase m_Perk { get; private set; }

    public EntityCharacterBattle OnCharacterBattleActivate(enum_EntityFlag _flag, ExpireBattleCharacterBase perk, int _spawnerID = -1)
    {
        m_Perk = perk;
        m_SpawnerEntityID = _spawnerID;
        OnEntityActivate(_flag);

        m_CharacterInfo.AddExpire(perk);
        m_Health.OnHealthMultiplierChange(1f + perk.m_MaxHealthMultiplierAdditive);
        return this;
    }

    public override void OnPoolRecycle()
    {
        base.OnPoolRecycle(); 
        m_Perk = null;
    }
}
