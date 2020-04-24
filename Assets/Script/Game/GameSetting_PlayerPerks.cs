using GameSetting;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameSetting_Action
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
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
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
        static readonly List<int> m_BuffID = new List<int>() { 103, 104, 105 };
        public override void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamage(receiver, info);
            for (int i = 0; i < m_Stack; i++)
            {
                if (TCommon.RandomPercentage() < Value1)
                    info.AddExtraBuff(m_BuffID.RandomItem());
            }
        }
        public P10006(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10007 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10007;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 5;
        public override float m_MaxArmorAdditive => Value1 * m_Stack;
        public P10007(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10008 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10008;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 10;
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
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override float Value1 => 2f;
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (receiver.m_IsDead)
                m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID ,-Value1 * m_Stack, enum_DamageType.Health));
        }
        public P10010(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10011 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10011;
        public override enum_Rarity m_Rarity => enum_Rarity.Rare;
        public override int m_MaxStack => 1;
        public override float Value1 => 3f;
        public override void OnAbilityTrigger()
        {
            base.OnAbilityTrigger();
            float sqrDistance = Value1;
            sqrDistance *= sqrDistance;
            GameObjectManager.TraversalAllSFXWeapon((SFXWeaponBase weapon) =>
            {
                if (!weapon.B_Playing)
                    return;

                SFXProjectile projectile = weapon as SFXProjectile;
                if (projectile == null || !GameManager.Instance.EntityOpposite(weapon.m_SourceID, m_Attacher.m_EntityID))
                    return;

                if (TCommon.GetXZSqrDistance(weapon.transform.position, m_Attacher.transform.position) > sqrDistance)
                    return;

                projectile.Stop();
            });
        }
        public P10011(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10012 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10012;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
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
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override int m_MaxStack => 1;
        public override float Value1 => 1;
        public override void OnAttackSetDamage(DamageInfo info)
        {
            base.OnAttackSetDamage(info);
            if (!info.m_CritcalHitted)
                return;

            m_Attacher.m_WeaponCurrent.AddAmmo(1);
        }
        public P10013(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10014 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10014;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override int m_EffectIndex => m_TimerActivate.m_Timing ? 40004 : 0;
        public override int m_MaxStack => 1;
        public override float Value1 => 10f;
        public override float Value2 => 3f;
        TimerBase m_TimerCoolDown, m_TimerActivate;
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            m_TimerActivate.Tick(deltaTime);
            m_TimerCoolDown.Tick(deltaTime);

            if (m_TimerCoolDown.m_Timing)
                return;
            m_TimerCoolDown.Replay();
            m_TimerActivate.Replay();
        }

        public P10014(PerkSaveData saveData) : base(saveData) { m_TimerActivate = new TimerBase(Value2); m_TimerCoolDown = new TimerBase(Value1); }
    }

    public class P10015:ExpirePlayerPerkBase
    {
        public override int m_Index => 10015;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override int m_MaxStack => 1;
        public override bool OnCheckRevive()
        {
            m_Expired = true;
            return true;
        }
        public P10015(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10016: PerkSFXweapon
    {
        public override int m_Index => 10016;
        public override enum_Rarity m_Rarity => enum_Rarity.Epic;
        public override int m_MaxStack => 1;
        public override float Value1 => 50f;
        float m_CurrentDamage = 0;
        protected override DamageInfo GetDamageInfo() => new DamageInfo(m_Attacher.m_EntityID, Value1 / 100f * m_CurrentDamage,enum_DamageType.Basic);
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

    public class P10017:PerkSFXweapon
    {
        public override int m_Index => 10017;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override int m_MaxStack => 1;
        public override float Value1 => 25f;
        protected override DamageInfo GetDamageInfo() => new DamageInfo(m_Attacher.m_EntityID,  Value1 / 100f * m_Attacher.m_WeaponCurrent.m_BaseDamage,  enum_DamageType.Basic);
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (!receiver.m_IsDead)
                return;
            m_SFXWeapon.OnPlay(false, receiver);
        }
        public P10017(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10018 : ExpirePlayerPerkBase
    {
        public override int m_Index => 10018;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
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
        public override float Value2 => 20f;
        public override float m_DamageMultiply => Value1 / 100f * m_Stack;
        public override float m_DamageReduction => -Value2 / 100f * m_Stack;
        public P10020(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10021:ExpirePlayerPerkBase
    {
        public override int m_Index => 10021;
        public override bool m_DataHidden => true;
        public P10021(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10022:ExpirePlayerPerkBase
    {
        public override int m_Index => 10022;
        public override enum_Rarity m_Rarity => enum_Rarity.Rare;
        public override float Value1 => 3f;
        public override float Value2 => 2f;
        public override float m_FireRateMultiply => (m_Attacher.m_Health.m_MaxHealth - m_Attacher.m_Health.m_CurrentHealth) / Value1 * Value2*m_Stack / 100f;
        public P10022(PerkSaveData saveData) : base(saveData) { }
    }
    
    public class P10023:ExpirePlayerPerkBase
    {
        public override int m_Index => 10023;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 5f;
        public override float m_DamageAdditive => m_Attacher.m_Health.m_HealthFull ? Value1 * m_Stack : 0;
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
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (receiver.m_IsDead)
                m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID ,- Value1 * m_Stack, enum_DamageType.Armor));
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
        public override enum_Rarity m_Rarity => enum_Rarity.Rare;
        public override int m_MaxStack => 1;
        public override float Value1 => 20f;
        public override float F_Discount => Value1 / 100f;
        public P10027(PerkSaveData saveData) : base(saveData) { }
    }

    public class P10028:ExpirePlayerPerkBase
    {
        public override int m_Index => 10028;
        public override bool m_DataHidden => true;
        public P10028(PerkSaveData saveData) : base(saveData) { }
    }

    
    public class P10030:PerkSFXweapon
    {
        public override int m_Index => 10030;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override float Value1 => 20f;
        public override float Value2 => 20f;
        protected override DamageInfo GetDamageInfo() => new DamageInfo(m_Attacher.m_EntityID, Value2, enum_DamageType.Basic);
        public override void OnAttackSetDamage(DamageInfo damageInfo)
        {
            base.OnAttackSetDamage(damageInfo);
            if (TCommon.RandomPercentage() > Value1 * m_Stack)
                return;

            m_SFXWeapon.OnPlay(null,m_Attacher.GetAimingPosition());
        }
        public P10030(PerkSaveData saveData) : base(saveData) { }
    }

    public class PerkSFXweapon : ExpirePlayerPerkBase
    {
        public WeaponHelperBase m_SFXWeapon { get; private set; }
        protected virtual DamageInfo GetDamageInfo()
        {
            Debug.LogError("Override This Please!");
            return null; 
        }

        public override EntityExpireBase OnActivate(EntityCharacterBase _actionEntity)
        {
            m_SFXWeapon = WeaponHelperBase.AcquireWeaponHelper(GameExpression.GetPlayerPerkSFXWeaponIndex(m_Index), m_Attacher, GetDamageInfo);
            return base.OnActivate(_actionEntity);
        }
        public PerkSFXweapon(PerkSaveData data):base(data) { }
    }
}
