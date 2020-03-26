using GameSetting;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameSetting_Action
{
    public class P0000Bonefire : ExpirePerkBase
    {
        public override int m_Index => 0;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override int m_MaxStack => -1;
        public override float Value1 => 50;
        public override float m_MaxHealthAdditive => Value1 * m_Stack;
        public override bool m_Hidden => true;
        public P0000Bonefire(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0001 : ExpirePerkBase
    {
        public override int m_Index => 0001;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override int m_MaxStack => -1;
        public override float Value1 => 3.3f;
        public override float m_DamageAdditive => Value1 * m_Stack;
        public P0001(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0002 : ExpirePerkBase
    {
        public override int m_Index => 0002;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override int m_MaxStack => -1;
        public override float Value1 => 20f;
        public override float m_FireRateMultiply => Value1 / 100f * m_Stack;
        public P0002(PerkSaveData saveData) : base(saveData) { }
    }


    public class P0003 : ExpirePerkBase
    {
        public override int m_Index => 0003;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override int m_MaxStack => -1;
        public override float Value1 => 20f;
        public override float m_ReloadRateMultiply => Value1 / 100f * m_Stack;
        public P0003(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0004:ExpirePerkBase
    {
        public override int m_Index => 0004;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override int m_MaxStack => -1;
        public override float Value1 => 10f;
        public override float m_CriticalChangeAdditive => Value1 / 100f *m_Stack;
        public P0004(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0005:ExpirePerkBase
    {
        public override int m_Index => 0005;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override int m_MaxStack => -1;
        public override float Value1 => 30f;
        public override float m_CriticalHitMultiplyAdditive => Value1 / 100f * m_Stack;
        public P0005(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0006:ExpirePerkBase
    {
        public override int m_Index => 0006;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override int m_MaxStack => -1;
        public override float Value1 => 10f;
        static readonly List<int> m_BuffID = new List<int>() { 14010, 14020, 14030 };
        public override void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamage(receiver, info);
            for(int i=0;i<m_Stack;i++)
            {
                if (TCommon.RandomPercentage() < Value1)
                    info.m_detail.AddExtraBuff(m_BuffID.RandomItem());
            }
        }
        public P0006(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0007:ExpirePerkBase
    {
        public override int m_Index => 0007;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override int m_MaxStack => -1;
        public override float Value1 => 5;
        public override float m_MaxArmorAdditive => Value1 * m_Stack;
        public P0007(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0008 : ExpirePerkBase
    {
        public override int m_Index => 0008;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override int m_MaxStack => -1;
        public override float Value1 => 10;
        public override float m_MaxHealthAdditive => Value1 * m_Stack;
        public P0008(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0009:ExpirePerkBase
    {
        public override int m_Index => 0009;
        public override enum_Rarity m_Rarity => enum_Rarity.Ordinary;
        public override int m_MaxStack => -1;
        public override float Value1 => 10f;
        public override float m_MovementSpeedMultiply => Value1/100f * m_Stack;
        public P0009(PerkSaveData saveData) : base(saveData) { }
    }

    public class P0010:ExpirePerkBase
    {
        public override int m_Index => 0010;
        public override enum_Rarity m_Rarity => enum_Rarity.Advanced;
        public override int m_MaxStack => -1;
        public override float Value1 => 2f;
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (receiver.m_IsDead)
                m_Attacher.m_HitCheck.TryHit(new DamageInfo(Value1*m_Stack, enum_DamageType.HealthOnly,DamageDeliverInfo.Default(-1)));
        }
        public P0010(PerkSaveData saveData) : base(saveData) { }
    }
}
#region DataHelper
public static class PerkDataManager
{
    static Dictionary<enum_Rarity, List<int>> m_PerkRarities = new Dictionary<enum_Rarity, List<int>>();
    static Dictionary<int, ExpirePerkBase> m_AllPerks = new Dictionary<int, ExpirePerkBase>();
    public static void Init()
    {
        m_AllPerks.Clear();
        m_PerkRarities.Clear();
        TReflection.TraversalAllInheritedClasses(((Type type, ExpirePerkBase action) => {
            m_AllPerks.Add(action.m_Index, action);
            if (!m_PerkRarities.ContainsKey(action.m_Rarity))
                m_PerkRarities.Add(action.m_Rarity, new List<int>());
            m_PerkRarities[action.m_Rarity].Add(action.m_Index);
        }), PerkSaveData.New(-1));
    }
    public static int RandomPerkIndex(enum_Rarity rarity, System.Random seed) => m_AllPerks.RandomKey(seed);

    public static ExpirePerkBase GetPerkData(int index) => m_AllPerks[index];

    public static ExpirePerkBase CreatePerk(PerkSaveData data)
    {
        if (!m_AllPerks.ContainsKey(data.m_Index))
            Debug.LogError("Error Action Equipment:" + data.m_Index + " ,Does not exist");
        ExpirePerkBase equipment = TReflection.CreateInstance<ExpirePerkBase>(m_AllPerks[data.m_Index].GetType(), data);
        return equipment;
    }
}
#endregion