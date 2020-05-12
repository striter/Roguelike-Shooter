using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGameSave;
using TExcel;
using System;
using System.Linq;

namespace GameSetting
{
    public static class OptionsDataManager
    {
        public static event Action event_OptionChanged;
        public static CGameOptions m_OptionsData => TGameData<CGameOptions>.Data;
        public static void Init()
        {
            TGameData<CGameOptions>.Init();
            OnOptionChanged();
        }

        public static void Save()
        {
            TGameData<CGameOptions>.Save();
            OnOptionChanged();
        }

        public static void OnOptionChanged() => event_OptionChanged?.Invoke();
        public static float F_SFXVolume => GameExpression.F_GameVFXVolume(m_OptionsData.m_VFXVolumeTap);
        public static float F_MusicVolume => GameExpression.F_GameMusicVolume(m_OptionsData.m_MusicVolumeTap);
    }

    public static class GameDataManager
    {
        public static CGameSave m_GameData => TGameData<CGameSave>.Data;
        public static CArmoryData m_ArmoryData => TGameData<CArmoryData>.Data;
        public static CCharacterUpgradeData m_CharacterData => TGameData<CCharacterUpgradeData>.Data;
        public static CGameProgressSave m_GameProgressData => TGameData<CGameProgressSave>.Data;

        public static bool m_Inited { get; private set; } = false;
        public static void Init()
        {
            if (m_Inited) return;
            m_Inited = true;
            Properties<SWeapon>.Init();
            Properties<SBuff>.Init();
            SheetProperties<SEnermyGenerate>.Init();

            InitPlayerPerks();
            InitEnermyPerks();
            InitArmory();

            TGameData<CGameSave>.Init();
            TGameData<CCharacterUpgradeData>.Init();
            TGameData<CArmoryData>.Init();
            TGameData<CGameProgressSave>.Init();

            InitArmoryGameWeaponUnlocked();
        }
        #region GameSave
        public static void OnNewGame()
        {
            TGameData<CGameProgressSave>.Reset();
            TGameData<CGameProgressSave>.Save();
        }

        public static void StageFinishSaveData(EntityCharacterPlayer data, GameProgressManager level)
        {
            m_GameProgressData.Adjust(data, level);
            TGameData<CGameProgressSave>.Save();
        }

        public static void OnGameResult(GameProgressManager progress)
        {
            if (progress.m_GameWin)
                m_GameData.UnlockDifficulty();
            OnCreditStatus(progress.F_CreditGain);

            TGameData<CGameProgressSave>.Reset();
            TGameData<CGameProgressSave>.Save();
        }
        #endregion

        #region GameData
        public static bool CanUseCredit(float credit) => m_GameData.f_Credits >= credit;
        public static void OnCreditStatus(float credit)
        {
            if (credit == 0)
                return;
            m_GameData.f_Credits += credit;
            TGameData<CGameSave>.Save();
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampCurrencyStatus);
        }

        public static enum_GameDifficulty OnCampDifficultySwitch()
        {
            m_GameData.m_GameDifficulty ++;
            if (m_GameData.m_GameDifficulty > m_GameData.m_DifficultyUnlocked)
                m_GameData.m_GameDifficulty =  enum_GameDifficulty.Normal;

            TGameData<CGameSave>.Save();
            return m_GameData.m_GameDifficulty;
        }

        public static bool OnDailyRewardRequire()
        {
            if (!m_GameData.CheckDailyReward())
                return false;
            OnCreditStatus(500f);
            return true;
        }
        #endregion

        #region ArmoryData
        public static Dictionary<enum_PlayerWeapon, SWeapon> m_AvailableWeapons { get; private set; } = new Dictionary<enum_PlayerWeapon, SWeapon>();
        public static Dictionary<enum_Rarity, List<enum_PlayerWeapon>> m_GameWeaponUnlocked { get; private set; } = new Dictionary<enum_Rarity, List<enum_PlayerWeapon>>();

