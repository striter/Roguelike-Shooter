using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

namespace GameSetting_Equipments
{
    public class E20000_Test0:ExpireEquipmentBase
    {
        public override int m_Index => 20000;
        public override float m_DamageAdditive => m_PassiveActivated ? 50f : 0 + base.m_DamageAdditive;
        public E20000_Test0(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) { }
    }
    
    public class E20001_Test1 : ExpireEquipmentBase
    {
        public override int m_Index => 20001;
        public override float m_MovementSpeedMultiply =>m_PassiveActivated?.5f:0+ base.m_MovementSpeedMultiply;
        public E20001_Test1(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) { }
    }

    public class E20002 : ExpireEquipmentBase
    {
        public override int m_Index => 20002;
        public override float m_CriticalRateAdditive => m_PassiveActivated ? .5f : 0 + base.m_CriticalRateAdditive;
        public E20002(enum_EquipmentPassitveType passiveActivate, EquipmentSaveData data) : base(passiveActivate, data) { }
    }
}