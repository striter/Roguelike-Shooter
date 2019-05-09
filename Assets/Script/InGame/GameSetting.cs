using UnityEngine;
using TExcel;
using System.Collections.Generic;
#pragma warning disable 0649
namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
        public static readonly int I_BulletMaxLastTime = 5; // No Collision Recycle Time
        public static readonly float I_BulletSpeedForward = 30f;  //Meter Per Second
        public static readonly float I_BulletSpeedDownward =30f;  //Meter Per Second
        
    }

    public static class GameExpression
    {

    }


    public static class UIConst
    {
        public static readonly int I_SporeManagerContainersMaxAmount = 9;  //Max Amount Of SporeManager Container
        public static readonly int I_SporeManagerContainerStartFreeSlot = 3;    //Free Slot For New Player
        public static readonly int I_SporeManagerTickOffsetEach = 60;       //Seconds
        public static readonly int I_SporeManagerContainerStartRandomEnd = 30;      // Start From 0 
        public static readonly int I_SporeManagerAutoSave = 5;      //Per Seconds Auto Save Case Game Crush
    }

    public static class UIExpression
    {
        public static float F_SporeManagerPorfitPerSecond(int level) => level == 1 ? 1f : Mathf.Pow(1.5f, (level - 1));     //Coins Profit Per Second 
        public static float F_SporeManagerChestCoinRequirement(int maxLevel) => 10 * Mathf.Pow(1.8f, (maxLevel - 1));       //Coin Requirement Per Chest
        public static float F_SporeManagerChestBlueRequirement(int maxLevel) => 100 * Mathf.Pow(1.05f, (maxLevel - 1));       //Blue Requirement Per Chest
    }
    #endregion
    #region For Developers Use
    #region BroadCastEnum
    enum enum_BC_UIStatusChanged
    {
        Invalid = -1,
        AmmoLeftChanged,
        PitchChanged,
    }
    #endregion
    #region GameEnum
    public enum enum_HitCheck
    {
        Invalid=-1,
        Static=1,
        Entity=2,
    }
    public enum enum_Weapon
    {
        Invalid=-1,
        Rifle=1,
        SnipeRifle=2,
    }
    public enum enum_Entity
    {
        Invalid=-1,
        Player=1,
        Dummy=2,
    }
    public enum enum_SFX
    {
        Invalid=-1,
        Bullet=1,
    }
    public static class GameEnum_Extend
    {
        public static int ToLayer(this enum_HitCheck layerType)
        {
            switch (layerType)
            {
                default:
                    Debug.LogError("Null Layer Can Be Transferd From:" + layerType.ToString());
                    return 0;
                case enum_HitCheck.Entity:
                    return GameLayer.I_Entity;
                case enum_HitCheck.Static:
                    return GameLayer.I_Static;
            }
        }
    }
    #endregion
    #region GameLayer
    public static class GameLayer
    {
        public static readonly int I_Static = LayerMask.NameToLayer("static");
        public static readonly int I_Entity = LayerMask.NameToLayer("entity");
    }
    #endregion
    #region GameSave
    public class CPlayerSave : ISave
    {
        public float f_blue;
        public CPlayerSave()
        {
            f_blue = 100;
        }
    }

    #endregion
    #region GameStruct
    public struct SEntity : ISExcel
    {
        int index;
        string s_name;
        int i_maxHealth;
        float f_moveSpeed;
        public enum_Entity m_Type =>(enum_Entity) index;
        public string m_Name => s_name;
        public int m_MaxHealth => i_maxHealth;
        public float m_moveSpeed => f_moveSpeed;
    }

    public struct SWeapon : ISExcel
    {
        int index;
        string s_name;
        float f_damage;
        float f_fireRate;
        int i_clipAmount;
        float f_reloadTime;
        float f_recoilHorizontal;
        float f_recoilVertical;
        public enum_Weapon m_Type => (enum_Weapon)index;
        public string m_Name => s_name;
        public float m_Damage => f_damage;
        public float m_FireRate => f_fireRate;
        public int m_ClipAmount => i_clipAmount;
        public float m_ReloadTime => f_reloadTime;
        public float m_RecoilHorizontal => f_recoilHorizontal;
        public float m_RecoilVertical => f_recoilVertical;
    }

    #endregion
    #region For UI Usage     
    class CSporeManagerSave : ISave     //Locked=-1 Spare=1
    {
        public float f_coin;
        public int i_maxLevel;
        public double d_timePassed;
        protected Dictionary<int, int> d_sporeContainerInfo;
        public int I_SlotCount => d_sporeContainerInfo.Count;
        public int this[int slotIndex]
        {
            set
            {
                if (!d_sporeContainerInfo.ContainsKey(slotIndex))
                    Debug.LogError("Invalid Slot Index:" + slotIndex);

                d_sporeContainerInfo[slotIndex] = value;
            }
            get
            {
                if (!d_sporeContainerInfo.ContainsKey(slotIndex))
                    Debug.LogError("Invalid Slot Index:" + slotIndex);

                return d_sporeContainerInfo[slotIndex];
            }
        }
        public void AddSlotValue(int slotIndex, int value = 1)
        {

            if (!d_sporeContainerInfo.ContainsKey(slotIndex))
                Debug.LogError("Invalid Slot Index:" + slotIndex);

            d_sporeContainerInfo[slotIndex] += value;
        }

        public int GetSpareSlot()
        {
            int spareSlot = -1;
            foreach (int slot in d_sporeContainerInfo.Keys)
            {
                if (d_sporeContainerInfo[slot] == 0)
                {
                    spareSlot = slot;
                    break;
                }
            }
            return spareSlot;
        }
        public void UnlockNewSlot()
        {
            foreach (int slot in d_sporeContainerInfo.Keys)
                if (d_sporeContainerInfo[slot] == -1)     //If Got New Slot Unlock it
                {
                    d_sporeContainerInfo[slot] = 0;
                    break;
                }
        }
        public CSporeManagerSave()
        {
            f_coin = 30;
            i_maxLevel = 1;
            d_timePassed = 0;
            d_sporeContainerInfo = new Dictionary<int, int>() { };
            for (int i = 1; i <= UIConst.I_SporeManagerContainersMaxAmount; i++)
                d_sporeContainerInfo.Add(i, i <= UIConst.I_SporeManagerContainerStartFreeSlot ? 0 : -1);
        }
    }

    public struct SSporeLevel : ISExcel
    {
        int maxLevel;
        float offset0;
        float offset1;
        float offset2;
        float offset3;
        float offset4;
        float offset5;
        float offset6;
        float offset7;
        float offset8;
        float offset9;
        bool addSlot;
        public int MaxLevel => maxLevel;
        public float F_OffSet0 => offset0;
        public float F_OffSet1 => offset1;
        public float F_OffSet2 => offset2;
        public float F_OffSet3 => offset3;
        public float F_OffSet4 => offset4;
        public float F_OffSet5 => offset5;
        public float F_OffSet6 => offset6;
        public float F_OffSet7 => offset7;
        public float F_OffSet8 => offset8;
        public float F_OffSet9 => offset9;
        public bool B_AddSlot => addSlot;
    }

    public class SSporeLevelRate
    {
        SSporeLevel m_BaseInfo;
        public float F_CoinChestPrice;
        public float F_BlueChestPrice;
        public int I_Level => m_BaseInfo.MaxLevel;
        public bool B_AddSlot => m_BaseInfo.B_AddSlot;
        public List<float> l_sporeRates;
        public SSporeLevelRate(SSporeLevel rate)
        {
            m_BaseInfo = rate;
            l_sporeRates = new List<float>() { m_BaseInfo.F_OffSet0, m_BaseInfo.F_OffSet1, m_BaseInfo.F_OffSet2, m_BaseInfo.F_OffSet3, m_BaseInfo.F_OffSet4, m_BaseInfo.F_OffSet5, m_BaseInfo.F_OffSet6, m_BaseInfo.F_OffSet7, m_BaseInfo.F_OffSet8, m_BaseInfo.F_OffSet9 };
            float count = 0;
            for (int i = 0; i < l_sporeRates.Count; i++)
                count += l_sporeRates[i];
            if (I_Level != -1 && count != 100)
                Debug.LogError("Spore Rate Total Unmatch 100! Line:" + I_Level);
            F_CoinChestPrice = UIExpression.F_SporeManagerChestCoinRequirement(I_Level);
            F_BlueChestPrice = UIExpression.F_SporeManagerChestBlueRequirement(I_Level);
        }

        public int AcquireNewSpore()
        {
            int random = Random.Range(1, 101);
            float count = 0;
            int offset = 0;
            for (int i = 0; i < l_sporeRates.Count; i++)
            {
                count += l_sporeRates[i];
                if (random < count)
                {
                    offset = i;
                    break;
                }
            }
            return I_Level - offset;
        }
    }
    #endregion
    #endregion
}
