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
        /// <summary>
        /// 从场景带到战斗场景的武器
        /// </summary>
        public static enum_PlayerWeaponIdentity[] m_bearArmsList=new enum_PlayerWeaponIdentity[] { enum_PlayerWeaponIdentity .Invalid, enum_PlayerWeaponIdentity.Invalid };



        /// <summary>
        /// 杀怪个数
        /// </summary>
        public static int  m_killMonsters = 0;
        /// <summary>
        /// 通过关卡数
        /// </summary>
        public static int m_PassTheGate = 0;
        /// <summary>
        /// 获得金币数量
        /// </summary>
        public static int m_getGoldCoins = 0;
        /// <summary>
        /// 击杀boss数量
        /// </summary>
        public static int m_killBoss = 0;
        /// <summary>
        /// 开启信号塔
        /// </summary>
        public static int m_portal = 0;
        /// <summary>
        /// 关卡
        /// </summary>
        public static int m_passThe = 4;
        /// <summary>
        /// 通过某个关卡金币
        /// </summary>
        public static int m_passTheGate = 0;
        /// <summary>
        /// 通过某个关卡钻石
        /// </summary>
        public static int m_passTheGateNew = 0;
        /// <summary>
        /// 获得武器最高星级
        /// </summary>
        public static int m_getWeapons = 0;

        public static CGameSave m_GameData => TGameData<CGameSave>.Data;
        public static CArmoryData m_ArmoryData => TGameData<CArmoryData>.Data;
        public static CPlayerCharactersCultivateData m_CharacterData => TGameData<CPlayerCharactersCultivateData>.Data;
        public static CBattleSave m_GameProgressData => TGameData<CBattleSave>.Data;
        public static CGameTask m_GameTaskData => TGameData<CGameTask>.Data;
        public static CGameShop m_CGameShopData => TGameData<CGameShop>.Data;
        public static CGameDrawWeapon m_CGameDrawWeaponData => TGameData<CGameDrawWeapon>.Data;
        /// <summary>
        /// 当前选择true=新游戏   false=继续游戏
        /// </summary>
        public static bool m_BattleResume = false;

        public static bool m_Inited { get; private set; } = false;
        public static void InitializeTask()
        {
            m_killMonsters=0;
            m_PassTheGate=0;
            m_getGoldCoins=0;
            m_killBoss=0;
            m_portal=0;
            m_passTheGate=0;
            m_passTheGateNew=0;
            m_getWeapons = 0;
        }
        public static void Init()
        {
            if (m_Inited) return;
            m_Inited = true;
            Properties<SWeaponInfos>.Init();
            Properties<SBuff>.Init();
            SheetProperties<SEnermyGenerate>.Init();

            TGameData<CPlayerCharactersCultivateData>.Init();
            TGameData<CArmoryData>.Init();
            TGameData<CGameSave>.Init();
            TGameData<CGameTask>.Init();
            TGameData<CGameShop>.Init();
            TGameData<CGameDrawWeapon>.Init();

            InitPlayerPerks();
            InitEnermyPerks();
            InitArmory();

            TGameData<CBattleSave>.Init();
        }

        #region GameSave
        public static void OnNewBattle()
        {
            TGameData<CBattleSave>.Reset();
            TGameData<CBattleSave>.Save();
            m_GameData.m_BattleResume = true;
            TGameData<CGameSave>.Save();
        }

        public static void OnBattleStageSave(BattleManager game)
        {
            m_GameProgressData.Adjust(game.m_LocalPlayer, game.m_BattleProgress, game.m_BattleEntity);
            TGameData<CBattleSave>.Save();
        }
        
        public static void OnGameResult( BattleProgressManager progress,BattleEntityManager entity)
        {
            m_GameData.m_BattleResume = false;
            if (progress .m_GameWin&& m_GameData.m_BattleDifficulty == m_GameData.m_DifficultyUnlocked && m_GameData.m_DifficultyUnlocked != enum_BattleDifficulty.Hard)
            {
                m_GameData. m_DifficultyUnlocked++;
                m_GameData.m_BattleDifficulty++;
            }

            float stageCredit = ((int)progress.m_Stage-1) *GameConst.F_GameResultCreditStageBase;
            float killCredit = entity.m_EnermyKilled *GameConst.F_GameResultCreditEnermyKilledBase;
            float difficultyBonus = (1f+((int)entity.m_Difficulty-1) *GameConst.F_GameResultCreditDifficultyBonus);
            m_GameData.m_Credit += (stageCredit+killCredit)*difficultyBonus;
            TGameData<CGameSave>.Save();
        }

        #endregion

        #region GameData
        public static bool CanUseCredit(float credit) => m_GameData.m_Credit >= credit;
        /// <summary>
        /// 金币改变
        /// </summary>
        /// <param name="credit"></param>
        public static bool OnCreditStatus(float credit)
        {
            if (credit == 0)
                return false;
            if ((m_GameData.m_Diamonds + credit) < 0)
                return false;

            m_GameData.m_Credit += credit;
            TGameData<CGameSave>.Save();
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampCurrencyStatus);
            return true;
        }
        public static bool CanUseDiamonds(float diamonds) => m_GameData.m_Diamonds >= diamonds;
        /// <summary>
        /// 钻石改变
        /// </summary>
        /// <param name="diamonds"></param>
        public static bool OnDiamondsStatus(float diamonds)
        {
            if (diamonds == 0)
                return false ;

            if ((m_GameData.m_Diamonds + diamonds) < 0)
                return false;
            m_GameData.m_Diamonds += diamonds;
            TGameData<CGameSave>.Save();
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampDiamondsStatus);
            return true;
        }

        public static enum_BattleDifficulty OnCampDifficultySwitch()
        {
            m_GameData.m_BattleDifficulty ++;
            if (m_GameData.m_BattleDifficulty > m_GameData.m_DifficultyUnlocked)
                m_GameData.m_BattleDifficulty =  enum_BattleDifficulty.Normal;

            TGameData<CGameSave>.Save();
            return m_GameData.m_BattleDifficulty;
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
        public static Dictionary<enum_PlayerWeaponIdentity, SWeaponInfos> m_AvailableWeapons { get; private set; } = new Dictionary<enum_PlayerWeaponIdentity, SWeaponInfos>();
        public static Dictionary<enum_Rarity, List<enum_PlayerWeaponIdentity>> m_GameWeaponUnlocked { get; private set; } = new Dictionary<enum_Rarity, List<enum_PlayerWeaponIdentity>>();

        static void InitArmory()
        {
            m_AvailableWeapons.Clear();

            Properties<SWeaponInfos>.PropertiesList.Traversal((SWeaponInfos weapon) =>
            {
                if (weapon.m_Hidden)
                    return;
                m_AvailableWeapons.Add(weapon.m_Weapon, weapon);
            });
            CheckArmoryUnlocked();
        }

        static void CheckArmoryUnlocked()
        {
            m_GameWeaponUnlocked.Clear();
            m_AvailableWeapons.Traversal((SWeaponInfos weapon) =>
            {
                if (!m_ArmoryData.m_WeaponsUnlocked.Contains(weapon.m_Weapon))
                    return;
                if (!m_GameWeaponUnlocked.ContainsKey(weapon.m_Rarity))
                    m_GameWeaponUnlocked.Add(weapon.m_Rarity, new List<enum_PlayerWeaponIdentity>());
                m_GameWeaponUnlocked[weapon.m_Rarity].Add(weapon.m_Weapon);
            });
        }

        public static WeaponSaveData RandomUnlockedWeaponData(enum_Rarity rarity, enum_BattleStage stage, System.Random random = null) => RandomUnlockedWeaponData(rarity, GameConst.m_StageWeaponEnhanceLevel[stage].RandomPercentage(0, random), random);
        public static WeaponSaveData RandomUnlockedWeaponData(enum_Rarity rarity, int enhance,System.Random random=null)
        {
            if (!m_GameWeaponUnlocked.ContainsKey(rarity))
                rarity = enum_Rarity.Ordinary;
            return WeaponSaveData.New(m_GameWeaponUnlocked[rarity].RandomItem(random),enhance);
        }
        public static WeaponSaveData CharacterStartWeaponData(enum_PlayerCharacter character, int enhance) => WeaponSaveData.New(GameConst.m_CharacterStartWeapon[character],enhance);

        public static int GetArmoryUnlockPrice(enum_PlayerWeaponIdentity weapon) => GameConst.m_ArmoryBlueprintUnlockPrice[GetWeaponProperties(weapon).m_Rarity];
        public static bool CanArmoryUnlock(enum_PlayerWeaponIdentity weapon) => m_GameData.m_Credit >= GetArmoryUnlockPrice(weapon);
        public static void OnArmoryUnlock(enum_PlayerWeaponIdentity weapon)
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
            TGameData<CArmoryData>.Save();
            CheckArmoryUnlocked();
        }

        public static enum_PlayerWeaponIdentity UnlockArmoryBlueprint(enum_Rarity _spawnRarity)
        {
            Dictionary<enum_Rarity, List<enum_PlayerWeaponIdentity>> _blueprintAvailable = new Dictionary<enum_Rarity, List<enum_PlayerWeaponIdentity>>();
            m_AvailableWeapons.Traversal((enum_PlayerWeaponIdentity weapon, SWeaponInfos weaponData) =>
            {
                if (m_ArmoryData.m_WeaponBlueprints.Contains(weapon) || m_ArmoryData.m_WeaponsUnlocked.Contains(weapon))
                    return;
                if (!_blueprintAvailable.ContainsKey(weaponData.m_Rarity))
                    _blueprintAvailable.Add(weaponData.m_Rarity, new List<enum_PlayerWeaponIdentity>());
                _blueprintAvailable[weaponData.m_Rarity].Add(weapon);
            });

            enum_PlayerWeaponIdentity bluePrint = enum_PlayerWeaponIdentity.Invalid;
            if (_blueprintAvailable.ContainsKey(_spawnRarity))
                bluePrint = _blueprintAvailable[_spawnRarity].RandomItem();
            else if (_blueprintAvailable.ContainsKey(enum_Rarity.Ordinary))
                bluePrint = _blueprintAvailable[enum_Rarity.Ordinary].RandomItem();

            if (bluePrint != enum_PlayerWeaponIdentity.Invalid)
            {
                m_ArmoryData.m_WeaponBlueprints.Add(bluePrint);
                TGameData<CArmoryData>.Save();
                CheckArmoryUnlocked();
            }

            return bluePrint;
        }
        #endregion

        #region CharacterData
        public static bool CheckCharacterEquipping(enum_PlayerCharacter character) => m_CharacterData.m_CharacterSelected == character;
        public static void DoSwitchCharacter(enum_PlayerCharacter character)
        {
            if (CheckCharacterEquipping(character)&&CheckCharacterUnlocked(character))
            {
                Debug.LogError("Can't Change To Invalid Character!");
                return;
            }

            m_CharacterData.ChangeSelectedCharacter(character);
            TGameData<CPlayerCharactersCultivateData>.Save();
        }
        /// <summary>
        /// 随机获取一个没有的角色
        /// </summary>
        public static enum_PlayerCharacter RandomPlayerCharacter()
        {
            List<enum_PlayerCharacter> PlayerCharacter = new List<enum_PlayerCharacter>();
            enum_PlayerCharacter[] PlayerCharacterList = Enum.GetValues(typeof(enum_PlayerCharacter)) as enum_PlayerCharacter[];

            for (int i = 0; i < PlayerCharacterList.Length; i++)
            {
                if (PlayerCharacterList[i] != enum_PlayerCharacter.Invalid && !CheckCharacterUnlocked(PlayerCharacterList[i]))
                {
                    PlayerCharacter.Add(PlayerCharacterList[i]);
                }
            }
            if (PlayerCharacter.Count <= 0)
                return enum_PlayerCharacter.Invalid;
            System.Random random = new System.Random();
            return  PlayerCharacter[random.Next(0, PlayerCharacter.Count)];
        }

        /// <summary>
        /// 随机获取一个武器
        /// </summary>
        public static enum_PlayerWeaponIdentity RandomWeapon()
        {
            List<enum_PlayerWeaponIdentity> PlayerWeapon = new List<enum_PlayerWeaponIdentity>();
            enum_PlayerWeaponIdentity[] PlayerWeaponList = Enum.GetValues(typeof(enum_PlayerWeaponIdentity)) as enum_PlayerWeaponIdentity[];

            for (int i = 0; i < PlayerWeaponList.Length; i++)
            {
                if (PlayerWeaponList[i] != enum_PlayerWeaponIdentity.Invalid)
                {
                    PlayerWeapon.Add(PlayerWeaponList[i]);
                }
            }
            if (PlayerWeapon.Count <= 0)
                return enum_PlayerWeaponIdentity.Invalid;
            System.Random random = new System.Random();
            return PlayerWeapon[random.Next(0, PlayerWeapon.Count)];
        }

        /// <summary>
        /// 随机获取一个未拥有的武器图纸
        /// </summary>
        public static enum_PlayerWeaponIdentity RandomWeaponDrawing()
        {
            List<enum_PlayerWeaponIdentity> PlayerWeapon = new List<enum_PlayerWeaponIdentity>();
            enum_PlayerWeaponIdentity[] PlayerWeaponList = Enum.GetValues(typeof(enum_PlayerWeaponIdentity)) as enum_PlayerWeaponIdentity[];

            for (int i = 0; i < PlayerWeaponList.Length; i++)
            {
                if (PlayerWeaponList[i] != enum_PlayerWeaponIdentity.Invalid&&(!m_ArmoryData.m_WeaponsUnlocked.Contains(PlayerWeaponList[i])&& !m_ArmoryData.m_WeaponBlueprints.Contains(PlayerWeaponList[i])))
                {
                    SWeaponInfos weaponInfo = GetWeaponProperties(PlayerWeaponList[i]);
                    if (!weaponInfo.m_Hidden)
                    {
                        PlayerWeapon.Add(PlayerWeaponList[i]);
                    }
                }
            }
            if (PlayerWeapon.Count <= 0)
                return enum_PlayerWeaponIdentity.Invalid;
            System.Random random = new System.Random();
            return PlayerWeapon[random.Next(0, PlayerWeapon.Count)];
        }
        public static bool CheckCharacterUnlocked(enum_PlayerCharacter character) => m_CharacterData.GetCharacterCultivateDetail(character).m_Unlocked;
        public static float GetCharacterUnlockPrice(enum_PlayerCharacter character)
        {
            if (!GameConst.m_CharacterUnlockCost.ContainsKey(character))
            {
                Debug.LogError("Invalid Character Found From Dic!" + character);
                return -1;
            }
            return GameConst.m_CharacterUnlockCost[character];
        }
        public static bool CanCharacterUnlock(enum_PlayerCharacter character) => !CheckCharacterUnlocked(character) &&CanUseCredit(GetCharacterUnlockPrice(character));
        public static void DoUnlockCharacter(enum_PlayerCharacter character)
        {
            if (!CanCharacterUnlock(character))
            {
                Debug.LogError("Can't Unlock Invalid Character!");
                return;
            }
            OnCreditStatus(-GetCharacterUnlockPrice(character));
            m_CharacterData.DoUnlock(character);
            TGameData<CPlayerCharactersCultivateData>.Save();
        }


        public static bool CheckCharacterEnhancable(enum_PlayerCharacter character) => m_CharacterData.GetCharacterCultivateDetail(character).CanEnhance();
        public static float GetCharacterEnhancePrice(enum_PlayerCharacter character)
        {
            if(!CheckCharacterEnhancable(character))
            {
                Debug.LogError("Invlaid Character Enhance!");
                return 0;
            }
            enum_PlayerCharacterEnhance enhance = m_CharacterData.GetCharacterCultivateDetail(character).NextEnhance();
            if (!GameConst.m_CharacterEnhanceCost.ContainsKey(enhance))
            {
                Debug.LogError("Invalid Character Enhance From Dic!" + enhance);
                return 0;
            }
            return GameConst.m_CharacterEnhanceCost[enhance];
        }
        public static bool CanEnhanceCharacter(enum_PlayerCharacter character) => CheckCharacterEnhancable(character)&&CanUseCredit(GetCharacterEnhancePrice(character));
        public static void DoEnhanceCharacter(enum_PlayerCharacter character)
        {
            if(!CanEnhanceCharacter(character))
            {
                Debug.LogError("Can't Enhance A Invalid Character!");
                return;
            }
            OnCreditStatus(-GetCharacterEnhancePrice(character));
            m_CharacterData.DoEnhance(character);
            TGameData<CPlayerCharactersCultivateData>.Save();
        }
        #endregion
        /// <summary>
        /// 任务储存
        /// </summary>
        /// <param name="credit"></param>
        #region ArmoryData
        /// <summary>
        /// 每天24点随机任务
        /// </summary>
        public static void RandomTask()
        {
            m_GameTaskData.RandomTask();
            TGameData<CGameTask>.Save();
        }
        /// <summary>
        /// 储存任务进度
        /// </summary>
        public static void OnCGameTask()
        {
            m_GameTaskData.m_killMonsters += m_killMonsters;
            m_GameTaskData.m_PassTheGate += m_PassTheGate;
            m_GameTaskData.m_getGoldCoins += m_getGoldCoins;
            m_GameTaskData.m_killBoss += m_killBoss;
            m_GameTaskData.m_portal += m_portal;
            m_GameTaskData.m_passTheGate += m_passTheGate;
            m_GameTaskData.m_passTheGateNew += m_passTheGateNew;
            m_GameTaskData.m_getWeapons = m_getWeapons;
            TGameData<CGameTask>.Save();
            //Debug.Log(m_GameTaskData.m_killMonsters);
            InitializeTask();
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
        static Dictionary<int, ExpireBattleCharacterBase> m_AllEnermyPerks = new Dictionary<int, ExpireBattleCharacterBase>();
        static Dictionary<int, Type> m_AllGameCharcterPerkTypes = new Dictionary<int, Type>();
        static List<int> m_AvailableEnermyPerks = new List<int>();
        public static void InitEnermyPerks()
        {
            m_AllEnermyPerks.Clear();
            m_AllGameCharcterPerkTypes.Clear();
            m_AvailableEnermyPerks.Clear();
            TReflection.TraversalAllInheritedClasses(((Type type, ExpireBattleCharacterBase perk) => {
                m_AllEnermyPerks.Add(perk.m_Index, perk);
                m_AllGameCharcterPerkTypes.Add(perk.m_Index, perk.GetType());
                if (perk.m_Index == m_DefaultEnermyPerkIdentity)
                    return;
                m_AvailableEnermyPerks.Add(perk.m_Index);
            }),0,0);
        }

        public static ExpireBattleCharacterBase RandomEnermyPerk(int minutesPassed,enum_BattleDifficulty difficulty,bool isElite)=>TReflection.CreateInstance<ExpireBattleCharacterBase>(m_AllGameCharcterPerkTypes[isElite ? m_AvailableEnermyPerks.RandomItem() : m_DefaultEnermyPerkIdentity], GameExpression.GetEnermyMaxHealthMultiplier(minutesPassed, difficulty),GameExpression.GetEnermyDamageMultilier(minutesPassed,difficulty));
        public static ExpireBattleCharacterBase DefaultGameCharacterPerk(float healthMultiplier,float damageMultiplier) => TReflection.CreateInstance<ExpireBattleCharacterBase>(m_AllGameCharcterPerkTypes[m_DefaultEnermyPerkIdentity], healthMultiplier, damageMultiplier);
        public static bool IsElitePerk(this ExpireBattleCharacterBase perk) => perk.m_Index != m_DefaultEnermyPerkIdentity;
        #endregion

        #region ExcelData
        public static enum_PlayerWeaponIdentity TryGetWeaponEnum(string weaponIdentity)
        {
            int idTry = -1;
            if (int.TryParse(weaponIdentity, out idTry) && Enum.IsDefined(typeof(enum_PlayerWeaponIdentity), idTry))
                return (enum_PlayerWeaponIdentity)idTry;

            enum_PlayerWeaponIdentity targetWeapon = enum_PlayerWeaponIdentity.Invalid;
            if (Enum.TryParse(weaponIdentity, out targetWeapon))
                return targetWeapon;

            if (Properties<SWeaponInfos>.PropertiesList.Any(p => TLocalization.GetKeyLocalized(p.m_Weapon.GetNameLocalizeKey()) == weaponIdentity))
                return Properties<SWeaponInfos>.PropertiesList.Find(p => TLocalization.GetKeyLocalized(p.m_Weapon.GetNameLocalizeKey()) == weaponIdentity).m_Weapon;

            Debug.LogError("Invalid Player Weapon Found!");
            return enum_PlayerWeaponIdentity.Invalid;
        }

        public static SWeaponInfos GetWeaponProperties(enum_PlayerWeaponIdentity type)
        {
            SWeaponInfos weapon = Properties<SWeaponInfos>.PropertiesList.Find(p => p.m_Weapon == type);
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

        public static List<SEnermyGenerate> GetEnermyGenerate(enum_BattleStage stage,enum_BattleDifficulty difficulty)=> SheetProperties<SEnermyGenerate>.GetPropertiesList(((int)difficulty-1)*5+((int)stage-1));
        #endregion
    }
    #region Structs
    #region SaveData
    public class CGameSave : ISave
    {
        public bool m_BattleResume;
        public enum_BattleDifficulty m_BattleDifficulty;
        public enum_BattleDifficulty m_DifficultyUnlocked;
        float m_credit=0;
        public float m_Credit
        {
            get {
                //Debug.Log("读取"+ m_credit);
                return m_credit;
            }
            set
            {
                //Debug.Log("录入" + value);
                m_credit = value;
            }
        }
        public float m_diamonds;
        public float m_Diamonds
        {
            get
            {
                //Debug.Log("读取" + m_diamonds);
                return m_diamonds;
            }
            set
            {
                //Debug.Log("录入" + value);
                m_diamonds = value;
            }
        }
        public int m_LastDailyRewardStamp;

        public CGameSave()
        {
            m_Credit = 1000000;
            m_Diamonds = 5000;
            m_BattleDifficulty =  enum_BattleDifficulty.Normal;
            m_DifficultyUnlocked =  enum_BattleDifficulty.Normal;
            m_LastDailyRewardStamp = -1;
            m_BattleResume = false;
        }


        public bool CheckDailyReward()
        {
            int currentDayStamp = TTime.TTimeTools.GetDayStampNow();
            if (currentDayStamp <= m_LastDailyRewardStamp)
                return false;
            m_LastDailyRewardStamp = currentDayStamp;
            return true;
        }

        public bool DataCrypt() => true;
        public void DataRecorrect()
        {
        }
    }

    public class CBattleSave : ISave
    {
        public string m_GameSeed;
        public enum_BattleDifficulty m_BattleDifficulty;

        public enum_BattleStage m_Stage;
        public float m_TimeElapsed;
        public float m_Health;
        public int m_Keys;
        public int m_TotalExp;
        public enum_PlayerCharacter m_Character;
        public enum_PlayerCharacterEnhance m_Enhance;
        public WeaponSaveData m_Weapon1;
        public WeaponSaveData m_Weapon2;
        public List<PerkSaveData> m_Perks;

        public List<enum_PlayerWeaponIdentity> m_ArmoryBlueprintsUnlocked;
        public int m_EnermyKilled;
        public CBattleSave()
        {
            enum_PlayerCharacter characterSelected = GameDataManager.m_CharacterData.m_CharacterSelected;
            enum_PlayerCharacterEnhance playerEnhance = GameDataManager.m_CharacterData.GetCharacterCultivateDetail(characterSelected).m_Enhance;
            SetData(GameDataManager.m_GameData.m_BattleDifficulty, characterSelected, playerEnhance, GameDataManager.CharacterStartWeaponData(characterSelected, playerEnhance >= enum_PlayerCharacterEnhance.StartWeapon ? 1 : 0), WeaponSaveData.New( enum_PlayerWeaponIdentity.Invalid));
        }

        public CBattleSave SetData(enum_BattleDifficulty difficulty, enum_PlayerCharacter character,enum_PlayerCharacterEnhance enhance, WeaponSaveData weapon1,WeaponSaveData weapon2)
        {
            m_EnermyKilled = 0;
            m_GameSeed = DateTime.Now.ToLongTimeString();
            m_BattleDifficulty = difficulty;
            m_TimeElapsed = 0;
            m_Keys = 0;
            m_TotalExp = 0;
            m_Health = -1;
            m_Character = character;
            m_Enhance = enhance;
            m_Perks = new List<PerkSaveData>();
            m_Weapon1 = weapon1;
            m_Weapon2 = weapon2;
            m_Stage = enum_BattleStage.Rookie;
            m_ArmoryBlueprintsUnlocked = new List<enum_PlayerWeaponIdentity>();
            return this;
        }

        public void Adjust(EntityCharacterPlayer _player, BattleProgressManager _level,BattleEntityManager _battle)
        {
            m_TimeElapsed = _battle.m_TimeElapsed;
            m_EnermyKilled = _battle.m_EnermyKilled;

            m_Stage = _level.m_Stage;

            m_Character = _player.m_Character;
            m_Enhance = _player.m_Enhance;
            m_Keys = _player.m_CharacterInfo.m_Keys;
            m_TotalExp = _player.m_CharacterInfo.m_RankManager.m_TotalExpOwned;
            m_Health = _player.m_Health.m_CurrentHealth;

            m_Weapon1 = WeaponSaveData.Save(_player.m_Weapon1);
            m_Weapon2 = WeaponSaveData.Save(_player.m_Weapon2);
            m_Perks = PerkSaveData.Save(_player.m_CharacterInfo.m_ExpirePerks.Values.ToList());
            m_ArmoryBlueprintsUnlocked = _level.m_ArmoryBlueprintsUnlocked;
        }

        public bool DataCrypt() => true;
        void ISave.DataRecorrect()
        {
        }
    }

    public class CArmoryData : ISave
    {
        /// <summary>
        /// 解锁武器
        /// </summary>
        public List<enum_PlayerWeaponIdentity> m_WeaponsUnlocked;
        public List<enum_PlayerWeaponIdentity> m_WeaponBlueprints;

        public CArmoryData()
        {
            m_WeaponsUnlocked = new List<enum_PlayerWeaponIdentity>() { enum_PlayerWeaponIdentity.P92, enum_PlayerWeaponIdentity.SpawnerBlastTurret, enum_PlayerWeaponIdentity.UZI, enum_PlayerWeaponIdentity.LavaWand, enum_PlayerWeaponIdentity.Bow, enum_PlayerWeaponIdentity.Kar98, enum_PlayerWeaponIdentity.S686, enum_PlayerWeaponIdentity.BloodThirster, enum_PlayerWeaponIdentity.Grenade, enum_PlayerWeaponIdentity.HeavySword, enum_PlayerWeaponIdentity.Minigun, enum_PlayerWeaponIdentity.MultishotBow, enum_PlayerWeaponIdentity.RocketLauncher, enum_PlayerWeaponIdentity.Driller };
            m_WeaponBlueprints = new List<enum_PlayerWeaponIdentity>() { enum_PlayerWeaponIdentity.Flamer };
        }

        public bool DataCrypt() => true;
        public void DataRecorrect()
        {
        }
    }

    public class CPlayerCharactersCultivateData:ISave
    {
        public enum_PlayerCharacter m_CharacterSelected;
        public Dictionary<enum_PlayerCharacter, PlayerCharacterCultivateSaveData> m_CharacterDetail;

        public CPlayerCharactersCultivateData()
        {
            m_CharacterSelected = enum_PlayerCharacter.Beth;
            m_CharacterDetail = new Dictionary<enum_PlayerCharacter, PlayerCharacterCultivateSaveData>() { { enum_PlayerCharacter.Beth, new PlayerCharacterCultivateSaveData(true, enum_PlayerCharacterEnhance.None)} };
        }

        public void ChangeSelectedCharacter(enum_PlayerCharacter character) => m_CharacterSelected = character;

        void Check(enum_PlayerCharacter character)
        {
            if (!m_CharacterDetail.ContainsKey(character))
                m_CharacterDetail.Add(character, new PlayerCharacterCultivateSaveData(false, enum_PlayerCharacterEnhance.None));
        }

        public PlayerCharacterCultivateSaveData GetCharacterCultivateDetail(enum_PlayerCharacter character)
        {
            Check(character);
            return m_CharacterDetail[character];
        }
        public void DoUnlock(enum_PlayerCharacter character)
        {
            Check(character);

            if (m_CharacterDetail[character].m_Unlocked)
            {
                Debug.LogError("Can't Unlock Unlocked!");
                return;
            }
            m_CharacterDetail[character] = m_CharacterDetail[character].Unlock();
        }

        public void DoEnhance(enum_PlayerCharacter character)
        {
            Check(character);
            if(m_CharacterDetail[character].m_Enhance>= enum_PlayerCharacterEnhance.Max)
            {
                Debug.LogError("Can't Enhance Max!");
                return;
            }
            m_CharacterDetail[character] = m_CharacterDetail[character].Enhance();
        }

        public bool DataCrypt() => true;
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

        public bool DataCrypt() => false;
        void ISave.DataRecorrect()
        {
        }
    }
    /// <summary>
    /// 任务记录
    /// </summary>
    public class CGameTask : ISave
    {
        //上次保存时间
        public int m_preservation;
        //金币任务
        public int m_goldCoinTask;
        //钻石任务
        public int m_diamondMission;
        //金币任务状态
        public int m_goldCoinTaskState=0;
        //钻石任务状态
        public int m_diamondMissionState=0 ;
        /// <summary>
        /// 每日登陆
        /// </summary>
        public int m_signIn=0;
        /// <summary>
        /// 看广告次数
        /// </summary>
        public int m_advertisement=0;
        /// <summary>
        /// 看广告领取次数
        /// </summary>
        public int m_advertisementCollection=0;
        /// <summary>
        /// 杀怪个数
        /// </summary>
        public int m_killMonsters = 0;
        /// <summary>
        /// 通过关卡数
        /// </summary>
        public int m_PassTheGate = 0;
        /// <summary>
        /// 获得金币数量
        /// </summary>
        public int m_getGoldCoins = 0;
        /// <summary>
        /// 击杀boss数量
        /// </summary>
        public int m_killBoss = 0;
        /// <summary>
        /// 开启信号塔
        /// </summary>
        public int m_portal = 0;
        /// <summary>
        /// 通过某个关卡金币
        /// </summary>
        public int m_passTheGate = 0;
        /// <summary>
        /// 通过某个关卡钻石
        /// </summary>
        public int m_passTheGateNew = 0;
        /// <summary>
        /// 获得武器最高星级
        /// </summary>
        public int m_getWeapons = 0;

        /// <summary>
        /// 每天24点随机任务
        /// </summary>
        public void RandomTask()
        {
            if (m_preservation == 0 || !IsTodayTimeBySpan(m_preservation))
            {
                m_goldCoinTask = UnityEngine.Random.Range(0, 7);
                m_diamondMission = UnityEngine.Random.Range(0, 7);
                Debug.Log("任务重置" + m_preservation);
                m_goldCoinTaskState = 0;
                m_diamondMissionState = 0;
                m_preservation = GetTimeStamp();
                m_signIn = 1;
                m_advertisement = 0;
                m_advertisementCollection = 0;
                m_killMonsters = 0;
                m_PassTheGate = 0;
                m_getGoldCoins = 0;
                m_killBoss = 0;
                m_portal = 0;
                m_passTheGate = 0;
                m_passTheGateNew = 0;
                m_getWeapons = 0;
                GameDataManager.InitializeTask();
                //商品刷新
                GameDataManager.m_CGameShopData.Random();
            }
        }
        //public CGameTask()
        //{
        //    m_killMonsters = 10;
        //}

        public bool DataCrypt() => false;
        void ISave.DataRecorrect()
        {
        }
        /// <summary>
        /// 判断上次保存是否是当天
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public  bool IsTodayTimeBySpan(int num)
        {
            if (num == 0) return false;
            DateTime nowTime = DateTime.Now;
            DateTime dt = GetDateTime(num);
            Debug.Log(string.Format("保存时间{0}———当前时间{1}", dt, nowTime));
            if (dt.Date == nowTime.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// 获取当前时间戳
        /// </summary>
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>
        /// <returns></returns>
        public  int GetTimeStamp(bool bflag = true)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ret;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds);
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds);
            return (int)ret;
        }
        /// <summary>    
        /// 时间戳Timestamp转换成日期    
        /// </summary>    
        /// <param name="timeStamp"></param>    
        /// <returns></returns>    
        private DateTime GetDateTime(int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = ((long)timeStamp * 10000000);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }

    }
    /// <summary>
    /// 商店记录
    /// </summary>
    public class CGameShop : ISave
    {
        int m_vip;
        public int m_Vip 
        {
            get {
                return m_vip;
            }
            set {
                m_vip = value;
                TGameData<CGameShop>.Save();
            }
        }
        public int m_noviceGiftBag = 0;
        public int m_NoviceGiftBag
        {
            get
            {
                return m_noviceGiftBag;
            }
            set
            {
                m_noviceGiftBag = value;
                TGameData<CGameShop>.Save();
            }
        }

        public int m_roleId;

        public void Random()
        {
            m_roleId = (int)GameDataManager.RandomPlayerCharacter();
            Debug.Log(m_roleId);
            TGameData<CGameShop>.Save();
        }

        public bool DataCrypt() => false;
        void ISave.DataRecorrect()
        {
        }
    }
    /// <summary>
    /// 抽奖武器记录
    /// </summary>
    public class CGameDrawWeapon : ISave
    {
        int m_currentValue = 0;
        //记录掉落的武器
        public enum_PlayerWeaponIdentity m_weapon0 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon1 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon2 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon3 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon4 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon5 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon6 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon7 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon8 = enum_PlayerWeaponIdentity.Invalid;
        public enum_PlayerWeaponIdentity m_weapon9 = enum_PlayerWeaponIdentity.Invalid;

        //记录掉落的武位置X
        public float m_weaponPosX0;
        public float m_weaponPosX1;
        public float m_weaponPosX2;
        public float m_weaponPosX3;
        public float m_weaponPosX4;
        public float m_weaponPosX5;
        public float m_weaponPosX6;
        public float m_weaponPosX7;
        public float m_weaponPosX8;
        public float m_weaponPosX9;

        //记录掉落的武器位置Y
        public float m_weaponPosZ0;
        public float m_weaponPosZ1;
        public float m_weaponPosZ2;
        public float m_weaponPosZ3;
        public float m_weaponPosZ4;
        public float m_weaponPosZ5;
        public float m_weaponPosZ6;
        public float m_weaponPosZ7;
        public float m_weaponPosZ8;
        public float m_weaponPosZ9;

        /// <summary>
        /// 添加武器储存
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="pos"></param>
        public void AddWeapon(enum_PlayerWeaponIdentity identity ,Vector3 pos)
        {
            switch (m_currentValue)
            {
                case 0:
                    m_weapon0 = identity;
                    m_weaponPosX0 = pos.x;
                    m_weaponPosZ0 = pos.z;
                    break;
                case 1:
                    m_weapon1 = identity;
                    m_weaponPosX1 = pos.x;
                    m_weaponPosZ1 = pos.z;
                    break;
                case 2:
                    m_weapon2 = identity;
                    m_weaponPosX2 = pos.x;
                    m_weaponPosZ2 = pos.z;
                    break;
                case 3:
                    m_weapon3 = identity;
                    m_weaponPosX3 = pos.x;
                    m_weaponPosZ3 = pos.z;
                    break;
                case 4:
                    m_weapon4 = identity;
                    m_weaponPosX4 = pos.x;
                    m_weaponPosZ4 = pos.z;
                    break;
                case 5:
                    m_weapon5 = identity;
                    m_weaponPosX5 = pos.x;
                    m_weaponPosZ5 = pos.z;
                    break;
                case 6:
                    m_weapon6 = identity;
                    m_weaponPosX6 = pos.x;
                    m_weaponPosZ6 = pos.z;
                    break;
                case 7:
                    m_weapon7 = identity;
                    m_weaponPosX7 = pos.x;
                    m_weaponPosZ7 = pos.z;
                    break;
                case 8:
                    m_weapon8 = identity;
                    m_weaponPosX8 = pos.x;
                    m_weaponPosZ8 = pos.z;
                    break;
                case 9:
                    m_weapon9 = identity;
                    m_weaponPosX9 = pos.x;
                    m_weaponPosZ9 = pos.z;
                    break;
            }
            m_currentValue++;
            if (m_currentValue > 9)
            {
                m_currentValue = 0;
            }
            TGameData<CGameDrawWeapon>.Save();
        }

        /// <summary>
        /// 获取武器储存
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="pos"></param>
        public enum_PlayerWeaponIdentity GetWeapon(int id)
        {
            switch (m_currentValue)
            {
                case 0:
                    return m_weapon0;
                case 1:
                    return m_weapon1;
                case 2:
                    return m_weapon2;
                case 3:
                    return m_weapon3;
                case 4:
                    return m_weapon4;
                case 5:
                    return m_weapon5;
                case 6:
                    return m_weapon6;
                case 7:
                    return m_weapon7;
                case 8:
                    return m_weapon8;
                case 9:
                    return m_weapon9;
            }
            return enum_PlayerWeaponIdentity.Invalid;
        }
        /// <summary>
        /// 获取武器储存位置
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="pos"></param>
        public Vector3 GetWeaponPos(int id)
        {
            switch (m_currentValue)
            {
                case 0:
                    return new Vector3(m_weaponPosX0, 0, m_weaponPosZ0);
                case 1:
                    return new Vector3(m_weaponPosX1, 0, m_weaponPosZ1);
                case 2:
                    return new Vector3(m_weaponPosX2, 0, m_weaponPosZ2);
                case 3:
                    return new Vector3(m_weaponPosX3, 0, m_weaponPosZ3);
                case 4:
                    return new Vector3(m_weaponPosX4, 0, m_weaponPosZ4);
                case 5:
                    return new Vector3(m_weaponPosX5, 0, m_weaponPosZ5);
                case 6:
                    return new Vector3(m_weaponPosX6, 0, m_weaponPosZ6);
                case 7:
                    return new Vector3(m_weaponPosX7, 0, m_weaponPosZ7);
                case 8:
                    return new Vector3(m_weaponPosX8, 0, m_weaponPosZ8);
                case 9:
                    return new Vector3(m_weaponPosX9, 0, m_weaponPosZ9);
            }
            return Vector3.zero;
        }
        public bool DataCrypt() => false;
        void ISave.DataRecorrect()
        {
        }
    }
    public struct WeaponSaveData : IDataConvert
    {
        public enum_PlayerWeaponIdentity m_Weapon { get; private set; }
        public int m_Enhance { get; private set; }

        public static WeaponSaveData New(enum_PlayerWeaponIdentity weapon,int enhanceLevel=0) => new WeaponSaveData() { m_Weapon = weapon,m_Enhance=enhanceLevel };
        public static WeaponSaveData Save(WeaponBase weapon) => new WeaponSaveData() { m_Weapon = weapon != null ? weapon.m_WeaponInfo.m_Weapon : enum_PlayerWeaponIdentity.Invalid, m_Enhance = weapon != null ? weapon.m_EnhanceLevel : 0 };
    }

    public struct PerkSaveData : IDataConvert
    {
        public int m_Index { get; private set; }
        public int m_PerkStack { get; private set; }
        public float m_RecordData { get; private set; }

        public static PerkSaveData New(int index) => new PerkSaveData() { m_Index = index, m_PerkStack = 1, m_RecordData = -1 };
        public static PerkSaveData Create(ExpirePlayerPerkBase perk) => new PerkSaveData() { m_Index = perk.m_Index, m_PerkStack = perk.m_Stack, m_RecordData = perk.m_RecordData };
        public static List<PerkSaveData> Save(List<ExpirePlayerPerkBase> perks)
        {
            List<PerkSaveData> data = new List<PerkSaveData>();
            perks.Traversal((ExpirePlayerPerkBase perk) => { data.Add(Create(perk)); });
            return data;
        }
    }
    
    public struct PlayerCharacterCultivateSaveData:IDataConvert
    {
        public bool m_Unlocked { get; private set; }
        public enum_PlayerCharacterEnhance m_Enhance { get; private set; }
        public PlayerCharacterCultivateSaveData(bool unlocked,enum_PlayerCharacterEnhance enhance)
        {
            m_Unlocked = unlocked;
            m_Enhance = enhance;
        }
        public PlayerCharacterCultivateSaveData Unlock()
        {
            m_Unlocked = true;
            return this;
        }
        public PlayerCharacterCultivateSaveData Enhance()
        {
            m_Enhance =NextEnhance();
            return this;
        }

        public bool CanEnhance() => m_Unlocked&&m_Enhance < enum_PlayerCharacterEnhance.Max-1;
        public enum_PlayerCharacterEnhance NextEnhance() => m_Enhance + 1;
    }
    #endregion

    #region ExcelData
    public struct SWeaponInfos : ISExcel
    {
        public int m_Index { get; private set; }
        public bool m_Hidden { get; private set; }
        public enum_Rarity m_Rarity { get; private set; }

        public int m_UICipAmount { get; private set; }
        public float m_UIScore1 { get; private set; }
        public float m_UIScore2 { get; private set; }
        public float m_UIScore3 { get; private set; }
        public float m_UIScore4 { get; private set; }
        public List<int> m_UITags { get; private set; }

        public enum_PlayerWeaponIdentity m_Weapon => (enum_PlayerWeaponIdentity)m_Index;

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

        public float m_DotTick { get; private set; }
        public float m_DotPercentage { get; private set; }
        public enum_DamageType m_DotType { get; private set; }

        public enum_ExpireRefreshType m_RefreshType => (enum_ExpireRefreshType)m_Refresh;
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