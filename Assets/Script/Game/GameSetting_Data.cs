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
        public static CEquipmentDepotData m_EquipmentDepotData => TGameData<CEquipmentDepotData>.Data;
        public static CCharacterUpgradeData m_CharacterData => TGameData<CCharacterUpgradeData>.Data;
        public static CPlayerBattleSave m_BattleData => TGameData<CPlayerBattleSave>.Data;

        public static bool m_Inited { get; private set; } = false;
        public static void Init()
        {
            if (m_Inited) return;
            m_Inited = true;
            Properties<SWeapon>.Init();
            Properties<SBuff>.Init();
            SheetProperties<SEnermyGenerate>.Init();

            InitPerks();
            InitArmory();
            InitEquipment();

            TGameData<CGameSave>.Init();
            TGameData<CEquipmentDepotData>.Init();
            TGameData<CCharacterUpgradeData>.Init();
            TGameData<CArmoryData>.Init();
            TGameData<CPlayerBattleSave>.Init();

            InitArmoryGameWeaponUnlocked();
        }
        #region GameSave
        public static void OnNewGame()
        {
            TGameData<CPlayerBattleSave>.Reset();
            TGameData<CPlayerBattleSave>.Save();
        }

        public static void StageFinishSaveData(EntityCharacterPlayer data, GameProgressManager level)
        {
            m_BattleData.Adjust(data, level);
            TGameData<CPlayerBattleSave>.Save();
        }

        public static void OnGameResult(GameProgressManager progress)
        {
            if (progress.m_GameWin)
                m_GameData.UnlockDifficulty();
            OnCreditStatus(progress.F_CreditGain);

            TGameData<CPlayerBattleSave>.Reset();
            TGameData<CPlayerBattleSave>.Save();
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
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_GameCurrencyStatus);
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

        #region Upgrade Combination
        static Dictionary<int, ExpireUpgrade> m_AllEquipments = new Dictionary<int, ExpireUpgrade>();
        static List<int> m_AvailableEquipments = new List<int>();
        public static void InitEquipment()
        {
            List<EquipmentSaveData> tempData1 = new List<EquipmentSaveData>();
            CharacterUpgradeData tempData2 = CharacterUpgradeData.Default();
            TReflection.TraversalAllInheritedClasses(((Type type, ExpireUpgrade equipment) => {
                m_AllEquipments.Add(equipment.m_Index, equipment);
                if (equipment.m_Index != GameConst.m_DefaultEquipmentCombinationIdentity)
                    m_AvailableEquipments.Add(equipment.m_Index);
            }), tempData1,tempData2);
        }

        public static ExpireUpgrade CreateUpgradeCombination(List<EquipmentSaveData> equipmentDatas,CharacterUpgradeData upgradeData)
        {
            int equipmentIndex = equipmentDatas.Find(p => equipmentDatas.FindAll(l => l.m_Index == p.m_Index).Count >= 2).m_Index;
            if (equipmentIndex <= 0)
            {
                equipmentIndex = GameConst.m_DefaultEquipmentCombinationIdentity;
            }
            else if (!m_AllEquipments.ContainsKey(equipmentIndex))
            {
                Debug.LogError("Error Equipment:" + equipmentIndex + " ,Does not exist");
                equipmentIndex = GameConst.m_DefaultEquipmentCombinationIdentity;
            }

            return TReflection.CreateInstance<ExpireUpgrade>(m_AllEquipments[equipmentIndex].GetType(), equipmentDatas, upgradeData);
        }
        #endregion

        #region Upgrade Equipment Data
        public static EquipmentSaveData RandomRarityEquipment(enum_Rarity rarity)
        {
            EquipmentSaveData data = new EquipmentSaveData(m_AvailableEquipments.RandomItem(), 0, rarity, new List<EquipmentEntrySaveData>());
            data.AcquireRandomEntry();
            int subEntryCount = TCommon.RandomPercentage(GameConst.m_EquipmentGenerateEntryCount[rarity]);
            for (int i = 0; i < subEntryCount; i++)
                data.AcquireRandomEntry();
            return data;
        }

        public static bool CheckEquipmentEquiping(int equipmentIndex) => m_EquipmentDepotData.m_Equipping.Contains(equipmentIndex);
        public static void DoEquipmentEquip(int equipmentIndex)
        {
            if (!CheckEquipmentEquiping(equipmentIndex))
            {
                if (m_EquipmentDepotData.m_Equipping.Count >= 3)
                    m_EquipmentDepotData.m_Equipping.RemoveAt(0);
                m_EquipmentDepotData.m_Equipping.Add(equipmentIndex);
            }
            else
            {
                m_EquipmentDepotData.m_Equipping.Remove(equipmentIndex);
            }
            TGameData<CEquipmentDepotData>.Save();
        }

        public static bool CheckEquipmentLocking(int equipmentIndex) => m_EquipmentDepotData.m_Locking.Contains(equipmentIndex);
        public static void DoEquipmentLock(int equipmentIndex)
        {
            if (!CheckEquipmentLocking(equipmentIndex))
                m_EquipmentDepotData.m_Locking.Add(equipmentIndex);
            else
                m_EquipmentDepotData.m_Locking.Remove(equipmentIndex);
            TGameData<CEquipmentDepotData>.Save();
        }
        #region Enhance
        #region Calculation
        public static int GetEnhanceLevel(int enhance, enum_Rarity rarity)
        {
            int level = 0;
            for (int i = 0; i < GameConst.m_EquipmentEnhanceMaxLevel; i++, level++)
            {
                enhance -= GameExpression.GetEquipmentEnhanceRequirement(rarity, level);
                if (enhance < 0)
                    break;
            }
            return level;
        }

        public static int GetNextLevelLeft(int enhance, enum_Rarity rarity)
        {
            for (int i = 0, level=0; i < GameConst.m_EquipmentEnhanceMaxLevel; i++, level++)
            {
                enhance -= GameExpression.GetEquipmentEnhanceRequirement(rarity, level);
                if (enhance < 0)
                    return -enhance;
            }
            return 0;
        }

        public static int GetTotalEnhance(int level,enum_Rarity rarity)
        {
            int enhance = 0;
            for(int i=0;i<level;i++)
                enhance += GameExpression.GetEquipmentEnhanceRequirement(rarity, i);
            return enhance;
        }
        #endregion
        #region EnhanceRequipment/DeconstructIncome

        public static int GetEnhanceLevel(this EquipmentSaveData data) => GetEnhanceLevel(data.m_Enhance,data.m_Rarity);
        public static int GetEnhanceRequireNextLevel(this EquipmentSaveData data) => GetNextLevelLeft(data.m_Enhance,data.m_Rarity);
        public static int GetEnhanceCurLevel(this EquipmentSaveData data) => data.m_Enhance - GetTotalEnhance(data.GetEnhanceLevel(),data.m_Rarity);

        public static int GetDeconstructIncome(this EquipmentSaveData data) => GameExpression.GetEquipmentDeconstruct(data.m_Rarity,data.GetEnhanceLevel());
        public static int GetDeconstructIncome(List<int> selections)
        {
            int enhanceReceived = 0;
            for (int i = 0; i < selections.Count; i++)
                enhanceReceived += m_EquipmentDepotData.m_Equipments[selections[i]].GetDeconstructIncome();
            return enhanceReceived;
        }
        #endregion

        #region Enhance/Deconstruct Equipment
        public static bool CanDeconstructEquipments(List<int> deconstructSelections, ref float credits)
        {
            credits = GetDeconstructIncome(deconstructSelections) * GameConst.m_EquipmentEnhanceCoinsCostMultiply;
            return CanUseCredit(credits);
        } 
        public static bool CanEnhanceEquipment(int targetSelection) => m_EquipmentDepotData.m_Equipments[targetSelection].GetEnhanceLevel() < GameConst.m_EquipmentEnhanceMaxLevel;
        public static int DoEnhanceEquipment(int targetIndex, List<int> deconstructSelections)
        {
            int enhanceReceived = GetDeconstructIncome(deconstructSelections);
            OnCreditStatus(-enhanceReceived * GameConst.m_EquipmentEnhanceCoinsCostMultiply);

            deconstructSelections.Sort((a, b) => b - a);
            m_EquipmentDepotData.m_Equipping.Sort((a, b) => b -a);
            m_EquipmentDepotData.m_Locking.Sort((a, b) => b - a);
            deconstructSelections.Traversal((int deconstructIndex) =>
            {
                m_EquipmentDepotData.m_Equipments.RemoveAt(deconstructIndex);
                m_EquipmentDepotData.m_Equipping.Traversal((int index,int equipIndex) => {
                    if (equipIndex == deconstructIndex)
                        m_EquipmentDepotData.m_Equipping.RemoveAt(index);
                    else if (equipIndex > deconstructIndex)
                        m_EquipmentDepotData.m_Equipping[index]--;
                },true);

                m_EquipmentDepotData.m_Locking.Traversal((int index, int lockIndex) => {
                    if (lockIndex == deconstructIndex)
                        m_EquipmentDepotData.m_Locking.RemoveAt(index);
                    else if (lockIndex > deconstructIndex)
                        m_EquipmentDepotData.m_Locking[index]--;
                }, true);

                if (targetIndex > deconstructIndex)
                    targetIndex--;
            });
            EquipmentSaveData equipmentData = m_EquipmentDepotData.m_Equipments[targetIndex];
            int enhanceStart = equipmentData.GetEnhanceCurLevel() + enhanceReceived;
            for (int level= equipmentData.GetEnhanceLevel(); level<GameConst.m_EquipmentEnhanceMaxLevel;)
            {
                enhanceStart -= GameExpression.GetEquipmentEnhanceRequirement(equipmentData.m_Rarity,level);
                if (enhanceStart < 0)
                    break;
                level++;
                equipmentData.UpgradeMainEntry();

                if (equipmentData.m_Entries.Count >= 4)
                    equipmentData.UpgradeRandomSubEntry();
                else if (level % 5 == 0)
                    equipmentData.AcquireRandomEntry();
            }
            equipmentData.ReceiveEnhance(enhanceReceived);
            m_EquipmentDepotData.m_Equipments[targetIndex] = equipmentData;
            TGameData<CEquipmentDepotData>.Save();
            return targetIndex;
        }
        #endregion

        #region Entry
        static void AcquireRandomEntry(this EquipmentSaveData data)
        {
            enum_CharacterUpgradeType entryType = TCommon.RandomEnumValues<enum_CharacterUpgradeType>();
            float startValue = data.m_Entries.Count < 1 ? GameConst.m_EquipmentEntryStart_Main[entryType][data.m_Rarity] : GameConst.m_EquipmentEntryStart_Sub[entryType][data.m_Rarity].Random();
            data.m_Entries.Add(new EquipmentEntrySaveData(entryType, startValue));
        }

        static void UpgradeMainEntry(this EquipmentSaveData data)
        {
            if (data.m_Entries.Count < 1)
                return;
            EquipmentEntrySaveData mainEntry = data.m_Entries[0];
            data.m_Entries[0] = mainEntry.Upgrade(GameConst.m_EquipmentEntryUpgrade_Main[mainEntry.m_Type][data.m_Rarity]);
        }

        static void UpgradeRandomSubEntry(this EquipmentSaveData data)
        {
            if (data.m_Entries.Count < 2)
                return;
            int randomSubEntry = UnityEngine.Random.Range(1, data.m_Entries.Count);
            EquipmentEntrySaveData subEntry = data.m_Entries[randomSubEntry];
            data.m_Entries[randomSubEntry] = subEntry.Upgrade(GameConst.m_EquipmentEntryUpgrade_Sub[subEntry.m_Type][data.m_Rarity]);
        }

        public static void AcquireEquipment(EquipmentSaveData data)
        {
            m_EquipmentDepotData.m_Equipments.Add(data);
            TGameData<CEquipmentDepotData>.Save();
        }
        #endregion
        #endregion
        #endregion

        #region Upgrade Character Data
        public static float GetUpgradeCost(CharacterUpgradeData data, enum_CharacterUpgradeType type) => GameExpression.GetCharacterUpgradePrice(type,data.m_Upgrades[type]);
        public static bool CanUpgradeItem(CharacterUpgradeData data, enum_CharacterUpgradeType type) => data.m_Upgrades[type] < GameConst.m_MaxCharacterUpgradeTime;

        public static void UpgradeCurrentCharacter(enum_CharacterUpgradeType type)
        {
            CharacterUpgradeData upgradeData = m_CharacterData.CurrentUpgrade;
            if (!CanUpgradeItem(upgradeData, type))
            {
                Debug.LogError("Unable To Upgrade!");
                return;
            }

            float credit = GetUpgradeCost(upgradeData,type);
            if (!CanUseCredit(credit))
            {
                Debug.LogError("Out Of Credits!");
                return;
            }

            OnCreditStatus(-credit);
            m_CharacterData.UpgradeCurrentData(type);
            TGameData<CCharacterUpgradeData>.Save();
        }
        #endregion

        #region PerkData
        static Dictionary<int, ExpirePerkBase> m_AllPerks = new Dictionary<int, ExpirePerkBase>();
        static Dictionary<enum_Rarity, List<int>> m_PerkRarities = new Dictionary<enum_Rarity, List<int>>();
        public static void InitPerks()
        {
            m_AllPerks.Clear();
            m_PerkRarities.Clear();
            TReflection.TraversalAllInheritedClasses(((Type type, ExpirePerkBase perk) => {
                m_AllPerks.Add(perk.m_Index, perk);
                if (perk.m_DataHidden)
                    return;
                if (!m_PerkRarities.ContainsKey(perk.m_Rarity))
                    m_PerkRarities.Add(perk.m_Rarity, new List<int>());
                m_PerkRarities[perk.m_Rarity].Add(perk.m_Index);
            }), PerkSaveData.New(-1));
        }
        public static int RandomPerk(enum_Rarity rarity, Dictionary<int, ExpirePerkBase> playerPerks, System.Random random = null)
        {
            List<int> rarityIDs = m_PerkRarities[rarity].DeepCopy();
            playerPerks.Traversal((ExpirePerkBase perk) => { if (perk.m_Rarity == rarity && perk.m_Stack == perk.m_MaxStack) rarityIDs.Remove(perk.m_Index); });
            return rarityIDs.RandomItem(random);
        }

        public static List<int> RandomPerks(int perkCount, Dictionary<enum_Rarity, int> perkGenerate, Dictionary<int, ExpirePerkBase> playerPerks, System.Random random = null)
        {
            Dictionary<enum_Rarity, List<int>> _perkIDs = m_PerkRarities.DeepCopy();
            Dictionary<enum_Rarity, int> _rarities = perkGenerate.DeepCopy();

            playerPerks.Traversal((ExpirePerkBase perk) => { if (perk.m_Stack == perk.m_MaxStack) _perkIDs[perk.m_Rarity].Remove(perk.m_Index); });

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

        public static ExpirePerkBase GetPerkData(int index) => m_AllPerks[index];

        public static ExpirePerkBase CreatePerk(PerkSaveData data)
        {
            if (!m_AllPerks.ContainsKey(data.m_Index))
                Debug.LogError("Error Perk:" + data.m_Index + " ,Does not exist");
            ExpirePerkBase equipment = TReflection.CreateInstance<ExpirePerkBase>(m_AllPerks[data.m_Index].GetType(), data);
            return equipment;
        }
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

            if (Properties<SWeapon>.PropertiesList.Any(p => TLocalization.GetKeyLocalized(p.m_Weapon.GetLocalizeNameKey()) == weaponIdentity))
                return Properties<SWeapon>.PropertiesList.Find(p => TLocalization.GetKeyLocalized(p.m_Weapon.GetLocalizeNameKey()) == weaponIdentity).m_Weapon;

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

        public static List<int> GetEnermyGenerate(enum_Stage stage,enum_GameDifficulty difficulty)
        {
            return SheetProperties<SEnermyGenerate>.GetPropertiesList(0).RandomItem().m_EnermyGenerate;
        }
        #endregion
    }

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

    public class CPlayerBattleSave : ISave
    {
        public string m_GameSeed;
        public enum_Stage m_Stage;
        public float m_Health;
        public float m_Coins;
        public int m_Keys;
        public int m_TotalExp;
        public enum_PlayerCharacter m_Character;
        public WeaponSaveData m_Weapon1;
        public WeaponSaveData m_Weapon2;
        public List<PerkSaveData> m_Perks;
        public List<EquipmentSaveData> m_Equipments;
        public CharacterUpgradeData m_Upgrade;
        public CPlayerBattleSave() : this(GameDataManager.m_CharacterData.m_CharacterSelected, GameDataManager.m_ArmoryData.m_WeaponSelected, GameDataManager.m_EquipmentDepotData.GetSelectedEquipments(),GameDataManager.m_CharacterData.CurrentUpgrade)
        {
        }

        public CPlayerBattleSave(enum_PlayerCharacter character, enum_PlayerWeapon weapon, List<EquipmentSaveData> equipments,CharacterUpgradeData upgrade)
        {
            m_Coins = 0;
            m_Keys = 0;
            m_TotalExp = 0;
            m_Health = -1;
            m_Character = character;
            m_Equipments = equipments;
            m_Upgrade = upgrade;
            m_Perks = new List<PerkSaveData>();
            m_Weapon1 = WeaponSaveData.New(weapon);
            m_Weapon2 = WeaponSaveData.New(enum_PlayerWeapon.Invalid);
            m_Stage = enum_Stage.Rookie;
            m_GameSeed = DateTime.Now.ToLongTimeString();
        }

        public void Adjust(EntityCharacterPlayer _player, GameProgressManager _level)
        {
            m_GameSeed = _level.m_GameSeed;
            m_Stage = _level.m_GameStage;
            m_Coins = _player.m_CharacterInfo.m_Coins;
            m_Keys = _player.m_CharacterInfo.m_Keys;
            m_TotalExp = _player.m_CharacterInfo.m_RankManager.m_TotalExp;
            m_Health = _player.m_Health.m_CurrentHealth;
            m_Weapon1 = WeaponSaveData.Save(_player.m_Weapon1);
            m_Weapon2 = WeaponSaveData.Save(_player.m_Weapon2);
            m_Perks = PerkSaveData.Create(_player.m_CharacterInfo.m_ExpirePerks.Values.ToList());
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
        public CArmoryData()
        {
            m_WeaponsUnlocked = new List<enum_PlayerWeapon>() { enum_PlayerWeapon.P92, enum_PlayerWeapon.UMP45, enum_PlayerWeapon.Kar98, enum_PlayerWeapon.AKM, enum_PlayerWeapon.S686, enum_PlayerWeapon.Minigun, enum_PlayerWeapon.RocketLauncher, enum_PlayerWeapon.FrostWand };
            m_WeaponBlueprints = new List<enum_PlayerWeapon>() { enum_PlayerWeapon.HeavySword, enum_PlayerWeapon.Flamer };
            m_WeaponSelected = enum_PlayerWeapon.P92;
        }

        public void DataRecorrect()
        {
        }
    }

    public class CEquipmentDepotData : ISave
    {
        public List<EquipmentSaveData> m_Equipments;
        public List<int> m_Equipping;
        public List<int> m_Locking;
        public CEquipmentDepotData()
        {
            m_Equipments = new List<EquipmentSaveData>() { GameDataManager.RandomRarityEquipment( enum_Rarity.Ordinary), GameDataManager.RandomRarityEquipment( enum_Rarity.Ordinary), GameDataManager.RandomRarityEquipment( enum_Rarity.Advanced), GameDataManager.RandomRarityEquipment( enum_Rarity.Rare), GameDataManager.RandomRarityEquipment( enum_Rarity.Epic),
            };
            m_Equipping = new List<int>() { 0, 1, 2 };
            m_Locking = new List<int>() { };
        }

        public List<EquipmentSaveData> GetSelectedEquipments()
        {
            List<EquipmentSaveData> datas = new List<EquipmentSaveData>();
            for (int i = 0; i < m_Equipping.Count; i++)
                datas.Add(m_Equipments[m_Equipping[i]]);
            return datas;
        }

        public void DataRecorrect()
        {
        }
    }

    public class CCharacterUpgradeData:ISave
    {
        public enum_PlayerCharacter m_CharacterSelected;
        public Dictionary<enum_PlayerCharacter, CharacterUpgradeData> m_CharacterUpgrades;

        public CCharacterUpgradeData()
        {
            m_CharacterUpgrades = new Dictionary<enum_PlayerCharacter, CharacterUpgradeData>();
            m_CharacterSelected = enum_PlayerCharacter.Railer;
        }

        public void ChangeSelectedCharacter(enum_PlayerCharacter character) => m_CharacterSelected = character;

        public CharacterUpgradeData CurrentUpgrade => GetUpgradeData(m_CharacterSelected);

        public void UpgradeCurrentData(enum_CharacterUpgradeType type)
        {
            CharacterUpgradeData data = m_CharacterUpgrades[m_CharacterSelected];
            data.m_Upgrades[type] += 1;
            m_CharacterUpgrades[m_CharacterSelected]=data;
        }

        public CharacterUpgradeData GetUpgradeData(enum_PlayerCharacter character)
        {
            if (m_CharacterUpgrades.ContainsKey(character))
                return m_CharacterUpgrades[character];

            CharacterUpgradeData newData = CharacterUpgradeData.Default();
            m_CharacterUpgrades.Add(character, newData);
            return newData;
        }


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
        public static WeaponSaveData Save(WeaponBase weapon) => new WeaponSaveData() { m_Weapon = weapon != null ? weapon.m_WeaponInfo.m_Weapon : enum_PlayerWeapon.Invalid };
        public static WeaponSaveData New(enum_PlayerWeapon weapon) => new WeaponSaveData() { m_Weapon = weapon };
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
        public static PerkSaveData Create(ExpirePerkBase perk) => new PerkSaveData() { m_Index = perk.m_Index, m_PerkStack = perk.m_Stack, m_RecordData = perk.m_RecordData };
        public static List<PerkSaveData> Create(List<ExpirePerkBase> perks)
        {
            List<PerkSaveData> data = new List<PerkSaveData>();
            perks.Traversal((ExpirePerkBase perk) => { data.Add(Create(perk)); });
            return data;
        }
    }

    public struct EquipmentSaveData : IDataConvert
    {
        public int m_Index { get; private set; }
        public int m_Enhance { get; private set; }
        public int m_AcquireStamp { get; private set; }
        public enum_Rarity m_Rarity { get; private set; }
        public List<EquipmentEntrySaveData> m_Entries { get; private set; }
        public EquipmentSaveData ReceiveEnhance(int enhance)
        {
            m_Enhance += enhance;
            return this;
        } 
        public EquipmentSaveData ReceiveEntry(EquipmentEntrySaveData entry)
        {
            m_Entries.Add(entry);
            return this;
        }
        public EquipmentSaveData(int index, int enhance, enum_Rarity rarity, List<EquipmentEntrySaveData> entries) { m_Index = index; m_Enhance = enhance; m_Rarity = rarity; m_AcquireStamp = TTime.TTimeTools.GetTimeStampNow(); m_Entries = entries; }
    }

    public struct EquipmentEntrySaveData : IDataConvert
    {
        public enum_CharacterUpgradeType m_Type { get; private set; }
        public float m_Value { get; private set; }

        public EquipmentEntrySaveData(enum_CharacterUpgradeType entryType, float value) { m_Type = entryType; m_Value = value; }

        public EquipmentEntrySaveData Upgrade(float value)
        {
            m_Value += value;
            return this;
        }
    }

    public struct CharacterUpgradeData:IDataConvert
    {
        public Dictionary<enum_CharacterUpgradeType,int> m_Upgrades { get; private set; }
        public static CharacterUpgradeData Default() => new CharacterUpgradeData() { m_Upgrades = new Dictionary<enum_CharacterUpgradeType, int>() { { enum_CharacterUpgradeType.Armor,0}, { enum_CharacterUpgradeType.CriticalRate, 0 }, { enum_CharacterUpgradeType.Health, 0 }, { enum_CharacterUpgradeType.FireRate, 0 }, { enum_CharacterUpgradeType.MovementSpeed, 0 }, { enum_CharacterUpgradeType.Damage, 0 } } };
        public CharacterUpgradeData(Dictionary<enum_CharacterUpgradeType,int> upgrades) { m_Upgrades = upgrades; }
    }
    #endregion

    #region ExcelData
    public struct SWeapon : ISExcel
    {
        public int m_Index { get; private set; }
        public bool m_Hidden { get; private set; }
        public enum_Rarity m_Rarity { get; private set; }
        public float m_FireRate { get; private set; }
        public int m_ClipAmount { get; private set; }
        public float m_Spread { get; private set; }
        public float m_BulletRefillTime { get; private set; }
        public int m_PelletsPerShot { get; private set; }
        public float m_Weight { get; private set; }
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
        public static SBuff CreateGameEnermyBuff(int difficulty, float damageMultiply)
        {
            SBuff buff = new SBuff();
            buff.m_Index = 2000 + difficulty;
            buff.m_Refresh = (int)enum_ExpireRefreshType.AddUp;
            buff.m_DamageMultiply = damageMultiply;
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
}