        static void InitArmory()
        {
            m_AvailableWeapons.Clear();

            Properties<SWeapon>.PropertiesList.Traversal((SWeapon weapon) =>
            {
                if (weapon.m_Hidden)
                    return;
                m_AvailableWeapons.Add(weapon.m_Weapon, weapon);
            });
        }

        static void InitArmoryGameWeaponUnlocked()
        {
            m_GameWeaponUnlocked.Clear();
            m_AvailableWeapons.Traversal((SWeapon weapon) =>
            {
                if (!m_ArmoryData.m_WeaponsUnlocked.Contains(weapon.m_Weapon))
                    return;
                if (!m_GameWeaponUnlocked.ContainsKey(weapon.m_Rarity))
                    m_GameWeaponUnlocked.Add(weapon.m_Rarity, new List<enum_PlayerWeapon>());
                m_GameWeaponUnlocked[weapon.m_Rarity].Add(weapon.m_Weapon);
            });
        }

        public static WeaponSaveData RandomUnlockedWeaponData(enum_Rarity rarity,enum_GameStage stage, System.Random random = null)
        {
            if (!m_GameWeaponUnlocked.ContainsKey(rarity))
                rarity = enum_Rarity.Ordinary;
            return WeaponSaveData.New(m_GameWeaponUnlocked[rarity].RandomItem(random), GameConst.m_StageWeaponEnhanceLevel[stage].RandomPercentage(0,random));
        }

        public static float GetArmoryUnlockPrice(enum_PlayerWeapon weapon) => GameConst.m_ArmoryBlueprintUnlockPrice[m_AvailableWeapons[weapon].m_Rarity];
        public static bool CanArmoryUnlock(enum_PlayerWeapon weapon) => m_GameData.f_Credits >= GetArmoryUnlockPrice(weapon);
        public static void OnArmoryUnlock(enum_PlayerWeapon weapon)
        {
            if (!m_ArmoryData.m_WeaponBlueprints.Contains(weapon))
            {
                Debug.LogError("Error! Unlock A None Blueprint Weapon" + weapon);
                return;
            }
            if (m_ArmoryData.m_WeaponsUnlocked.Contains(weapon))
            {
                Debug.LogError("Error! Unlock A Unlocked Weapon" + weapon);
                return;
            }
            if (!CanArmoryUnlock(weapon))
            {
                Debug.LogError("Invalid Unlock Behaviour!");
                return;
            }
            OnCreditStatus(-GetArmoryUnlockPrice(weapon));
            m_ArmoryData.m_WeaponBlueprints.Remove(weapon);
            m_ArmoryData.m_WeaponsUnlocked.Add(weapon);
            m_ArmoryData.m_WeaponSelected = weapon;
            TGameData<CArmoryData>.Save();
            InitArmoryGameWeaponUnlocked();
        }

        public static void OnArmorySelect(enum_PlayerWeapon weapon)
        {
            if (!m_ArmoryData.m_WeaponsUnlocked.Contains(weapon))
            {
                Debug.LogError("Error! Equipping A Locked Weapon!");
                return;
            }
            if (weapon == m_ArmoryData.m_WeaponSelected)
            {
                Debug.LogError("Error! Equipping A Selcted Weapon!");
                return;
            }
            m_ArmoryData.m_WeaponSelected = weapon;
            TGameData<CArmoryData>.Save();
        }

