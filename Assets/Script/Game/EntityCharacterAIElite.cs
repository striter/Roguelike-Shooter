using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterAIElite : EntityCharacterAI {
    TimeCounter m_BuffCounter = new TimeCounter();
    EliteBuffCombine m_Buff;
    bool m_Indicated;
    public override void OnActivate(enum_EntityFlag _flag, int _spawnerID, float startHealth)
    {
        base.OnActivate(_flag, _spawnerID, startHealth);
        PrepareEliteBuff();
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (!m_Battling)
            return;

        if (!m_BuffCounter.m_Timing)
            return;

        m_BuffCounter.Tick(Time.deltaTime);
        if (!m_Indicated && m_BuffCounter.m_timeCheck < 2f)
            DoBuffIndicate();

        if (m_BuffCounter.m_Timing)
            return;
        GenerateBuff();
        PrepareEliteBuff();
    }

    void PrepareEliteBuff()
    {
        m_Buff = GameExpression.GetEliteBuff.RandomItem();
        m_BuffCounter.SetTimer(GameExpression.GetEliteBuffTimer(m_Health.F_HealthMaxScale));
        m_Indicated = false;
    }
    void DoBuffIndicate()
    {
        m_Indicated = true;
        SFXIndicator indicator = GameObjectManager.SpawnIndicator(m_Buff.m_IndicatorIndex, transform.position, Vector3.up);
        indicator.AttachTo(transform);
        indicator.Play(m_EntityID, 2f);
    }

    void GenerateBuff()
    {
        m_CharacterInfo.AddBuff(-1,GameDataManager.GetPresetBuff(m_Buff.m_BuffIndex));
        GameObjectManager.SpawnSFX<SFXMuzzle>(m_Buff.m_MuzzleIndex,transform.position,Vector3.up).Play(m_EntityID);
    }
}
