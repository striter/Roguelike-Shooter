using UnityEngine;
using TExcel;
using System.Collections.Generic;
using System;
#pragma warning disable 0649
namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
        public const int I_NormalBulletLastTime = 5; // No Collision Recycle Time
        public const int I_BoltMaxLastTime = 10;
        public const int I_LaserMaxLastTime = 5;

        public const int I_RocketBlastRadius = 5;        //Meter
        public const float F_LaserRayStartPause = .5f;      //Laser Start Pause

        public const int I_BurstFirePelletsOnceTrigger = 3;       //Times While Burst Fire
        public const int I_BulletSpeadAtDistance = 100;       //Meter,  Bullet Spread In A Circle At End Of This Distance
    }

    public static class GameExpression
    {
        public static int I_EntityID(int index, bool isPlayer) => index + (isPlayer ? 10000 : 20000);

        public static float F_RocketBlastDamage(float weaponDamage, float distance) => weaponDamage * (distance / GameConst.I_RocketBlastRadius);

        public static bool B_CanHitTarget(HitCheckEntity hb, int sourceID) => hb.I_AttacherID != sourceID;
    }


    public static class UIConst
    {
        public const int I_SporeManagerUnitTime = 60;//Seconds
        public const int I_SporeManagerContainersMaxAmount = 9;  //Max Amount Of SporeManager Container
        public const int I_SporeManagerContainerStartFreeSlot = 3;    //Free Slot For New Player
        public const int I_SporeManagerContainerTickTime = 60;       //Seconds
        public const int I_SporeManagerContainerStartRandomEnd = 30;      // Start From 0 
        public const int I_SporeManagerAutoSave = 5;      //Per Seconds Auto Save Case Game Crush
        public const int I_SporeManagerHybridMaxLevel = 40;      //Spore Level Equals Won't Hybrid
    }

    public static class UIExpression
    {
        public static float F_SporeManagerProfitPerMinute(int level) => 10 *Mathf.Pow(1.3f,level-1);     //Coins Profit Per Unit Time
        public static float F_SporeManagerCoinsMaxAmount(int level) => 100 * Mathf.Pow(10 * Mathf.Pow(1.3f , level - 1) , 1.03f) + 400 * Mathf.Pow(1.4f , level - 1) ;
        //Coins Max Amount Each Level
        public static float F_SporeManagerChestCoinRequirement(int maxLevel) => 200 * Mathf.Pow(1.4f, maxLevel - 1);       //Coin Requirement Per Chest
        public static float F_SporeManagerChestBlueRequirement(int maxLevel) => 100 * Mathf.Pow(1.05f, maxLevel - 1);       //Blue Requirement Per Chest
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

    public enum enum_Entity     //Preset For Entities
    {
        Invalid = -1,
        //Player
        Player = 1,
        //Enermy
        Dummy = 2,
    }

    public enum enum_SFX        //Preset For SFX
    {
        Invalid = -1,
        Bullet_Normal = 1,
        Bullet_LaserRay = 2,
        Bullet_LaserBeam = 3,
        Bullet_Bolt = 4,
        Bullet_Rocket = 5,
        Blast_Rocket = 6,
    }

    public enum enum_HitCheck
    {
        Invalid = -1,
        Static = 1,
        Entity = 2,
        Dynamic = 3,
    }
    public enum enum_Weapon
    {
        Invalid = -1,
        //Laser
        LaserPistol = 1001,
        LaserCannon = 1002,
        //Snipe Rifle
        MK10 = 2001,
        Kar98 = 2002,
        //Submachine Gun
        UZI = 3001,
        UMP45 = 3002,
        //Assult Rifle
        SCAR = 4001,
        M16A4 = 4002,
        AKM = 4003,
        //Pistol
        P92 = 5001,
        DE = 5002,
        //Shotgun
        XM1014 = 6001,
        S686 = 6002,
        //Heavy Weapon
        Crossbow = 7001,
        RocketLauncher = 7002,
    }
    public enum enum_TriggerType
    {
        Invalid = -1,
        Single = 1,
        Auto = 2,
        Burst = 3,
        Pull = 4,
        Store = 5,
    }
    public enum enum_BulletType
    {
        Invalid = -1,
        Normal = 1,
        LaserRay = 2,
        LaserBeam = 3,
        Bolt = 4,
        Rocket = 5,
    }

    public static class GameEnum_Extend
    {
        public static enum_SFX ToSFXType(this enum_BulletType type)
        {
            switch (type)
            {
                default: Debug.LogError("Insert More Convertions Here:" + type.ToString()); return enum_SFX.Invalid;
                case enum_BulletType.Normal: return enum_SFX.Bullet_Normal;
                case enum_BulletType.LaserRay: return enum_SFX.Bullet_LaserRay;
                case enum_BulletType.LaserBeam: return enum_SFX.Bullet_LaserBeam;
                case enum_BulletType.Bolt: return enum_SFX.Bullet_Bolt;
                case enum_BulletType.Rocket: return enum_SFX.Bullet_Rocket;
            }
        }
        public static int ToLayer(this enum_HitCheck layerType)
        {
            switch (layerType)
            {
                default: Debug.LogError("Null Layer Can Be Transferd From:" + layerType.ToString()); return 0;
                case enum_HitCheck.Entity: return GameLayer.I_Entity;
                case enum_HitCheck.Static: return GameLayer.I_Static;
            }
        }
    }
    #endregion
    #region GameLayer
    public static class GameLayer
    {
        public static readonly int I_Static = LayerMask.NameToLayer("static");
        public static readonly int I_Entity = LayerMask.NameToLayer("entity");
        public static readonly int I_Dynamic = LayerMask.NameToLayer("dynamic");
        public static class Physics
        {
            public static readonly int I_All = 1 << I_Static | 1 << I_Entity | 1 << I_Dynamic;
            public static readonly int I_EntityOnly = (1 << I_Entity);
        }
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
    #region GameClass
    class HitCheckDetect
    {
        Action<HitCheckStatic> OnHitCheckStatic;
        Action<HitCheckDynamic> OnHitCheckDynamic;
        Action<HitCheckEntity> OnHitCheckEntity;
        Action OnHitCheckError;
        public HitCheckDetect(Action<HitCheckStatic> _OnHitCheckStatic, Action<HitCheckDynamic> _OnHitCheckDynamic, Action<HitCheckEntity> _OnHitCheckEntity, Action _OnHitCheckError)
        {
            OnHitCheckStatic = _OnHitCheckStatic;
            OnHitCheckDynamic = _OnHitCheckDynamic;
            OnHitCheckEntity = _OnHitCheckEntity;
            OnHitCheckError = _OnHitCheckError;
        }
        public void DoDetect(Collider other)
        {
            HitCheckBase hitCheck = other.GetComponent<HitCheckBase>();
            if (hitCheck == null)
            {
                Debug.LogWarning("Null Hit Check Attached:" + other.gameObject);
                OnHitCheckError();
                return;
            }
            switch (hitCheck.m_HitCheckType)
            {
                default: Debug.LogError("Add More Convertions Here:" + hitCheck.m_HitCheckType); break;
                case enum_HitCheck.Static:
                    OnHitCheckStatic(hitCheck as HitCheckStatic);
                    break;
                case enum_HitCheck.Dynamic:
                    OnHitCheckDynamic(hitCheck as HitCheckDynamic);
                    break;
                case enum_HitCheck.Entity:
                    OnHitCheckEntity(hitCheck as HitCheckEntity);
                    break;
            }
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
        int i_triggerType;
        int i_bulletType;
        float f_damage;
        float f_fireRate;
        float f_specialRate;
        float f_manaCost;
        int i_clipAmount;
        float f_spread;
        float f_reloadTime;
        int i_PelletsPerShot;
        float f_stunAfterShot;
        float f_horizontalSpeed;
        float f_horizontalDrag;
        float f_verticalAcceleration;
        float f_recoilHorizontal;
        float f_recoilVertical;

        public enum_Weapon m_Weapon => (enum_Weapon)index;
        public string m_Name => s_name;
        public enum_TriggerType m_TriggerType=>(enum_TriggerType)i_triggerType;
        public enum_BulletType m_BulletType => (enum_BulletType)i_bulletType;
        public float m_Damage => f_damage;
        public float m_FireRate => f_fireRate;
        public float m_SpecialRate => f_specialRate;
        public float m_ManaCost => f_manaCost;
        public int m_ClipAmount => i_clipAmount;
        public float m_Spread => f_spread;
        public float m_ReloadTime => f_reloadTime;
        public int m_PelletsPerShot => i_PelletsPerShot;
        public float m_stunAfterShot => f_stunAfterShot;
        public float m_HorizontalSpeed => f_horizontalSpeed;
        public float m_HorizontalDrag => f_horizontalDrag;
        public float m_VerticalAcceleration => f_verticalAcceleration;
        public Vector2 m_RecoilPerShot => new Vector2(f_recoilHorizontal, f_recoilVertical);
    }

    #endregion
    #region For UI Usage     
    class CSporeManagerSave : ISave     //Locked=-1 Spare=1
    {
        public float f_coin;
        public int i_maxLevel;
        public int i_previousTimeStamp;
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
            f_coin = 1000f;
            i_maxLevel = 1;
            d_timePassed = 0;
            i_previousTimeStamp = TCommon.GetTimeStamp(DateTime.Now);
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
        public float F_CoinChestPrice { get; private set; }
        public float F_BlueChestPrice { get; private set; }
        public float F_MaxCoinsAmount { get; private set; }
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
            F_MaxCoinsAmount = UIExpression.F_SporeManagerCoinsMaxAmount(I_Level);
        }

        public int AcquireNewSpore()
        {
            int random = UnityEngine.Random.Range(1, 101);
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