        public static enum_PlayerWeapon UnlockArmoryBlueprint(enum_Rarity _spawnRarity)
        {
            Dictionary<enum_Rarity, List<enum_PlayerWeapon>> _blueprintAvailable = new Dictionary<enum_Rarity, List<enum_PlayerWeapon>>();
            m_AvailableWeapons.Traversal((enum_PlayerWeapon weapon, SWeapon weaponData) =>
            {
                if (m_ArmoryData.m_WeaponBlueprints.Contains(weapon) || m_ArmoryData.m_WeaponsUnlocked.Contains(weapon))
                    return;
                if (!_blueprintAvailable.ContainsKey(weaponData.m_Rarity))
                    _blueprintAvailable.Add(weaponData.m_Rarity, new List<enum_PlayerWeapon>());
                _blueprintAvailable[weaponData.m_Rarity].Add(weapon);
            });

            enum_PlayerWeapon bluePrint = enum_PlayerWeapon.Invalid;
            if (_blueprintAvailable.ContainsKey(_spawnRarity))
                bluePrint = _blueprintAvailable[_spawnRarity].RandomItem();
            else if (_blueprintAvailable.ContainsKey(enum_Rarity.Ordinary))
                bluePrint = _blueprintAvailable[enum_Rarity.Ordinary].RandomItem();

            if (bluePrint != enum_PlayerWeapon.Invalid)
            {
                m_ArmoryData.m_WeaponBlueprints.Add(bluePrint);
                TGameData<CArmoryData>.Save();
                InitArmoryGameWeaponUnlocked();
            }

            return bluePrint;
        }

        public static bool CanUseArmoryParts(int count) => m_ArmoryData.m_WeaponParts >= count;
        public static void OnArmoryPartsStatus(int count)
        {
            m_ArmoryData.m_WeaponParts += count;
            TGameData<CArmoryData>.Save();
        }
        #endregion

        #region CharacterData
        public static bool CanChangeCharacter(enum_PlayerCharacter character) => m_CharacterData.m_CharacterSelected != character;
        public static void SwitchCharacter(enum_PlayerCharacter character)
        {
            if (!CanChangeCharacter(character))
            {
                Debug.LogError("Can't Change Character!");
                return;
            }

            m_CharacterData.ChangeSelectedCharacter(character);
            TGameData<CCharacterUpgradeData>.Save();
        }
        #endregion

        #region Player Perk Data
        static Dictionary<int, ExpirePlayerPerkBase> m_AllPlayerPerks = new Dictionary<int, ExpirePlayerPerkBase>();
        static Dictionary<enum_Rarity, List<int>> m_PlayerPerkRarities = new Dictionary<enum_Rarity, List<int>>();
        public static void InitPlayerPerks()
        {
            m_AllPlayerPerks.Clear();
            m_PlayerPerkRarities.Clear();
            TReflection.TraversalAllInheritedClasses(((Type type, ExpirePlayerPerkBase perk) => {
                m_AllPlayerPerks.Add(perk.m_Index, perk);
                if (perk.m_DataHidden)
                    return;
                if (!m_PlayerPerkRarities.ContainsKey(perk.m_Rarity))
                    m_PlayerPerkRarities.Add(perk.m_Rarity, new List<int>());
                m_PlayerPerkRarities[perk.m_Rarity].Add(perk.m_Index);
            }), PerkSaveData.New(-1));
        }
        public static int RandomPlayerPerk(enum_Rarity rarity, Dictionary<int, ExpirePlayerPerkBase> playerPerks, System.Random random = null)
        {
            List<int> rarityIDs = m_PlayerPerkRarities[rarity].DeepCopy();
            playerPerks.Traversal((ExpirePlayerPerkBase perk) => { if (perk.m_Rarity == rarity && perk.m_Stack == perk.m_MaxStack) rarityIDs.Remove(perk.m_Index); });
            return rarityIDs.RandomItem(random);
        }

