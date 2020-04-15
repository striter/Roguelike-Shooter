using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

namespace GameSetting_Equipments
{
    public class E20000_Default:ExpireUpgrade
    {
        public override int m_Index => GameConst.m_DefaultEquipmentCombinationIdentity;

        public E20000_Default(List<EquipmentSaveData> equipmentUpgrade,CharacterUpgradeData characterUpgrade) : base(equipmentUpgrade,characterUpgrade) {  }
    }


    public class E20001:ExpireUpgrade
    {
        public override int m_Index => 20001;

        public override float Value1 => 10f;
        public override float Value2 => 5f;
        public override float m_CriticalRateAdditive => m_Timer.m_Timing ? Value1/100f : 0f;
        TimerBase m_Timer;

        public override void OnAbilityTrigger()
        {
            base.OnAbilityTrigger();
            m_Timer.Replay();
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
        }

        public E20001(List<EquipmentSaveData> equipmentUpgrade, CharacterUpgradeData characterUpgrade) : base(equipmentUpgrade, characterUpgrade) { m_Timer = new TimerBase(Value2, true); }
    }

    public class E20002:ExpireUpgrade
    {
        public override int m_Index => 20002;
        public override float Value1 => 20;
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (! !receiver.m_IsDead)
                return;
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID,-m_Attacher.m_Health.m_MaxHealth*Value1/100f,enum_DamageType.HealthPenetrate));
        }
        public E20002(List<EquipmentSaveData> equipmentUpgrade, CharacterUpgradeData characterUpgrade) : base(equipmentUpgrade, characterUpgrade) { }
    }

    public class E20003:ExpireUpgrade
    {
        public override int m_Index => 20003;
        public override float Value1 => 8f;
        public override float Value2 => 80f;
        protected int m_TargetID;
        protected  float m_DamageStackUp=0f;
        public override void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamage(receiver, info);
            m_DamageStackUp = m_TargetID == receiver.m_EntityID ? Mathf.Clamp(m_DamageStackUp +Value1,0,Value2): 0;
            info.AddExtraDamage(m_DamageStackUp/100f,0f);
            m_TargetID = receiver.m_EntityID;
        }

        public E20003(List<EquipmentSaveData> equipmentUpgrade, CharacterUpgradeData characterUpgrade) : base(equipmentUpgrade, characterUpgrade) { }
    }
}