using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

namespace GameSetting_EnermyPerks
{
    public class EB30000_Default:ExpireEnermyPerkBase
    {
        public override int m_Index => GameDataManager.m_DefaultEnermyPerkIdentity;
        public EB30000_Default(float baseMaxHealthMultiplier, float baseDamageMultiplier) : base(baseMaxHealthMultiplier, baseDamageMultiplier) { }
    }

    public class EB30001:ExpireEnermyPerkBase
    {
        public override int m_Index => 30001;
        public override int m_EffectIndex => 40014;
        public override float m_MaxHealthMultiplierAdditive => base.m_MaxHealthMultiplierAdditive + 3f;
        public override void OnAttackSetDamage(DamageInfo info)
        {
            base.OnAttackSetDamage(info);
            info.AddExtraBuff(105);
        }
        public EB30001(float baseMaxHealthMultiplier, float baseDamageMultiplier) : base(baseMaxHealthMultiplier, baseDamageMultiplier) { }
    }

    public class EB30002 : ExpireEnermyPerkBase
    {
        public override int m_Index => 30002;
        public override int m_EffectIndex => 40002;
        public override float m_MaxHealthMultiplierAdditive => base.m_MaxHealthMultiplierAdditive+3f;
        public EB30002(float baseMaxHealthMultiplier, float baseDamageMultiplier) : base(baseMaxHealthMultiplier, baseDamageMultiplier) { }
    }

    public class EB30003 : ExpireEnermyPerkBase
    {
        public override int m_Index => 30003;
        public override int m_EffectIndex => 40002;
        public override float m_MaxHealthMultiplierAdditive => base.m_MaxHealthMultiplierAdditive+3f;
        TimerBase m_CooldownTimer = new TimerBase(5f,true);
        TimerBase m_HealTimer = new TimerBase(5f,true);
        public override void OnReceiveDamage(DamageInfo info, float amount)
        {
            base.OnReceiveDamage(info, amount);
            m_CooldownTimer.Replay();
            m_HealTimer.Stop();
        }
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_CooldownTimer.Tick(deltaTime);
            if (m_CooldownTimer.m_Timing)
                return;

            m_HealTimer.Tick(Time.deltaTime);
            if (m_HealTimer.m_Timing)
                return;
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID,-m_Attacher.m_Health.m_MaxHealth/100f, enum_DamageType.Health));
            m_HealTimer.Replay();
        }
        public EB30003(float baseMaxHealthMultiplier, float baseDamageMultiplier) : base(baseMaxHealthMultiplier, baseDamageMultiplier) { }
    }
}