        public static List<int> RandomPlayerPerks(int perkCount, Dictionary<enum_Rarity, int> perkGenerate, Dictionary<int, ExpirePlayerPerkBase> playerPerks, System.Random random = null)
        {
            Dictionary<enum_Rarity, List<int>> _perkIDs = m_PlayerPerkRarities.DeepCopy();
            Dictionary<enum_Rarity, int> _rarities = perkGenerate.DeepCopy();

            playerPerks.Traversal((ExpirePlayerPerkBase perk) => { if (perk.m_Stack == perk.m_MaxStack) _perkIDs[perk.m_Rarity].Remove(perk.m_Index); });

            List<int> randomIDs = new List<int>();
            for (int i = 0; i < perkCount; i++)
            {
                enum_Rarity rarity = TCommon.RandomPercentage(_rarities, random);
                if (_perkIDs[rarity].Count == 0)
                    rarity = enum_Rarity.Ordinary;

                int perkID = _perkIDs[rarity].RandomItem(random);
                _perkIDs[rarity].Remove(perkID);
                randomIDs.Add(perkID);
            }
            return randomIDs;
        }

        public static ExpirePlayerPerkBase GetPlayerPerkData(int index) => m_AllPlayerPerks[index];

        public static ExpirePlayerPerkBase CreatePlayerPerk(PerkSaveData data)
        {
            if (!m_AllPlayerPerks.ContainsKey(data.m_Index))
                Debug.LogError("Error Perk:" + data.m_Index + " ,Does not exist");
            ExpirePlayerPerkBase equipment = TReflection.CreateInstance<ExpirePlayerPerkBase>(m_AllPlayerPerks[data.m_Index].GetType(), data);
            return equipment;
        }
        #endregion

        #region Enermy Perk Data
       public const int m_DefaultEnermyPerkIdentity= 20000;
        static Dictionary<int, ExpireEnermyPerkBase> m_AllEnermyPerks = new Dictionary<int, ExpireEnermyPerkBase>();
        static List<int> m_AvailableEnermyPerks = new List<int>();
        public static void InitEnermyPerks()
        {
            m_AllEnermyPerks.Clear();
            m_AvailableEnermyPerks.Clear();
            TReflection.TraversalAllInheritedClasses(((Type type, ExpireEnermyPerkBase perk) => {
                m_AllEnermyPerks.Add(perk.m_Index, perk);
                if (perk.m_Index == m_DefaultEnermyPerkIdentity)
                    return;
                m_AvailableEnermyPerks.Add(perk.m_Index);
            }),0,0);
        }

        public static ExpireEnermyPerkBase RandomEnermyPerk(int minutesPassed,enum_GameDifficulty difficulty,bool isElite)=>TReflection.CreateInstance<ExpireEnermyPerkBase>(m_AllEnermyPerks[isElite ? m_AvailableEnermyPerks.RandomItem() : m_DefaultEnermyPerkIdentity].GetType(), GameExpression.GetEnermyMaxHealthMultiplier(minutesPassed, difficulty),GameExpression.GetEnermyDamageMultilier(minutesPassed,difficulty));
        public static bool IsElitePerk(this ExpireEnermyPerkBase perk) => perk.m_Index != m_DefaultEnermyPerkIdentity;
        #endregion

        #region ExcelData
        public static enum_PlayerWeapon TryGetWeaponEnum(string weaponIdentity)
        {
            int idTry = -1;
            if (int.TryParse(weaponIdentity, out idTry) && Enum.IsDefined(typeof(enum_PlayerWeapon), idTry))
                return (enum_PlayerWeapon)idTry;

            enum_PlayerWeapon targetWeapon = enum_PlayerWeapon.Invalid;
            if (Enum.TryParse(weaponIdentity, out targetWeapon))
                return targetWeapon;

            if (Properties<SWeapon>.PropertiesList.Any(p => TLocalization.GetKeyLocalized(p.m_Weapon.GetNameLocalizeKey()) == weaponIdentity))
                return Properties<SWeapon>.PropertiesList.Find(p => TLocalization.GetKeyLocalized(p.m_Weapon.GetNameLocalizeKey()) == weaponIdentity).m_Weapon;

            Debug.LogError("Invalid Player Weapon Found!");
            return enum_PlayerWeapon.Invalid;
        }

