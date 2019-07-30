using UnityEngine;
using TExcel;
using System.Collections.Generic;
using System;
using TTiles;
using TPhysics;
using System.Linq;
#pragma warning disable 0649
namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
        public const int I_ProjectileMaxDistance = 100;

        public const short I_BoltLastTimeAfterHit = 5;

        public const float F_LaserRayStartPause = .5f;      //Laser Start Pause

        public const int I_BurstFirePelletsOnceTrigger = 3;       //Times While Burst Fire
        public const int I_ProjectileSpreadAtDistance = 100;       //Meter,  Bullet Spread In A Circle At End Of This Distance

        public const float F_LevelTileSize = 2f;        //Cube Size For Level Tiles

        public const float F_DamagePlayerFallInOcean = 10f;        //Player Ocean Fall Damage

        public const float F_PlayerCameraSmoothParam = 1f;     //Camera Smooth Param For Player .2 is suggested
        public const float F_PlayerFallSpeed = 9.8f;       //Player Fall Speed(Not Acceleration)

        public const int I_TileMapPortalMinusOffset = 3;        //The Minimum Tile Offset Away From Origin Portal Will Generate

        public const float F_EnermyAICheckTime = .3f;       //AI Check Offset Time, 0.3 is suggested;

        public const int I_EnermyCountWaveFinish = 0;       //When Total Enermy Count Reaches This Amount,Wave Finish
        public const int I_EnermySpawnDelay = 2;        //Enermy Spawn Delay Time 
    }

    public static class GameExpression
    {
        public static Vector3 V3_TileAxisOffset(TileAxis axis) => new Vector3(axis.X * GameConst.F_LevelTileSize, 0, axis.Y * GameConst.F_LevelTileSize);
        public static float F_BigmapYaw(Vector3 direction) => TCommon.GetAngle(direction, Vector3.forward, Vector3.up);         //Used For Bigmap Direction
        public static enum_TileDirection E_BigmapDirection(Vector3 direction)  //Top 135-225    Right 45 - 135  Bottom 135 - -135 Right -135 - -45
        {
            float angle = F_BigmapYaw(direction);       //0-360
            if (angle <= 45 && angle > -45)
                return enum_TileDirection.Top;
            if (angle <= 135 && angle > 45)
                return enum_TileDirection.Left;
            if (angle <= -135 || angle > 135)
                return enum_TileDirection.Bottom;
            if (angle <= -45 && angle > -135)
                return enum_TileDirection.Right;
            Debug.LogError("GameSetting.WorldOffsetDirection Error? Invalid angle of:" + angle);
            return enum_TileDirection.Invalid;
        }

        public static float F_SphereCastDamageReduction(float weaponDamage, float distance, float radius) => weaponDamage * (1 - (distance / radius));       //Rocket Blast Damage
        public static Vector3 V3_RangeSpreadDirection(Vector3 aimDirection, float spread, Vector3 up, Vector3 right) => (aimDirection * GameConst.I_ProjectileSpreadAtDistance + up * UnityEngine.Random.Range(-spread, spread) + right * UnityEngine.Random.Range(-spread, spread)).normalized;

        public static int GetEnermyWeaponIndex(int enermyIndex, int weaponIndex = 0, int subWeaponIndex = 0) => enermyIndex * 100 + weaponIndex * 10 + subWeaponIndex;
        public static int GetEnermyWeaponSubIndex(int weaponIndex) => weaponIndex + 1;
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
        public static float F_SporeManagerProfitPerMinute(int level) => 10 * Mathf.Pow(1.3f, level - 1);     //Coins Profit Per Unit Time
        public static float F_SporeManagerCoinsMaxAmount(int level) => 100 * Mathf.Pow(10 * Mathf.Pow(1.3f, level - 1), 1.03f) + 400 * Mathf.Pow(1.4f, level - 1);
        //Coins Max Amount Each Level
        public static float F_SporeManagerChestCoinRequirement(int maxLevel) => 200 * Mathf.Pow(1.4f, maxLevel - 1);       //Coin Requirement Per Chest
        public static float F_SporeManagerChestBlueRequirement(int maxLevel) => 100 * Mathf.Pow(1.05f, maxLevel - 1);       //Blue Requirement Per Chest

        public static Color BigmapTileColor(enum_TileLocking levelLocking, enum_TileType levelType)
        {
            Color color;
            switch (levelType)
            {
                default: color = TCommon.ColorAlpha(Color.blue, .5f); break;
                case enum_TileType.Battle: color = TCommon.ColorAlpha(Color.red, .5f); break;
                case enum_TileType.Reward: color = TCommon.ColorAlpha(Color.green, .5f); break;
                case enum_TileType.Start: color = TCommon.ColorAlpha(Color.blue, .5f); break;
                case enum_TileType.End: color = TCommon.ColorAlpha(Color.black, .5f); break;
            }
            switch (levelLocking)
            {
                case enum_TileLocking.Unlockable: color = TCommon.ColorAlpha(color, .2f); break;
            }
            return color;
        }
    }

    public static class Enum_Relative
    {
        public static enum_LevelGenerateType ToPrefabType(this enum_TileType type)
        {
            switch (type)
            {
                default: Debug.LogError("Please Edit This Please:" + type.ToString()); return enum_LevelGenerateType.Invalid;
                case enum_TileType.Battle:
                case enum_TileType.End:
                    return enum_LevelGenerateType.Big;
                case enum_TileType.Reward:
                case enum_TileType.Start:
                    return enum_LevelGenerateType.Small;
            }
        }
        public static int ToLayer(this enum_HitCheck layerType)
        {
            switch (layerType)
            {
                default: Debug.LogError("Null Layer Can Be Transferd From:" + layerType.ToString()); return 0;
                case enum_HitCheck.Entity: return GameLayer.I_Entity;
                case enum_HitCheck.Static: return GameLayer.I_Static;
                case enum_HitCheck.Dynamic: return GameLayer.I_Dynamic;
            }
        }
    }
    #endregion

    #region For Developers Use

    #region BroadCastEnum
    enum enum_BC_UIStatusChanged
    {
        Invalid = -1,
        PlayerInfoChanged,
        PlayerLevelStatusChanged,
    }
    enum enum_BC_GameStatusChanged
    {
        Invalid = -1,
        OnSpawnEntity,
        OnRecycleEntity,

        OnStageStart,       //Total Stage Start
        OnStageFinish,
        OnLevelStart,       //Change Between Each Level
        OnLevelFinish,
        OnBattleStart,      //Battle Against Entity
        OnBattleFinish,
        OnWaveStart,     //Battle Wave
        OnWaveFinish,
    }
    #endregion

    #region GameEnum
    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, }

    public enum enum_BattleDifficulty { Invalid = -1, Default = 0, Eazy = 1, Normal = 2, Hard = 3 }

    public enum enum_TileLocking { Invalid = -1, Locked = 0, Unlockable = 1, Unlocked = 2, }

    public enum enum_Style { Invalid = -1, Forest = 1, Desert = 2, Iceland = 3, Horde = 4, Undead = 5, }

    public enum enum_TileType { Invalid = -1, Start = 0, Battle = 1, End = 2, Reward = 3, }

    public enum enum_LevelItemType { Invalid = -1, LargeMore, LargeLess, MediumMore, MediumLess, SmallMore, SmallLess, ManmadeMore, ManmadeLess, NoCollisionMore, NoCollisionLess,BorderBlock, }

    public enum enum_LevelTileType { Invaid = -1, Empty, Main,Border, Item, Portal, }

    public enum enum_LevelTileOccupy { Invalid = -1, Inner, Outer, Border, }

    public enum enum_LevelGenerateType { Invalid = -1, Big = 1, Small = 2 }

    public enum enum_EntityType { Invalid = -1, Fighter = 1, Shooter = 2, AOECaster = 3, Elite = 4 }

    public enum enum_Interaction { Invalid = -1, Interact_Portal, }

    public enum enum_TriggerType { Invalid = -1, Single = 1, Auto = 2, Burst = 3, Pull = 4, Store = 5, }

    public enum enum_EnermyWeaponProjectile { Invalid=-1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, }

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, }

    public enum enum_BuffAddType { Invalid=-1, AddUp = 1,Refresh =2}

    public enum enum_HealthMessageType { Invalid = -1, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Projectile = 1, Area = 2, DOT = 3, Fall = 4, ArmorOnly = 5 }

    public enum enum_PlayerWeapon
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

    public enum enum_EnermyAnim
    {
        Invalid = -1,
        Axe_Dual_Pound_10 = 10,
        Spear_R_Stick_20 = 20,
        Sword_R_Swipe_30 = 30,
        Sword_R_Slash_31 = 31,
        Dagger_Dual_Twice_40=40,
        Staff_L_Cast_110 = 110,
        Staff_Dual_Cast_111 = 111,
        Staff_R_Cast_Loop_112 = 112,
        Staff_R_Cast_113=113,
        Bow_Shoot_130 = 130,
        CrossBow_Shoot_131=131,
        Bow_UpShoot_133=133,
        Rifle_HipShot_140 = 140,
        Rifle_AimShot_141=141,
        Throwable_Hips_150 = 150,
        Throwable_R_ThrowHip_151=151,
        Throwable_R_ThrowBack_152=152,
        Heavy_HipShot_161=161,
        Heavy_Mortal_162=162,
        Heavy_Shield_Spear_163=163,
        Heavy_Remote_164=164,
    }
    #endregion

    #region GameLayer
    public static class GameLayer
    {
        public static readonly int I_Static = LayerMask.NameToLayer("static");
        public static readonly int I_Entity = LayerMask.NameToLayer("entity");
        public static readonly int I_Dynamic = LayerMask.NameToLayer("dynamic");
        public static readonly int I_DynamicDetect = LayerMask.NameToLayer("dynamicDetect");
        public static readonly int I_MovementDetect = LayerMask.NameToLayer("movementDetect");
        public static class Physics
        {
            public static readonly int I_All = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity | 1 << GameLayer.I_Dynamic;
            public static readonly int I_StaticEntity = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity;
            public static readonly int I_EntityOnly = (1 << I_Entity);
            public static readonly int I_Static = (1 << GameLayer.I_Static);
        }
    }
    #endregion

    #region GameSave
    public class CPlayerSave : ISave        //Save Outta Game
    {
        public float f_blue;
        public CPlayerSave()
        {
            f_blue = 100;
        }
    }
    #endregion

    #region GameClass/Structs
    #region GameBase
    public class HealthBase
    {
        public float m_CurrentHealth { get; protected set; }
        public float m_MaxHealth { get; private set; }
        public bool b_IsDead => m_CurrentHealth <= 0;

        protected Action<enum_HealthMessageType> OnHealthChanged;
        protected Action OnDead;
        public HealthBase(float _maxHealth, Action<enum_HealthMessageType> _OnHealthChanged, Action _OnDead)
        {
            m_MaxHealth = _maxHealth;
            OnHealthChanged = _OnHealthChanged;
            OnDead = _OnDead;
        }
        public virtual void OnActivate()
        {
            m_CurrentHealth = m_MaxHealth;
        }
        public virtual bool OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1)
        {
            if (b_IsDead)
                return false;

            m_CurrentHealth -= damageInfo.m_AmountApply * damageReduction;
            OnHealthChanged?.Invoke( enum_HealthMessageType.DamageHealth);

            if (b_IsDead)
                OnDead();

            return true;
        }
    }

    public class EntityHealth:HealthBase
    {
        public float m_CurrentArmor { get; protected set; }

        public float m_MaxArmor { get; private set; }
        
        public float m_ArmorRegenSpeed { get; private set; }
        public float m_ArmorRegenDuration { get; private set; }
        public float F_TotalHealth => m_MaxHealth + m_MaxArmor;
        public bool B_HealthFull => m_CurrentHealth >= m_MaxHealth;
        public bool B_ArmorFull => m_CurrentArmor >= m_MaxArmor;
        
        protected float f_ArmorRegenCheck;
        public EntityHealth(SEntity entityInfo, Action<enum_HealthMessageType> _OnHealthChanged, Action _OnDead) :base(entityInfo.m_MaxHealth,_OnHealthChanged,_OnDead)
        {
            m_MaxArmor = entityInfo.m_MaxArmor;
            m_ArmorRegenSpeed = entityInfo.m_ArmorRegenSpeed;
            m_ArmorRegenDuration = entityInfo.m_ArmorRegenDuration;

        }
        public override void OnActivate()
        {
            base.OnActivate();
            m_CurrentArmor = m_MaxArmor;
        }
        public override bool OnReceiveDamage(DamageInfo damageInfo,float damageMultiply=1)
        {
            if (b_IsDead)
                return false;
            
            if (damageInfo.m_AmountApply > 0)    //Damage
            {
                m_CurrentArmor -= damageInfo.m_AmountApply *damageMultiply;
                if (m_CurrentArmor < 0)
                {
                    m_CurrentHealth += m_CurrentArmor;
                    m_CurrentArmor = 0;
                }

                f_ArmorRegenCheck = m_ArmorRegenDuration;

                OnHealthChanged( m_CurrentArmor > 0? enum_HealthMessageType.DamageArmor: enum_HealthMessageType.DamageHealth);
                if (b_IsDead)
                    OnDead();
            }
            else if (damageInfo.m_AmountApply < 0)    //Healing
            {
                if (damageInfo.m_damageType == enum_DamageType.ArmorOnly)       //Heal Armor
                {
                    if (B_ArmorFull)
                        return false;

                    m_CurrentArmor -= damageInfo.m_AmountApply;
                    if (m_CurrentArmor > m_MaxArmor)
                        m_CurrentArmor = m_MaxArmor;

                    OnHealthChanged(enum_HealthMessageType.ReceiveArmor);
                }
                else    //Heal Health Then Armor
                {
                    m_CurrentHealth -= damageInfo.m_AmountApply;
                    if (!B_HealthFull)
                    {
                        OnHealthChanged(enum_HealthMessageType.ReceiveHealth);
                        return true;
                    }

                    float armorReceive = m_CurrentHealth - m_MaxHealth;
                    if (m_CurrentHealth > m_MaxHealth)
                        m_CurrentHealth = m_MaxHealth;

                    if (B_ArmorFull)
                        return false;

                    m_CurrentArmor += armorReceive;
                    if (m_CurrentArmor > m_MaxArmor)
                        m_CurrentArmor = m_MaxArmor;

                    OnHealthChanged(enum_HealthMessageType.ReceiveArmor);
                }
            }

            return true;
        }
        public void Tick(float deltaTime)
        {
            if (m_ArmorRegenSpeed == 0)
                return;

            f_ArmorRegenCheck -= deltaTime;
            if (f_ArmorRegenCheck < 0 && m_CurrentArmor != m_MaxArmor)
                OnReceiveDamage(new DamageInfo(-1 * m_ArmorRegenSpeed * deltaTime, enum_DamageType.ArmorOnly));
        }
    }

    public class DamageInfo
    {
        public float m_AmountApply => m_baseDamage * m_BuffApply.F_DamageEnhanceMultiply;
        public DamageBuffInfo m_BuffApply { get; private set; }
        public enum_DamageType m_damageType { get; private set; }
        private float m_baseDamage;
        public DamageInfo(float damage,enum_DamageType type)
        {
            m_baseDamage = damage;
            m_BuffApply = DamageBuffInfo.Create();
            m_damageType = type;
        }
        public DamageInfo(float damage, enum_DamageType type, DamageBuffInfo buff)
        {
            m_baseDamage = damage;
            m_damageType = type;
            m_BuffApply = buff;
        }
        public void ResetDamage(float damage)
        {
            m_baseDamage = damage;
        }
        public void ResetBuff(DamageBuffInfo buffInfo)
        {
            m_BuffApply = buffInfo;
        }
    }
    #endregion

    #region BuffClass
    public class EntityInfoManager
    {
        public EntityBuffInfo m_EntityBuffProperty { get; private set; }
        public DamageBuffInfo m_DamageBuffProperty { get; private set; }
        public List<BuffBase> m_BuffList { get; private set; } = new List<BuffBase>();
        public SEntity m_InfoData { get; private set; }
        public int I_ObjectPoolIndex => m_InfoData.m_Index;
        public float F_MaxMana => m_InfoData.m_MaxMana;
        public float F_MaxHealth => m_InfoData.m_MaxHealth;
        public float F_MaxArmor => m_InfoData.m_MaxArmor;
        public float F_MovementSpeed => m_InfoData.m_MoveSpeed*m_EntityBuffProperty.F_MovementSpeedMultiply;
        public float F_Spread => m_InfoData.m_Spread;
        public float F_FireRateTick(float deltaTime) => deltaTime* m_EntityBuffProperty.F_FireRateMultiply;
        Func<DamageInfo, bool> OnDamageEntity;
        Action OnInfoChange;
        public EntityInfoManager(SEntity _entityInfo,Func<DamageInfo, bool> _OnDamageEntity,Action _OnInfoChange)
        {
            OnDamageEntity = _OnDamageEntity;
            OnInfoChange = _OnInfoChange;
            m_InfoData = _entityInfo;
            m_EntityBuffProperty = EntityBuffInfo.Create();
            m_DamageBuffProperty = DamageBuffInfo.Create();
        }
        public void Tick(float deltaTime) => m_BuffList.Traversal((BuffBase buff) => { buff.OnTick(deltaTime); });
        public void AddBuff(int buffIndex)
        {
            BuffBase buff = GetBuff(buffIndex);
            switch (buff.m_buffInfo.m_AddType)
            {
                case enum_BuffAddType.AddUp:
                    {
                        m_BuffList.Add(GetBuff(buffIndex));
                    }
                    break;
                case enum_BuffAddType.Refresh:
                    {
                        BuffBase buffRefresh = m_BuffList.Find(p => p.m_buffInfo.m_Index == buffIndex);
                        if (buffRefresh != null)
                            buffRefresh.ExpireRefresh();
                        else
                            m_BuffList.Add(GetBuff(buffIndex));
                    }
                    break;
            }
            OnBuffChanged();
        }
        protected void OnBuffExpired(BuffBase buff)
        {
            m_BuffList.Remove(buff);
            OnBuffChanged();
        }
        public void OnDeactivate()
        {
            m_BuffList.Clear();
            OnBuffChanged();
        }
        public BuffBase GetBuff(int buffIndex) => new BuffBase(DataManager.GetEntityBuffProperties(buffIndex), OnBuffExpired, OnDamageEntity);
        public void OnBuffChanged()
        {
            float entityDamageReduce = 1f;
            float damageApplyEnhance = 1f;
            float movementEnhance = 1f;
            float firerateEnhance = 1f;
            m_BuffList.Traversal((BuffBase buff) => {
                entityDamageReduce -= buff.m_buffInfo.m_DamageReductionPercentage > 0 ? buff.m_buffInfo.m_DamageReductionPercentage / 100f : 0;
                damageApplyEnhance += buff.m_buffInfo.m_DamageMultiplyPercentage > 0 ? buff.m_buffInfo.m_DamageMultiplyPercentage / 100f : 0;
                movementEnhance += buff.m_buffInfo.m_MovementSpeedMultiplyPercentage > 0 ? buff.m_buffInfo.m_MovementSpeedMultiplyPercentage / 100f : 0;
                firerateEnhance += buff.m_buffInfo.m_FireRateMultiplyPercentage > 0 ? buff.m_buffInfo.m_FireRateMultiplyPercentage / 100f : 0;
            });
            entityDamageReduce = entityDamageReduce < 0 ? 0 : entityDamageReduce;
            m_EntityBuffProperty = EntityBuffInfo.Create(entityDamageReduce,movementEnhance,firerateEnhance);
            m_DamageBuffProperty = DamageBuffInfo.Create(damageApplyEnhance, new List<int>());
            OnInfoChange();
        }
    }

    public class BuffBase
    {
        public SBuff m_buffInfo { get; private set; }
        float f_expireCheck, f_dotCheck;
        Action<BuffBase> OnBuffExpired;
        Func<DamageInfo, bool> OnDOTDamage;
        public BuffBase(SBuff _buffInfo, Action<BuffBase> _OnBuffExpired, Func<DamageInfo, bool> _OnDOTDamage)
        {
            m_buffInfo = _buffInfo;
            OnBuffExpired = _OnBuffExpired;
            OnDOTDamage = _OnDOTDamage;
            f_expireCheck = 0;
        }
        public void OnTick(float deltaTime)
        {
            DotCheck(deltaTime);
            ExpireCheck(deltaTime);
        }
        public void DotCheck(float deltaTime)
        {
            if (m_buffInfo.m_DamageTickTime <= 0)
                return;
            f_dotCheck += deltaTime;
            if (f_dotCheck > m_buffInfo.m_DamageTickTime)
            {
                f_dotCheck -= m_buffInfo.m_DamageTickTime;
                OnDOTDamage(new DamageInfo(m_buffInfo.m_DamagePerTick, enum_DamageType.DOT));
            }
        }
        public void ExpireRefresh()
        {
            f_expireCheck = 0;
        }
        public void ExpireCheck(float deltaTime)
        {
            if (m_buffInfo.m_ExpireDuration <= 0)
                return;

            f_expireCheck += deltaTime;
            if (f_expireCheck > m_buffInfo.m_ExpireDuration)
                OnBuffExpired(this);
        }
    }

    public struct EntityBuffInfo
    {
        public float F_DamageReceiveMultiply { get; private set; }
        public float F_MovementSpeedMultiply { get; private set; }
        public float F_FireRateMultiply { get; private set; }
        public static EntityBuffInfo Create()
        {
            EntityBuffInfo buff = new EntityBuffInfo
            {
                F_DamageReceiveMultiply = 1f,
                F_MovementSpeedMultiply = 1f,
                F_FireRateMultiply=1f,
            };
            return buff;
        }
        public static EntityBuffInfo Create(float _damageReduceMultiply,float _movementSpeedMultiply,float _firerateMultiply)
        {
            EntityBuffInfo buff = new EntityBuffInfo
            {
                F_DamageReceiveMultiply = _damageReduceMultiply,
                F_MovementSpeedMultiply = _movementSpeedMultiply,
                F_FireRateMultiply = _firerateMultiply,
            };
            return buff;
        }
    }
    public struct DamageBuffInfo
    {
        public float F_DamageEnhanceMultiply { get; private set; }
        public List<int> m_BuffAplly { get; private set; }
        public static DamageBuffInfo Create(float _damageEnhanceMultiply, List<int> _buffApply)
        {
            DamageBuffInfo buff = new DamageBuffInfo
            {
                F_DamageEnhanceMultiply = _damageEnhanceMultiply,
                m_BuffAplly = _buffApply
            };
            return buff;
        }
        public static DamageBuffInfo Create()
        {
            DamageBuffInfo buff = new DamageBuffInfo
            {
                F_DamageEnhanceMultiply = 1f,
                m_BuffAplly = new List<int>()
            };
            return buff;
        }
    }
    #endregion

    #region Physics
    public static class HitCheckDetect_Extend
    {
        public static HitCheckBase Detect(this Collider other)
        {
            HitCheckBase hitCheck = other.GetComponent<HitCheckBase>();
            if (hitCheck == null)
                Debug.LogWarning("Null Hit Check Attached:" + other.gameObject);

            return hitCheck;
        }
        public static HitCheckEntity DetectEntity(this Collider other)
        {
            HitCheckBase hitCheck = other.GetComponent<HitCheckBase>();
            if (hitCheck == null)
                Debug.LogWarning("Null Hit Check Attached:" + other.gameObject);
            if (hitCheck.m_HitCheckType == enum_HitCheck.Entity)
                return hitCheck as HitCheckEntity;
            return null;
        }
    }
    public class ProjectilePhysicsSimulator : PhysicsSimulatorCapsule
    {
        protected Vector3 m_VerticalDirection;
        float m_horizontalSpeed;
        public ProjectilePhysicsSimulator(Transform _transform, Vector3 _startPos, Vector3 _horizontalDirection, Vector3 _verticalDirection, float _horizontalSpeed, float _height, float _radius, int _hitLayer, Action<RaycastHit[]> _onTargetHit):base(_transform,_startPos, _horizontalDirection,_height,_radius,_hitLayer,_onTargetHit)
        {
            m_VerticalDirection = _verticalDirection.normalized;
            m_horizontalSpeed = _horizontalSpeed;
        }
        public override Vector3 GetSimulatedPosition(float elapsedTime)=> m_startPos + m_Direction * Expressions.SpeedShift(m_horizontalSpeed, elapsedTime); 
    }
    public class ProjectilePhysicsLerpSimulator : PhysicsSimulatorCapsule
    {
        bool b_lerpFinished;
        Action OnLerpFinished;
        Vector3 m_endPos;
        float f_totalTime;
        public ProjectilePhysicsLerpSimulator(Transform _transform, Vector3 _startPos,Vector3 _endPos,Action _OnLerpFinished, float _duration, float _height, float _radius, int _hitLayer, Action<RaycastHit[]> _onTargetHit) : base(_transform, _startPos,_endPos-_startPos , _height, _radius, _hitLayer, _onTargetHit)
        {
            m_endPos = _endPos;
            OnLerpFinished = _OnLerpFinished;
            f_totalTime= _duration;
            b_lerpFinished = false;
        }
        public override void Simulate(float deltaTime)
        {
            base.Simulate(deltaTime);
            if (!b_lerpFinished && m_simulateTime > f_totalTime)
            {
                OnLerpFinished?.Invoke();
                b_lerpFinished = true;
            }
         }
        public override Vector3 GetSimulatedPosition(float elapsedTime) =>b_lerpFinished?m_endPos:Vector3.Lerp(m_startPos, m_endPos, elapsedTime / f_totalTime);
    }
    public class ThrowablePhysicsSimulator : PhysicsSimulatorCapsule
    {
        float f_speed;
        float f_vertiAcceleration;
        float f_bounceHitMaxAnlge;
        bool b_randomRotation;
        bool b_bounceOnHit;
        float f_bounceSpeedMultiply;
        bool B_SpeedOff => f_speed <= 0;
        protected Vector3 v3_RotateEuler;
        protected Vector3 v3_RotateDirection;
        public ThrowablePhysicsSimulator(Transform _transform, Vector3 _startPos, Vector3 _endPos, float _angle, float _horiSpeed, float _height, float _radius, bool randomRotation, int _hitLayer,bool bounce,float _bounceHitAngle,float _bounceSpeedMultiply, Action<RaycastHit[]> _onBounceHit):base(_transform,_startPos,Vector3.zero,_height,_radius,_hitLayer,_onBounceHit)
        {
            Vector3 horiDirection = TCommon.GetXZLookDirection(_startPos, _endPos);
            Vector3 horiRight = horiDirection.RotateDirection(Vector3.up,90);
            m_Direction = horiDirection.RotateDirection(horiRight,-_angle);
            f_speed =  _horiSpeed/Mathf.Cos(_angle*Mathf.Deg2Rad);
            float horiDistance = Vector3.Distance(_startPos, _endPos);
            float duration = horiDistance / _horiSpeed;
            float vertiDistance = Mathf.Tan(_angle * Mathf.Deg2Rad) * horiDistance;
            f_vertiAcceleration = Expressions.GetAcceleration(0, vertiDistance, duration);
            b_randomRotation = randomRotation;
            b_bounceOnHit = bounce;
            f_bounceHitMaxAnlge = _bounceHitAngle;
            f_bounceSpeedMultiply = _bounceSpeedMultiply;
            v3_RotateEuler = Quaternion.LookRotation(m_Direction).eulerAngles;
            v3_RotateDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        }
        public override void Simulate(float deltaTime)
        {
            if (B_SpeedOff)
                return; 

            base.Simulate(deltaTime);

            if (b_randomRotation)
            {
                v3_RotateEuler += v3_RotateDirection * deltaTime * 300f;
                transform.rotation = Quaternion.Euler(v3_RotateEuler);
            }
        }
        public override void OnTargetHitted(RaycastHit[] hitTargets)
        {
            if (hitTargets.Length > 0)
            {
                if (!b_bounceOnHit)
                {
                    base.OnTargetHit(hitTargets);
                    return;
                }

                Vector3 bounceDirection = hitTargets[0].point == Vector3.zero ? Vector3.up : hitTargets[0].normal;
                float bounceAngle = TCommon.GetAngle(bounceDirection, Vector3.up, Vector3.up);
                if (bounceAngle > 15)
                    m_Direction = bounceDirection;
                m_startPos = transform.position;
                m_simulateTime = 0;

                f_speed -= .1f;
                f_speed *= f_bounceSpeedMultiply;
                if (f_speed < 0)
                    f_speed = 0;

                if (f_bounceHitMaxAnlge != 0 && bounceAngle < f_bounceHitMaxAnlge)      //OnBounceHitTarget
                    base.OnTargetHit(hitTargets);
                return;
            }
        }

        public override Vector3 GetSimulatedPosition(float elapsedTime)=>  m_startPos + m_Direction * f_speed * elapsedTime + Vector3.down * f_vertiAcceleration * elapsedTime * elapsedTime;
    }
    #endregion

    #region BigmapTile
    public class SBigmapTileInfo : ITileAxis
    {
        public TileAxis m_TileAxis => m_Tile;
        protected TileAxis m_Tile { get; private set; }
        public enum_TileType m_TileType { get; private set; } = enum_TileType.Invalid;
        public enum_TileLocking m_TileLocking { get; private set; } = enum_TileLocking.Invalid;
        public Dictionary<enum_TileDirection, TileAxis> m_Connections { get; protected set; } = new Dictionary<enum_TileDirection, TileAxis>();

        public SBigmapTileInfo(TileAxis _tileAxis, enum_TileType _tileType, enum_TileLocking _tileLocking)
        {
            m_Tile = _tileAxis;
            m_TileType = _tileType;
            m_TileLocking = _tileLocking;
        }
        public void ResetTileType(enum_TileType _tileType)
        {
            m_TileType = _tileType;
        }
        public void SetTileLocking(enum_TileLocking _lockType)
        {
            if(m_TileLocking!= enum_TileLocking.Unlocked)
               m_TileLocking = _lockType;
        }
    }

    public class SBigmapLevelInfo : SBigmapTileInfo
    {
        protected Transform m_LevelParent;
        public LevelBase m_Level { get; private set; } = null;
        public SBigmapLevelInfo(SBigmapTileInfo tile) : base(tile.m_TileAxis, tile.m_TileType,tile.m_TileLocking)
        {
            m_Connections = tile.m_Connections;
        }
        public Dictionary<LevelItemBase, int> GenerateMap(Transform _levelParent,LevelBase prefab,SLevelGenerate innerData,SLevelGenerate outerData, LevelItemBase[] _levelItemPrefabs,System.Random seed)
        {
            m_LevelParent = _levelParent;
            m_Level =  GameObject.Instantiate(prefab, _levelParent);
            m_Level.transform.localRotation = Quaternion.Euler(0, seed.Next(360), 0);
            m_Level.transform.localPosition = Vector3.zero;
            m_Level.transform.localScale = Vector3.one;
            m_Level.SetActivate(false);
            return m_Level.Init( innerData,outerData, _levelItemPrefabs, m_TileType, seed, m_Connections.Keys.ToList().Find(p => m_Connections[p] == new TileAxis(-1, -1)));        //Add Portal For Level End
        }
        public void StartLevel()
        {
            m_Level.ShowAllItems();
            m_Level.SetActivate(true);
        }
    }
    #endregion

    #region LevelTile
    public class LevelTile 
    {
        public TileAxis m_TileAxis;
        public Vector3 m_Offset => GameExpression.V3_TileAxisOffset(m_TileAxis);
        public virtual enum_LevelTileType E_TileType => enum_LevelTileType.Empty;
        public enum_TileDirection E_WorldDireciton { get; private set; } = enum_TileDirection.Invalid;
        public enum_LevelTileOccupy E_Occupation { get; private set; } = enum_LevelTileOccupy.Invalid;
        public LevelTile(TileAxis _axis ,enum_TileDirection _direction,enum_LevelTileOccupy _occupy) 
        {
            E_WorldDireciton = _direction;
            m_TileAxis = _axis;
            E_Occupation = _occupy;
        }
        public LevelTile(LevelTile tile)
        {
            m_TileAxis = tile.m_TileAxis;
            E_WorldDireciton = tile.E_WorldDireciton;
            E_Occupation = tile.E_Occupation;
        }
    }
    public class LevelTilePortal : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Portal;
        public enum_TileDirection E_PortalDirection { get; private set; }
        public Vector3 m_worldPos;
        public LevelTilePortal(LevelTile current, enum_TileDirection _direction,Vector3 _worldPos) : base(current)
        {
            E_PortalDirection = _direction;
            m_worldPos = _worldPos;
        }
    }
    class LevelTileItemSub : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Item;
        public int m_ParentMainIndex { get; private set; }
        public LevelTileItemSub(LevelTile current, int _parentMainIndex) : base(current)
        {
            m_ParentMainIndex = _parentMainIndex;
        }
    }
    class LevelTileItemMain : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Main;
        public int m_LevelItemListIndex { get; private set; }
        public enum_LevelItemType m_LevelItemType { get; private set; }
        public enum_TileDirection m_ItemDirection { get; private set; }
        public List<int> m_AreaTiles { get; private set; }
        
        public LevelTileItemMain(LevelTile current, int levelItemListIndex, enum_LevelItemType levelItemType,enum_TileDirection _ItemDirection, List<int> _AreaTiles) : base(current)
        {
            m_LevelItemListIndex = levelItemListIndex;
            m_LevelItemType = levelItemType;
            m_AreaTiles = _AreaTiles;
            m_ItemDirection = _ItemDirection;
        }
    }
    class LevelTileBorder : LevelTileItemMain
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Border;
        public LevelTileBorder(LevelTile current, int levelItemListIndex, enum_LevelItemType levelItemType, enum_TileDirection _ItemDirection) : base(current,levelItemListIndex,levelItemType,_ItemDirection,null)
        {
        }
    }
    #endregion

    #region ExcelStruct
    public struct SEntity :ISExcel
    {
        int i_index;
        int e_type;
        float f_maxHealth;
        float f_maxArmor;
        float f_maxMana;
        float f_armorRegenSpeed;
        float f_armorRegenDuration;
        float f_moveSpeed;
        float f_chaseRange;
        float f_attackRange;    
        bool b_battleObstacleCheck;
        RangeFloat fr_duration;
        float f_firerate;
        RangeInt ir_count;
        int i_spread;
        RangeInt ir_rangeExtension;
        float f_offsetExtension;
        
        public int m_Index=>i_index;
        public enum_EntityType m_Type => (enum_EntityType)e_type;
        public float m_MaxHealth => f_maxHealth;
        public float m_MaxArmor => f_maxArmor;
        public float m_MaxMana => f_maxMana;
        public float m_ArmorRegenSpeed => f_armorRegenSpeed;
        public float m_ArmorRegenDuration => f_armorRegenDuration;
        public float m_MoveSpeed => f_moveSpeed;
        public float m_AIChaseRange => f_chaseRange;
        public float m_AIAttackRange => f_attackRange;
        public bool m_BattleCheckObsatacle => b_battleObstacleCheck;
        public RangeFloat m_BarrageDuration => fr_duration;
        internal float m_Firerate => f_firerate;
        public RangeInt m_ProjectileCount => ir_count;
        public int m_Spread => i_spread>0? i_spread:0;
        public RangeInt m_RangeExtension => ir_rangeExtension;
        public float m_OffsetExtension => f_offsetExtension;
        public void InitOnValueSet()
        {
        }
    }
    public struct SWeapon : ISExcel
    {
        int index;
        string s_name;
        int i_triggerType;
        int i_muzzleIndex;
        int i_projectileIndex;
        float f_fireRate;
        float f_specialRate;
        float f_manaCost;
        int i_clipAmount;
        float f_spread;
        float f_reloadTime;
        int i_PelletsPerShot;
        float f_stunAfterShot;
        float f_recoilHorizontal;
        float f_recoilVertical;
        public enum_PlayerWeapon m_Weapon => (enum_PlayerWeapon)index;
        public string m_Name => s_name;
        public int m_ProjectileSFX => i_projectileIndex;
        public int m_MuzzleSFX => i_muzzleIndex;
        public enum_TriggerType m_TriggerType=>(enum_TriggerType)i_triggerType;
        public float m_FireRate => f_fireRate;
        public float m_SpecialRate => f_specialRate;
        public float m_ManaCost => f_manaCost;
        public int m_ClipAmount => i_clipAmount;
        public float m_Spread => f_spread;
        public float m_ReloadTime => f_reloadTime;
        public int m_PelletsPerShot => i_PelletsPerShot;
        public float m_stunAfterShot => f_stunAfterShot;
        public Vector2 m_RecoilPerShot => new Vector2(f_recoilHorizontal, f_recoilVertical);
        public void InitOnValueSet()
        {
        }
    }
    public struct SLevelGenerate : ISExcel
    {
        string em_defines;
        RangeInt ir_length;
        RangeInt ir_smallLess;
        RangeInt ir_smallMore;
        RangeInt ir_mediumLess;
        RangeInt ir_mediumMore;
        RangeInt ir_largeLess;
        RangeInt ir_largeMore;
        RangeInt ir_manmadeLess;
        RangeInt ir_manmadeMore;
        RangeInt ir_noCollisionLess;
        RangeInt ir_noCollisionMore;
        public bool m_IsInner;
        public enum_Style m_LevelStyle;
        public enum_LevelGenerateType m_LevelPrefabType;
        public Dictionary<enum_LevelItemType, RangeInt> m_ItemGenerate;
        public RangeInt m_Length => ir_length;
        public void InitOnValueSet()
        {
            string[] defineSplit = em_defines.Split('_');
            if (defineSplit.Length != 3)
                Debug.LogError("Please Corret Format Of DefineSplit:" +em_defines   );
            m_LevelStyle = (enum_Style)(int.Parse(defineSplit[0]));
            m_LevelPrefabType = (enum_LevelGenerateType)(int.Parse(defineSplit[1]));
            m_IsInner = int.Parse(defineSplit[2]) == 1;
            m_ItemGenerate = new Dictionary<enum_LevelItemType, RangeInt>(); 
            m_ItemGenerate.Add(enum_LevelItemType.LargeLess, ir_largeLess);
            m_ItemGenerate.Add(enum_LevelItemType.LargeMore, ir_largeMore);
            m_ItemGenerate.Add(enum_LevelItemType.MediumLess, ir_mediumLess);
            m_ItemGenerate.Add(enum_LevelItemType.MediumMore, ir_mediumMore);
            m_ItemGenerate.Add(enum_LevelItemType.SmallLess,ir_smallLess);
            m_ItemGenerate.Add(enum_LevelItemType.SmallMore, ir_smallMore);
            m_ItemGenerate.Add(enum_LevelItemType.ManmadeLess, ir_manmadeLess);
            m_ItemGenerate.Add(enum_LevelItemType.ManmadeMore, ir_manmadeMore);
            m_ItemGenerate.Add(enum_LevelItemType.NoCollisionLess, ir_noCollisionLess);
            m_ItemGenerate.Add(enum_LevelItemType.NoCollisionMore, ir_noCollisionMore);
        }
    }
    public struct SGenerateEntity:ISExcel
    {
        string em_defines;
        int i_waveCount;
        float f_eliteChance;
        RangeInt ir_fighter;
        RangeInt ir_shooter;
        RangeInt ir_aoeCaster;
        RangeInt ir_elite;
        public int m_stageIndex;
        public enum_TileType m_TileType;
        public enum_BattleDifficulty m_Difficulty;
        public int m_WaveCount => i_waveCount;
        public float m_EliteChance => f_eliteChance;
        public Dictionary<enum_EntityType, RangeInt> m_EntityGenerate;
        public void InitOnValueSet()
        {
            string[] defineSplit = em_defines.Split('_');
            m_stageIndex = int.Parse(defineSplit[0]);
            m_TileType = (enum_TileType)(int.Parse(defineSplit[1]));
            m_Difficulty = (enum_BattleDifficulty)(int.Parse(defineSplit[2]));
            m_EntityGenerate = new Dictionary<enum_EntityType, RangeInt>();
            m_EntityGenerate.Add( enum_EntityType.Fighter,ir_fighter);
            m_EntityGenerate.Add(enum_EntityType.Shooter, ir_shooter);
            m_EntityGenerate.Add(enum_EntityType.AOECaster, ir_aoeCaster);
            m_EntityGenerate.Add(enum_EntityType.Elite, ir_elite);

        }
    }
    public struct SBuff : ISExcel
    {
        int index;
        int i_addType;
        float f_expireDuration;
        int i_effect;
        float f_movementSpeedMultiply;
        float f_fireRateMultiply;
        float f_damageMultiply;
        float f_damageReduce;
        float f_damageTickTime;
        float f_damagePerTick;
        public int m_Index => index;
        public enum_BuffAddType m_AddType => (enum_BuffAddType)i_addType;
        public float m_ExpireDuration => f_expireDuration;
        public int m_EffectIndex => i_effect;
        public float m_MovementSpeedMultiplyPercentage => f_movementSpeedMultiply > 0 ? f_movementSpeedMultiply : 0;
        public float m_FireRateMultiplyPercentage => f_fireRateMultiply > 0 ? f_fireRateMultiply : 0;
        public float m_DamageMultiplyPercentage => f_damageMultiply > 0 ? f_damageMultiply : 0;
        public float m_DamageReductionPercentage => f_damageReduce > 0 ? f_damageReduce : 0;
        public float m_DamageTickTime => f_damageTickTime;
        public float m_DamagePerTick => f_damagePerTick;
        public void InitOnValueSet()
        {
        }
    }
    #endregion
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
            i_previousTimeStamp = TTime.TTime.GetTimeStamp(DateTime.Now);
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
        public void InitOnValueSet()
        {

        }
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
