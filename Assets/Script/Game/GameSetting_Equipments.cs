using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

namespace GameSetting_Equipments
{
    public class E20000:ExpireEquipmentBase
    {
        public override int m_Index => 20000;

        public override float Value1 => 10f;
        public override float Value2 => 5f;
        public override float m_CriticalRateAdditive => m_Timer.m_Timing ? Value1/100f : 0f;
        TimeCounter m_Timer;

        public override void OnAbilityTrigger()
        {
            base.OnAbilityTrigger();
            if (m_PassiveActivated)
                m_Timer.Replay();
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
        }

        public E20000(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) { m_Timer = new TimeCounter(Value2, true); }
    }

    public class E20001:ExpireEquipmentBase
    {
        public override int m_Index => 20001;
        public override float Value1 => 20;
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (!m_PassiveActivated || !receiver.m_IsDead)
                return;
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID,-m_Attacher.m_Health.m_MaxHealth*Value1/100f,enum_DamageType.HealthPenetrate));
        }
        public E20001(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) {  }
    }

    public class E20002:ExpireEquipmentBase
    {
        public override int m_Index => 20002;
        public override float Value1 => 8f;
        public override float Value2 => 80f;
        protected int m_TargetID;
        protected  float m_DamageStackUp=0f;
        public override void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamage(receiver, info);
            if (!m_PassiveActivated)
                return;

            m_DamageStackUp = m_TargetID == receiver.m_EntityID ? Mathf.Clamp(m_DamageStackUp +Value1,0,Value2): 0;
            info.AddExtraDamage(m_DamageStackUp/100f,0f);
            m_TargetID = receiver.m_EntityID;
        }

        public E20002(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) { }
    }

    public class E20003_Test0:ExpireEquipmentBase
    {
        public override int m_Index => 20003;
        public override float m_DamageAdditive => m_PassiveActivated ? 50f : 0 + base.m_DamageAdditive;
        public E20003_Test0(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) { }
    }
    
    public class E20004_Test1 : ExpireEquipmentBase
    {
        public override int m_Index => 20004;
        public override float m_MovementSpeedMultiply =>m_PassiveActivated?.5f:0+ base.m_MovementSpeedMultiply;
        public E20004_Test1(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) { }
    }

    public class E20005_Test2 : ExpireEquipmentBase
    {
        public override int m_Index => 20005;
        public override float m_CriticalRateAdditive => m_PassiveActivated ? .5f : 0 + base.m_CriticalRateAdditive;
        public E20005_Test2(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) { }
    }
}