using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterAISub : EntityCharacterAI {
    public int m_SpawnerEntityID { get; private set; }
    public EntitySubInfoManager m_SubInfo { get; private set; }
    protected override CharacterInfoManager GetEntityInfo()
    {
        m_SubInfo = new EntitySubInfoManager(this, m_HitCheck.TryHit, OnExpireChange);
        return m_SubInfo;
    }
    public void OnRegister(int _spawnerEntityID)
    {
        m_SpawnerEntityID = _spawnerEntityID;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, ForceDead);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, ForceDead);
    }
    public void ForceDead()
    {
        OnDead();
    }
}
