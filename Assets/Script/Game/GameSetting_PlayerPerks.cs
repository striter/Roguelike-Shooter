using GameSetting;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameSetting_PlayerPerks
{
    public class P10000Bonefire : ExpirePlayerPerkBase
    {
        public override int m_Index => 10000;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 50;
        public override float m_MaxHealthAdditive => Value1 * m_Stack;
        public override bool m_DataHidden => true;
        public P10000Bonefire(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10001 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10001;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 3.3f;
        public override float m_DamageAdditive => Value1 * m_Stack;
        public P10001(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10002 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10002;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 20f;
        public override float m_FireRateMultiply => Value1 / 100f * m_Stack;
        public P10002(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10003 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10003;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 20f;
        public override float m_ReloadRateMultiply => Value1 / 100f * m_Stack;
        public P10003(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10004 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10004;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 10f;
        public override float m_CriticalRateAdditive => Value1 / 100f * m_Stack;
        public P10004(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10005 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10005;
        public override enum_Rarity m_Rarity => enum_Rarity.Rare;
        public override float Value1 => 100f;
        public override float m_CriticalHitMultiplyAdditive => Value1 / 100f * m_Stack;
        public P10005(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10006 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10006;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 10f;
        public override void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamage(receiver, info);
            if (info.m_ExpireDamage)
                return;

            float rate = Value1 * m_Stack;
            if (rate >= 100f || TCommon.RandomPercentageInt() <= rate)
                info.AddExtraBuff(GameConst.m_GameDebuffID.RandomItem());
        }
        public P10006(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10007 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10007;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 20;
        public override float m_MaxArmorAdditive => Value1 * m_Stack;
        public P10007(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10008 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10008;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override float Value1 => 200;
        public override float m_MaxHealthAdditive => Value1 * m_Stack;
        public P10008(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10009 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10009;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 10f;
        public override float m_MovementSpeedMultiply => Value1 / 100f * m_Stack;
        public P10009(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10010 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10010;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 2f;
        public override void OnKillEnermy(EntityCharacterBase target)
        {
            base.OnKillEnermy(target);
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID, -Value1 * m_Stack, enum_DamageType.Health,true));
        }
        public P10010(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10011 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10011;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override void OnReceiveHealing(DamageInfo info, float applyAmount)
        {
            base.OnReceiveHealing(info, applyAmount);
            if (info.m_DamageType != enum_DamageType.Health)
                return;
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID, applyAmount*m_Stack, enum_DamageType.Armor,true));
        }
        public P10011(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10012 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10012;
        public override enum_Rarity m_Rarity => enum_Rarity.Rare;
        public override int m_EffectIndex => m_TimerActivate.m_Timing ? 40004 : 0;
        public override int m_MaxStack => 1;
        public override float Value1 => 2f;
        public override float Value2 => 10f;
        public override float m_DamageReduction => m_TimerActivate.m_Timing ? 1f : 0f;
        TimerBase m_TimerCoolDown, m_TimerActivate;
        public override void OnAbilityTrigger()
        {
            base.OnAbilityTrigger();
            if (m_TimerCoolDown.m_Timing)
                return;
            m_TimerActivate.Replay();
            m_TimerCoolDown.Replay();
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_TimerCoolDown.Tick(deltaTime);
            m_TimerActivate.Tick(deltaTime);
        }

        public P10012(PerkSaveData saveData) : base(saveData) { m_TimerActivate = new TimerBase(Value1); m_TimerCoolDown = new TimerBase(Value2); }
    }

    public class P10013 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10013;
        public override enum_Rarity m_Rarity => enum_Rarity.Rare;
        public override float Value1 => 50f;
        public override float F_ClipMultiply => Value1*m_Stack / 100f;
        public P10013(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10014 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10014;
        public override enum_Rarity m_Rarity => enum_Rarity.Rare;
        public override float Value1 => 100f;
        public override void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamage(receiver, info);
            if (receiver.m_Health.m_HealthFull)
                info.AddExtraDamage(Value1/100f*m_Stack , 0);
        }

        public P10014(PerkSaveData saveData) : base(saveData) {  }
    }

    public class P10015:ExpirePlayerPerkWeapon
    {
        public override int m_Index => 10015;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override float Value1 => 25f;
        protected override DamageInfo GetDamageInfo() => new DamageInfo(m_Attacher.m_EntityID, Value1*m_Stack / 100f *m_Attacher.m_WeaponCurrent.m_BaseDamage, enum_DamageType.Basic,105,true);
        public override void OnKillEnermy(EntityCharacterBase target)
        {
            base.OnKillEnermy(target);
            m_SFXWeapon.OnPlay(false, target);
        }
        public P10015(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10016: ExpirePlayerPerkWeapon
    {
        public override int m_Index => 10016;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override int m_MaxStack => 1;
        public override float Value1 => 50f;
        float m_CurrentDamage = 0;
        protected override DamageInfo GetDamageInfo() => new DamageInfo(m_Attacher.m_EntityID, Value1 / 100f * m_CurrentDamage,enum_DamageType.Basic,true);
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (!info.m_CritcalHitted)
                return;
            m_CurrentDamage = info.m_AmountApply;
            m_SFXWeapon.OnPlay(false, receiver);
        }
        public P10016(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10017:ExpirePlayerPerkBase
    {
        public override int m_Index => 10017;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override float Value1 => 3f;
        public override float Value2 => 5f;
        TimerBase m_HealTimer = new TimerBase(3f,true);
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_HealTimer.Tick(deltaTime);
            if (m_HealTimer.m_Timing)
                return;
            if (m_Attacher.m_Health.m_HealthFull)
                return;
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID, -m_Attacher.m_Health.m_MaxHealth *m_Stack* Value2 / 100f, enum_DamageType.Health, true));
            m_HealTimer.Replay();
        }
        public P10017(PerkSaveData saveData) : base(saveData) { m_HealTimer = new TimerBase(Value1,true); }
    }

    public class P10018 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10018;
        public override enum_Rarity m_Rarity => enum_Rarity.Rare;
        public override int m_MaxStack => 1;
        public override float Value1 => 3f;
        public override int m_EffectIndex => m_Timer.m_Timing ? 40004 : 0;
        public override float m_DamageReduction => m_Timer.m_Timing ? 1f : 0;
        bool haveArmor;
        TimerBase m_Timer;
        public override void OnBeforeReceiveDamage(DamageInfo info)
        {
            base.OnBeforeReceiveDamage(info);
            haveArmor = m_Attacher.m_Health.m_CurrentArmor > 0;
        }

        public override void OnReceiveDamage(DamageInfo info, float amount)
        {
            base.OnReceiveDamage(info, amount);
            if (haveArmor && m_Attacher.m_Health.m_CurrentArmor <= 0)
                m_Timer.Replay();
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
        }
        public P10018(PerkSaveData saveData) : base(saveData) { m_Timer = new TimerBase(Value1); }
    }

    public class P10019:ExpirePlayerPerkBase
    {
        public override int m_Index => 10019;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 40f;
        public override float m_HealAdditive => Value1 / 100f*m_Stack;
        public P10019(PerkSaveData saveData) : base(saveData) {}
    }

    public class P10020:ExpirePlayerPerkBase
    {
        public override int m_Index => 10020;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 20f;
        public override float Value2 => 10f;
        public override float m_DamageMultiply => Value1 / 100f * m_Stack;
        public override float m_DamageReduction => -Value2 / 100f * m_Stack;
        public P10020(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10021:ExpirePlayerPerkBase
    {
        public override int m_Index => 10021;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 30f;
        public override float Value2 => 3f;
        public override float m_FireRateMultiply => m_Timer.m_Timing?Value1/100f*m_Stack*m_Timer.m_TimeLeftScale:0;
        TimerBase m_Timer;
        public override void OnKillEnermy(EntityCharacterBase target)
        {
            base.OnKillEnermy(target);
            m_Timer.Replay();
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
        }
        public P10021(PerkSaveData saveData) : base(saveData) { m_Timer = new TimerBase(Value2,false); }
    }

    public class P10022:ExpirePlayerPerkBase
    {
        public override int m_Index => 10022;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 50f;
        public override float Value2 => 2f;
        public override float m_MovementSpeedMultiply => m_Timer.m_Timing? Value1 / 100f * m_Stack:0;
        TimerBase m_Timer;
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
        }
        public override void OnAbilityTrigger()
        {
            base.OnAbilityTrigger();
            m_Timer.Replay();
        }
        public P10022(PerkSaveData saveData) : base(saveData) { m_Timer = new TimerBase(2f); }
    }
    
    public class P10023:ExpirePlayerPerkBase
    {
        public override int m_Index => 10023;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 50f;
        public override void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamage(receiver, info);
            if (receiver.m_CharacterInfo.m_Expires.Any(p => GameConst.m_GameDebuffID.Contains(p.m_Index)))
                info.AddExtraDamage(Value1*m_Stack/100f,0);
        }
        public P10023(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10024:ExpirePlayerPerkBase
    {
        public override int m_Index => 10024;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override float Value1 => 3f;
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (applyAmount <= 0)
                return;
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID,-applyAmount*Value1/100f*m_Stack,enum_DamageType.Health));
        }
        public P10024(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10025:ExpirePlayerPerkBase
    {
        public override int m_Index => 10025;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 5f;
        public override void OnKillEnermy(EntityCharacterBase target)
        {
            base.OnKillEnermy(target);
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID, -Value1 * m_Stack, enum_DamageType.Armor));
        }
        public P10025(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10026:ExpirePlayerPerkBase
    {
        public override int m_Index => 10026;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 20f;
        public override float m_MovementSpeedMultiply => m_Attacher.m_Aiming ? Value1 / 100f * m_Stack : 0;
        public P10026(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10027:ExpirePlayerPerkBase
    {
        public override int m_Index => 10027;
        public override int m_MaxStack => 1;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override bool B_Projectile_Penetrate => true;
        public P10027(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10029:ExpirePlayerPerkWeapon
    {
        public override int m_Index => 10029;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override float Value1 => 5f;
        public override float Value2 => 50f;
        protected override DamageInfo GetDamageInfo() => new DamageInfo(m_Attacher.m_EntityID,expireBaseDamage*Value2/100f*m_Stack,enum_DamageType.Basic,true);
        float expireBaseDamage = 0;
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (info.m_ExpireDamage)
                return;
            expireBaseDamage = applyAmount;
            m_SFXWeapon.OnPlay(false, receiver);
        }
        public P10029(PerkSaveData saveData) : base(saveData) { }
    }
    
    public class P10030:ExpirePlayerPerkWeapon
    {
        public override int m_Index => 10030;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 20f;
        public override float Value2 => 20f;
        protected override DamageInfo GetDamageInfo() => new DamageInfo(m_Attacher.m_EntityID, Value2, enum_DamageType.Basic,true);
        public override void OnAttackSetDamage(DamageInfo damageInfo)
        {
            base.OnAttackSetDamage(damageInfo);
            if (TCommon.RandomPercentageInt() > Value1 * m_Stack)
                return;

            m_SFXWeapon.OnPlay(null,m_Attacher.GetAimingPosition(true));
        }
        public P10030(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10031:ExpirePlayerPerkBase
    {
        public override int m_Index => 10031;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 3;
        public override int I_Projectile_Multi_PelletsAdditive => (int)Value1 * m_Stack;
        public P10031(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10032 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10032;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 50f;
        public override float F_Projectile_Store_TickMultiply => Value1 / 100f * m_Stack;
        public P10032(PerkSaveData saveData) : base(saveData) { }
    }
    public class P10033 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10033;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 50f;
        public override float F_Cast_Melee_SizeMultiply => Value1 / 100f * m_Stack;
        public P10033(PerkSaveData saveData) : base(saveData) { }
    }
    public class ExpirePlayerPerkWeapon : ExpirePlayerPerkBase
    {
        public WeaponHelperBase m_SFXWeapon { get; private set; }
        protected virtual DamageInfo GetDamageInfo()
        {
            Debug.LogError("Override This Please!");
            return null; 
        }

        public override EntityExpireBase OnActivate(EntityCharacterBase _actionEntity)
        {
            m_SFXWeapon = WeaponHelperBase.AcquireWeaponHelper(GameExpression.GetPlayerPerkSFXWeaponIndex(m_Index), _actionEntity, GetDamageInfo);
            return base.OnActivate(_actionEntity);
        }
        public ExpirePlayerPerkWeapon(PerkSaveData data):base(data) { }
    }
}
