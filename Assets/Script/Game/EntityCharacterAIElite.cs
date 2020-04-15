using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterAIElite : EntityCharacterAI {
    TimerBase m_BuffCounter = new TimerBase(GameConst.F_EliteBuffTimerDurationWhenFullHealth), m_IndicateCounter=new TimerBase(2f);
    EliteBuffCombine m_Buff;
    bool m_Indicating;
    protected override void OnEntityActivate(enum_EntityFlag flag, float startHealth = 0)
    {
        base.OnEntityActivate(flag, startHealth);
        m_Buff = GameConst.L_GameEliteBuff.RandomItem();
        m_BuffCounter.Replay();
        m_Indicating = false;
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if(m_Indicating)
        {
            m_IndicateCounter.Tick(deltaTime);
            if (m_IndicateCounter.m_Timing)
                return;

            m_CharacterInfo.AddBuff(-1, GameDataManager.GetPresetBuff(m_Buff.m_BuffIndex));
            GameObjectManager.SpawnSFX<SFXMuzzle>(m_Buff.m_MuzzleIndex, transform.position, Vector3.up).PlayUncontrolled(m_EntityID);
            m_Buff = GameConst.L_GameEliteBuff.RandomItem();
            m_BuffCounter.Replay();
            m_Indicating = false;
        }
        else
        {
            m_BuffCounter.Tick(deltaTime*(1-m_Health.F_HealthMaxScale)*GameConst.F_EliteBuffTimerTickRateMultiplyHealthLoss);
            if (m_BuffCounter.m_Timing)
                return;

            m_Indicating = true;
            m_IndicateCounter.Replay();
            SFXIndicator indicator = GameObjectManager.SpawnIndicator(m_Buff.m_IndicatorIndex, transform.position, Vector3.up);
            indicator.AttachTo(transform);
            indicator.PlayUncontrolled(m_EntityID, 2f);
        }
    }

}