        public static SWeapon GetWeaponProperties(enum_PlayerWeapon type)
        {
            SWeapon weapon = Properties<SWeapon>.PropertiesList.Find(p => p.m_Weapon == type);
            if (weapon.m_Weapon == 0)
                Debug.LogError("Error Properties Found Of Index:" + type.ToString() + "|" + ((int)type));
            else if (weapon.m_Hidden)
                Debug.LogWarning("You've Spawned A Hidden Weapon!");
            return weapon;
        }
        public static SBuff GetPresetBuff(int index)
        {
            SBuff buff = Properties<SBuff>.PropertiesList.Find(p => p.m_Index == index);
            if (buff.m_Index == 0)
                Debug.LogError("Error Properties Found Of Index:" + index);
            return buff;
        }

        public static List<SEnermyGenerate> GetEnermyGenerate(enum_GameStage stage,enum_GameDifficulty difficulty)=> SheetProperties<SEnermyGenerate>.GetPropertiesList(((int)difficulty-1)*5+((int)stage-1));
        #endregion
    }
    #region Structs
    #region SaveData
    public class CGameSave : ISave
    {
        public float f_Credits;
        public enum_GameDifficulty m_GameDifficulty;
        public enum_GameDifficulty m_DifficultyUnlocked;
        public int m_LastDailyRewardStamp;
        public CGameSave()
        {
            f_Credits = 100;
            m_GameDifficulty =  enum_GameDifficulty.Normal;
            m_DifficultyUnlocked =  enum_GameDifficulty.Normal;
            m_LastDailyRewardStamp = -1;
        }

        public void UnlockDifficulty()
        {
            if (m_GameDifficulty != m_DifficultyUnlocked)
                return;

            if (m_DifficultyUnlocked == enum_GameDifficulty.Hard)
                return;

            m_DifficultyUnlocked++;
            m_GameDifficulty++;
        }

        public bool CheckDailyReward()
        {
            int currentDayStamp = TTime.TTimeTools.GetDayStampNow();
            if (currentDayStamp <= m_LastDailyRewardStamp)
                return false;
            m_LastDailyRewardStamp = currentDayStamp;
            return true;
        }

        public void DataRecorrect()
        {
        }
    }

    public class CGameProgressSave : ISave
    {
        public string m_GameSeed;
        public enum_GameDifficulty m_GameDifficulty;

        public enum_GameStage m_Stage;
        public float m_TimeElapsed;
        public float m_Health;
        public int m_Keys;
        public int m_TotalExp;
        public enum_PlayerCharacter m_Character;
        public WeaponSaveData m_Weapon1;
        public WeaponSaveData m_Weapon2;
        public List<PerkSaveData> m_Perks;

        public List<enum_PlayerWeapon> m_ArmoryBlueprintsUnlocked;
        public int m_ArmoryPartsAcquired;

        public CGameProgressSave() : this(GameDataManager.m_GameData.m_GameDifficulty,GameDataManager.m_CharacterData.m_CharacterSelected, GameDataManager.m_ArmoryData.m_WeaponSelected)
        {
        }

        public CGameProgressSave(enum_GameDifficulty difficulty, enum_PlayerCharacter character, enum_PlayerWeapon weapon)
        {
            m_GameSeed = DateTime.Now.ToLongTimeString();
            m_GameDifficulty = difficulty;

            m_TimeElapsed = 0;
            m_Keys = 0;
            m_TotalExp = 0;
            m_Health = -1;
            m_Character = character;
            m_Perks = new List<PerkSaveData>();
            m_Weapon1 = WeaponSaveData.New(weapon);
            m_Weapon2 = WeaponSaveData.New(enum_PlayerWeapon.Invalid);
            m_Stage = enum_GameStage.Rookie;
            m_ArmoryBlueprintsUnlocked = new List<enum_PlayerWeapon>();
            m_ArmoryPartsAcquired = 0;
        }

        public void Adjust(EntityCharacterPlayer _player, GameProgressManager _level)
        {
            m_TimeElapsed = _level.m_TimeElapsed;
            m_Stage = _level.m_Stage;
            m_Keys = _player.m_CharacterInfo.m_Keys;
            m_TotalExp = _player.m_CharacterInfo.m_RankManager.m_TotalExp;
            m_Health = _player.m_Health.m_CurrentHealth;
            m_Weapon1 = WeaponSaveData.Save(_player.m_Weapon1);
            m_Weapon2 = WeaponSaveData.Save(_player.m_Weapon2);
            m_Perks = PerkSaveData.Create(_player.m_CharacterInfo.m_ExpirePerks.Values.ToList());
            m_ArmoryBlueprintsUnlocked = _level.m_ArmoryBlueprintsUnlocked;
            m_ArmoryPartsAcquired = _level.m_ArmoryPartsAcquired;
        }

        void ISave.DataRecorrect()
        {
        }
    }

    public class CArmoryData : ISave
    {
        public List<enum_PlayerWeapon> m_WeaponsUnlocked;
        public List<enum_PlayerWeapon> m_WeaponBlueprints;
        public enum_PlayerWeapon m_WeaponSelected;
        public int m_WeaponParts;
        public CArmoryData()
        {
            m_WeaponsUnlocked = new List<enum_PlayerWeapon>() { enum_PlayerWeapon.P92, enum_PlayerWeapon.UMP45, enum_PlayerWeapon.Kar98, enum_PlayerWeapon.AKM, enum_PlayerWeapon.S686, enum_PlayerWeapon.Minigun, enum_PlayerWeapon.RocketLauncher, enum_PlayerWeapon.FrostWand };
            m_WeaponBlueprints = new List<enum_PlayerWeapon>() { enum_PlayerWeapon.HeavySword, enum_PlayerWeapon.Flamer };
            m_WeaponSelected = enum_PlayerWeapon.P92;
            m_WeaponParts = 5;
        }

        public void DataRecorrect()
        {
        }
    }

    public class CCharacterUpgradeData:ISave
    {
        public enum_PlayerCharacter m_CharacterSelected;

        public CCharacterUpgradeData()
        {
            m_CharacterSelected = enum_PlayerCharacter.Railer;
        }

        public void ChangeSelectedCharacter(enum_PlayerCharacter character) => m_CharacterSelected = character;

        public void DataRecorrect()
        {
        }
    }

    public class CGameOptions : ISave
    {
        public enum_Option_JoyStickMode m_JoyStickMode;
        public enum_Option_FrameRate m_FrameRate;
        public enum_Option_Effect m_Effect;
        public enum_Option_LanguageRegion m_Region;
        public bool m_ShadowOff;
        public int m_MusicVolumeTap;
        public int m_VFXVolumeTap;
        public int m_SensitiveTap;

        public CGameOptions()
        {
            m_JoyStickMode = enum_Option_JoyStickMode.Retarget;
            m_FrameRate = enum_Option_FrameRate.High;
            m_Effect = enum_Option_Effect.High;
            m_Region = enum_Option_LanguageRegion.CN;
            m_ShadowOff = false;
            m_SensitiveTap = 5;
            m_MusicVolumeTap = 10;
            m_VFXVolumeTap = 10;
        }

        void ISave.DataRecorrect()
        {
        }
    }

    public struct WeaponSaveData : IDataConvert
    {
        public enum_PlayerWeapon m_Weapon { get; private set; }
        public int m_Enhance { get; private set; }
        public static WeaponSaveData Save(WeaponBase weapon) => new WeaponSaveData() { m_Weapon = weapon != null ? weapon.m_WeaponInfo.m_Weapon : enum_PlayerWeapon.Invalid,m_Enhance=weapon!=null?weapon.m_EnhanceLevel:0 };
        public static WeaponSaveData New(enum_PlayerWeapon weapon,int enhanceLevel=0) => new WeaponSaveData() { m_Weapon = weapon,m_Enhance=enhanceLevel };
    }

    public struct MercenarySaveData : IDataConvert
    {
        public enum_MercenaryCharacter m_MercenaryCharacter { get; private set; }
        public WeaponSaveData m_Weapon { get; private set; }
        public float m_Health { get; private set; }
        public MercenarySaveData(EntityCharacterMercenary _mercenary)
        {
            m_Weapon = WeaponSaveData.Save(_mercenary.m_Weapon);
            m_MercenaryCharacter = _mercenary.m_Character;
            m_Health = _mercenary.m_Health.m_CurrentHealth;

        }
    }

    public struct PerkSaveData : IDataConvert
    {
        public int m_Index { get; private set; }
        public int m_PerkStack { get; private set; }
        public float m_RecordData { get; private set; }
        public static PerkSaveData New(int index) => new PerkSaveData() { m_Index = index, m_PerkStack = 1, m_RecordData = -1 };
        public static PerkSaveData Create(ExpirePlayerPerkBase perk) => new PerkSaveData() { m_Index = perk.m_Index, m_PerkStack = perk.m_Stack, m_RecordData = perk.m_RecordData };
        public static List<PerkSaveData> Create(List<ExpirePlayerPerkBase> perks)
        {
            List<PerkSaveData> data = new List<PerkSaveData>();
            perks.Traversal((ExpirePlayerPerkBase perk) => { data.Add(Create(perk)); });
            return data;
        }
    }
    #endregion

    #region ExcelData
    public struct SWeapon : ISExcel
    {
        public int m_Index { get; private set; }
        public bool m_Hidden { get; private set; }
        public enum_Rarity m_Rarity { get; private set; }
        public float m_Damage { get; private set; }
        public float m_DamagePerEnhance { get; private set; }
        public float m_FireRate { get; private set; }
        public int m_ClipAmount { get; private set; }
        public float m_RefillTime { get; private set; }
        public float m_RecoilPerShot { get; private set; }

        public float m_UIDamage { get; private set; }
        public float m_UIRPM { get; private set; }
        public float m_UIStability { get; private set; }
        public float m_UISpeed { get; private set; }
        public enum_PlayerWeapon m_Weapon => (enum_PlayerWeapon)m_Index;

        public void InitAfterSet()
        {
        }
    }

    public struct SBuff : ISExcel
    {
        public int m_Index { get; private set; }
        public int m_Refresh { get; private set; }
        public float m_ExpireDuration { get; private set; }
        public int m_EffectIndex { get; private set; }
        public float m_MovementSpeedMultiply { get; private set; }
        public float m_FireRateMultiply { get; private set; }
        public float m_ReloadRateMultiply { get; private set; }
        public float m_DamageMultiply { get; private set; }
        public float m_DamageReduction { get; private set; }
        public float m_DamageTickTime { get; private set; }
        public float m_DamagePerTick { get; private set; }
        public enum_DamageType m_DamageType { get; private set; }
        public enum_ExpireRefreshType m_AddType => (enum_ExpireRefreshType)m_Refresh;
        public void InitAfterSet()
        {
            m_MovementSpeedMultiply /= 100f;
            m_FireRateMultiply /= 100f;
            m_ReloadRateMultiply /= 100f;
            m_DamageMultiply /= 100f;
            m_DamageReduction /= 100f;
        }
        //Normally In Excel 0-999
        public static SBuff CreateGameBethBuff(float fireRate,float duration)
        {
            SBuff buff= new SBuff();
            buff.m_Index = (int)enum_PlayerCharacter.Beth;
            buff.m_Refresh = (int)enum_ExpireRefreshType.AddUp;
            buff.m_FireRateMultiply = fireRate;
            buff.m_ExpireDuration = duration;
            return buff;
        }
    }

    public struct SEnermyGenerate : ISExcel
    {
        public List<int> m_EnermyGenerate { get; private set; }

        public void InitAfterSet()
        {
        }
    }
    #endregion
    #endregion
}