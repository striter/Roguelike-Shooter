﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

namespace GameSetting
{

    public enum enum_UIWeaponTag
    {
        Invalid=-1,
        Projectile,
        MultiShot,
        StoreShot,

        Cast,
        Melee,
        Duration,
    }

    public static class UIConst
    {
        public const int I_PlayerDyingMinValue = 10;
        public const int I_PlayerDyingMaxValue = 50;

        public const int I_GameProgressDifficultyColorRampMaxMinutes = 30;

        public static Vector2 V2_UINumericVisualizeOffset = new Vector2(0, 100f);

        public const int I_DetailWeaponTagMax = 5;
    }

    public static class UIExpression
    {
        public static Color GetUIDamageColor(bool criticalHit) => TCommon.GetHexColor(criticalHit ? "F2F025FF" : "F22525FF");
        public static Color TipsColor(this enum_UITipsType type)
        {
            switch (type)
            {
                case enum_UITipsType.Normal:
                    return Color.green;
                case enum_UITipsType.Warning:
                    return Color.yellow;
                case enum_UITipsType.Error:
                    return Color.white;
                default:
                    return Color.magenta;
            }
        }

        public static int GetUIWeaponScore(WeaponBase weapon) => (int)weapon.m_WeaponInfo.m_Rarity + weapon.m_EnhanceLevel;

        public static float GetUIWeaponDamageValue(float uiDamage) => Mathf.InverseLerp(0, 100, uiDamage);   //武器数据查看ui标准
        public static float GetUIWeaponRPMValue(float uiRPM) => Mathf.InverseLerp(0, 400, uiRPM);
        public static float GetUIWeaponStabilityValue(float uiStability) => Mathf.InverseLerp(0, 100, uiStability);
        public static float GetUIWeaponSpeedValue(float uiSpeed) => Mathf.InverseLerp(0, 100, uiSpeed);
        public static float GetUIDamageScale(float damage) => ((damage / 50 / 10) + .9f) / 2;  //伤害显示比例缩放，默认是两倍大小
        public static Vector2 GetUIDamagePositionOffset() => new Vector2(TCommon.RandomUnitValue() * 40f, TCommon.RandomLength(1f) * 30f);
    }

    public static class UIConvertions
    {
        public static enum_UIWeaponTag[] GetWeaponTags(this enum_PlayerWeaponType type)
        {
            switch (type)
            {
                default:Debug.LogError("Invalid Convertions Here!");return new enum_UIWeaponTag[0];
                case enum_PlayerWeaponType.CastDuration: return new enum_UIWeaponTag[2] {  enum_UIWeaponTag.Cast, enum_UIWeaponTag.Duration};
                case enum_PlayerWeaponType.CastMelee: return new enum_UIWeaponTag[2] {  enum_UIWeaponTag.Cast, enum_UIWeaponTag.Melee};
                case enum_PlayerWeaponType.ProjectileShot:return new enum_UIWeaponTag[1] { enum_UIWeaponTag.Projectile };
                case enum_PlayerWeaponType.ProjectileShotMulti: return new enum_UIWeaponTag[2] {  enum_UIWeaponTag.Projectile, enum_UIWeaponTag.MultiShot};
                case enum_PlayerWeaponType.ProjectileStore:return new enum_UIWeaponTag[2] { enum_UIWeaponTag.Projectile, enum_UIWeaponTag.StoreShot };
            }

        }

        public static Color GetVisualizeAmountColor(this enum_Interaction type)
        {
            switch (type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return Color.magenta;
                case enum_Interaction.PickupKey:
                    return Color.green;
                case enum_Interaction.PickupCoin:
                    return TCommon.GetHexColor("FFCC1FFF");
                case enum_Interaction.PickupArmor:
                    return TCommon.GetHexColor("1FF2FFFF");
                case enum_Interaction.PickupHealth:
                case enum_Interaction.PickupHealthPack:
                    return TCommon.GetHexColor("FFA54EFF");
            }
        }


        public static string GetUIColor(this enum_Rarity rarity)
        {
            switch (rarity)
            {
                default: return "FFFFFFFF";
                case enum_Rarity.Ordinary: return "E3E3E3FF";
                case enum_Rarity.Advanced: return "6F8AFFFF";
                case enum_Rarity.Rare: return "C26FFFFF";
                case enum_Rarity.Epic: return "FFCC1FFF";
            }
        }

        public static string GetUIGameResultTitleBG(bool win, enum_Option_LanguageRegion language) => "result_title_" + (win ? "win_" : "fail_") + language;
    }

    public static class SpriteKeyJoint
    {
        public static string GetInteractMainIcon(this InteractBase interact) => "control_main_interact";
        public static string GetInteractIcon(this enum_Interaction type) => "Interact_Icon_" + type;
        public static string GetAbilitySprite(this enum_PlayerCharacter character) => "control_ability_" + character;
        public static string GetAbilityBackground(bool cooldowning) => cooldowning ? "control_ability_bottom_cooldown" : "control_ability_bottom_activate";
        public static string GetIconSprite(this enum_PlayerWeapon weapon) => "icon_" + ((int)weapon);
        public static string GetDetailSprite(this enum_PlayerWeapon weapon) => "detail_" + ((int)weapon);
        public static string GetExpireSprite(this EntityExpireBase expire) => expire.m_Index.ToString();

        public static string GetUIInteractBackground(this enum_Rarity rarity) => "interact_" + rarity;
        public static string GetUIDetailBackground(this enum_Rarity rarity) => "detail_weapon_background_" + rarity;
        public static string GetUIGameControlBackground(this enum_Rarity rarity) => "control_" + rarity;

        public static string GetInteractMapIcon(this InteractGameBase interact)
        {
            switch (interact.m_InteractType)
            {
                default: return "Map_Icon_Unknown";
                case enum_Interaction.SignalTower:
                    return "Map_Icon_" + interact.m_InteractType;
            }
        }

        public static string GetNumericVisualizeIcon(this enum_Interaction type)
        {
            switch (type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return "";
                case enum_Interaction.PickupCoin:
                    return "NumericIcon_Coin";
                case enum_Interaction.PickupKey:
                    return "NumericIcon_Key";
                case enum_Interaction.PickupArmor:
                    return "NumericIcon_Armor";
                case enum_Interaction.PickupHealth:
                case enum_Interaction.PickupHealthPack:
                    return "NumericIcon_Health";
            }
        }

    }

    public static class LocalizationKeyJoint
    {
        public static string GetNameLocalizeKey(this EntityExpirePreset buff) => "Buff_Name_" + buff.m_Index;
        public static string GetNameLocalizeKey(this ExpirePlayerPerkBase perk) => "Perk_Name_" + perk.m_Index;
        public static string GetDetailLocalizeKey(this ExpirePlayerPerkBase perk) => "Perk_Detail_" + perk.m_Index;
        public static string GetIntroLocalizeKey(this ExpirePlayerPerkBase perk) => "Perk_Intro_" + perk.m_Index;
        public static string GetNameLocalizeKey(this enum_PlayerCharacter character) => "Character_Name_" + character;
        public static string GetIntroLocalizeKey(this enum_PlayerCharacter character) => "Character_Intro_" + character;
        public static string GetAbilityNameLocalizeKey(this enum_PlayerCharacter character) => "Character_Ability_Name_" + character;
        public static string GetAbilityDetailLocalizeKey(this enum_PlayerCharacter character) => "Character_Ability_Detail_" + character;
        public static string GetNameLocalizeKey(this EquipmentSaveData equipment) => "Equipment_Name_" + equipment.m_Index;
        public static string GetPassiveLocalizeKey(this EquipmentSaveData upgrade) => "Equipment_Passive_" + upgrade.m_Index;
        public static string GetPassiveLocalizeKey(this ExpirePlayerUpgradeCombine upgrade) => "Equipment_Passive_" + upgrade.m_Index;
        public static string GetLocalizeKey(this EquipmentEntrySaveData entry) => "Equipment_Entry_" + entry.m_Type;
        public static string GetLocalizeKey(this enum_GameStage stage) => "Game_Stage_" + stage;
        public static string GetLocalizeKey(this enum_GameStyle style) => "Game_Style_" + style;
        public static string GetLocalizeNameKey(this enum_GamePortalType type) => "UI_Level_" + type + "_Name";
        public static string GetLocalizeIntroKey(this enum_GamePortalType type) => "UI_Level_" + type + "_Intro";
        public static string GetNameLocalizeKey(this enum_PlayerWeapon weapon) => "Weapon_Name_" + weapon;
        public static string GetIntroLocalizeKey(this enum_PlayerWeapon weapon) => "Weapon_Intro_" + weapon;
        public static string GetTitleLocalizeKey(this enum_Interaction interact) => "UI_Interact_" + interact + "_Title";
        public static string GetIntroLocalizeKey(this enum_Interaction interact) => "UI_Interact_" + interact + "_Intro";
        public static string GetLocalizeKey(this enum_Option_FrameRate frameRate) => "UI_Option_" + frameRate;
        public static string GetLocalizeKey(this enum_Option_JoyStickMode joystick) => "UI_Option_" + joystick;
        public static string GetLocalizeKey(this enum_Option_LanguageRegion region) => "UI_Option_" + region;
        public static string SetActionIntro(this ExpirePlayerPerkBase actionInfo, UIT_TextExtend text) => text.formatText(actionInfo.GetIntroLocalizeKey(), actionInfo.Value1, actionInfo.Value2, actionInfo.Value3);
        public static string GetLocalizeKey(this enum_UIWeaponTag tag) => "UI_Weapon_Tag_" + tag;
    }
    public class UIC_RarityLevel
    {
        class RarityLevel
        {
            public Image m_HighLight { get; private set; }
            public Image m_BackGround { get; private set; }
            public RarityLevel(Transform trans)
            {
                m_HighLight = trans.Find("HighLight").GetComponent<Image>();
                m_BackGround = trans.Find("Background").GetComponent<Image>();
            }
            public void SetHighlight(bool show)
            {
                m_HighLight.SetActivate(show);
                m_BackGround.SetActivate(!show);
            }
        }
        public Transform transform { get; private set; }
        ObjectPoolListComponent<int, Transform> m_Grid;
        Dictionary<int, RarityLevel> m_Levels = new Dictionary<int, RarityLevel>();
        public UIC_RarityLevel(Transform _transform)
        {
            transform = _transform;
            m_Grid = new ObjectPoolListComponent<int, Transform>(transform, "GridItem");
            m_Grid.Clear();
            TCommon.TraversalEnum((enum_Rarity rarity) => { m_Levels.Add((int)rarity, new RarityLevel(m_Grid.AddItem((int)rarity))); });
        }
        public void SetRarity(enum_Rarity level)
        {
            m_Levels.Traversal((int index, RarityLevel rarity) => rarity.SetHighlight(index <= (int)level));
        }
    }

    public class UIC_Button
    {
        Button m_Button;
        Transform m_Show;
        Transform m_Hide;
        public UIC_Button(Transform _transform, UnityEngine.Events.UnityAction OnButtonClick)
        {
            m_Button = _transform.GetComponent<Button>();
            m_Button.onClick.AddListener(OnButtonClick);
            m_Show = _transform.Find("Show");
            m_Hide = _transform.Find("Hide");
            SetInteractable(true);
        }
        public void SetInteractable(bool interactable)
        {
            m_Hide.SetActivate(!interactable);
            m_Show.SetActivate(interactable);
            m_Button.interactable = interactable;
        }
    }
    public class UIC_EquipmentData
    {
        public Transform transform { get; private set; }

        Image m_Image;
        UIC_RarityLevel m_Rarity;
        public UIC_EquipmentData(Transform _transform)
        {
            transform = _transform;
            m_Image = transform.Find("Mask/Image").GetComponent<Image>();
            m_Rarity = new UIC_RarityLevel(transform.Find("Rarity"));
        }
        public virtual void SetInfo(ExpirePlayerPerkBase equipmentInfo)
        {
            m_Image.sprite = GameUIManager.Instance.m_ExpireSprites[equipmentInfo.m_Index.ToString()];
            m_Rarity.SetRarity(equipmentInfo.m_Rarity);
        }
    }

    public class UIC_EquipmentNameData : UIC_EquipmentData
    {
        UIT_TextExtend m_Name;
        public UIC_EquipmentNameData(Transform _transform) : base(_transform)
        {
            m_Name = transform.Find("Name").GetComponent<UIT_TextExtend>();
        }
        public override void SetInfo(ExpirePlayerPerkBase equipmentInfo)
        {
            base.SetInfo(equipmentInfo);
            m_Name.localizeKey = equipmentInfo.GetNameLocalizeKey();
        }
    }

    public class UIC_EquipmentNameFormatIntro : UIC_EquipmentNameData
    {
        UIT_TextExtend m_Intro;

        public UIC_EquipmentNameFormatIntro(Transform _transform) : base(_transform)
        {

            m_Intro = transform.Find("Intro").GetComponent<UIT_TextExtend>();
        }
        public override void SetInfo(ExpirePlayerPerkBase equipmentInfo)
        {
            base.SetInfo(equipmentInfo);
            m_Intro.formatText(equipmentInfo.GetIntroLocalizeKey(), string.Format("<color=#FFDA6BFF>{0}</color>", equipmentInfo.Value1), string.Format("<color=#FFDA6BFF>{0}</color>", equipmentInfo.Value2), string.Format("<color=#FFDA6BFF>{0}</color>", equipmentInfo.Value3));
        }
    }

    public class UIC_WeaponData
    {
        public Transform transform { get; private set; }
        UIT_TextExtend m_Name;
        Image m_Background;
        Image m_Image;
        Transform tf_AmmoStatus;
        Text m_Clip, m_Total;
        public UIC_WeaponData(Transform _transform)
        {
            transform = _transform;
            m_Background = transform.Find("Background").GetComponent<Image>();
            m_Image = transform.Find("Image").GetComponent<Image>();
            m_Name = transform.Find("NameStatus/Name").GetComponent<UIT_TextExtend>();
            tf_AmmoStatus = transform.Find("NameStatus/AmmoStatus");
            m_Clip = tf_AmmoStatus.Find("Clip").GetComponent<Text>();
            m_Total = tf_AmmoStatus.Find("Total").GetComponent<Text>();
        }

        public void UpdateInfo(WeaponBase weapon)
        {
            m_Background.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Rarity.GetUIGameControlBackground()];
            m_Image.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Weapon.GetDetailSprite()];
            m_Name.localizeKey = weapon.m_WeaponInfo.m_Weapon.GetNameLocalizeKey();
            m_Name.color = TCommon.GetHexColor(weapon.m_WeaponInfo.m_Rarity.GetUIColor());
        }
        public void UpdateAmmoInfo(int ammoLeft, int clipAmount)
        {
            m_Clip.text = ammoLeft.ToString();
            m_Total.text = clipAmount.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(tf_AmmoStatus as RectTransform);
        }
    }

    class UIC_MapBase
    {
        protected RectTransform rectTransform { get; private set; }
        protected RectTransform m_Map_Origin { get; private set; }
        protected RawImage m_Map_Origin_Base { get; private set; }
        protected RawImage m_Map_Origin_Base_Fog { get; private set; }
        protected UIGI_MapEntityLocation m_Player { get; private set; }
        protected float m_MapScale { get; private set; }
        protected float m_MapAngle { get; private set; }
        public UIC_MapBase(Transform transform, float mapScale)
        {
            rectTransform = transform as RectTransform;
            m_Map_Origin = transform.Find("Origin") as RectTransform;
            m_Map_Origin_Base = m_Map_Origin.Find("Base").GetComponent<RawImage>();
            m_Map_Origin_Base_Fog = m_Map_Origin_Base.transform.Find("Fog").GetComponent<RawImage>();
            m_Player = m_Map_Origin_Base.transform.Find("Player").GetComponent<UIGI_MapEntityLocation>();
            m_Player.OnInitItem();
            m_Player.Play(GameManager.Instance.m_LocalPlayer);
            ChangeMapScale(mapScale);
        }

        public virtual void DoMapInit()
        {
            m_Player.Play(GameManager.Instance.m_LocalPlayer);
            m_Map_Origin_Base.texture = GameLevelManager.Instance.m_MapTexture;
            m_Map_Origin_Base.SetNativeSize();
            m_Map_Origin_Base_Fog.texture = GameLevelManager.Instance.m_FogTexture;
        }
        protected void UpdateMapRotation(float mapAngle)
        {
            m_MapAngle = mapAngle;
            m_Map_Origin.localRotation = Quaternion.Euler(0, 0, m_MapAngle);
            m_Player.Tick();
        }

        protected void ChangeMapScale(float mapScale)
        {
            m_MapScale = mapScale;
            m_Map_Origin_Base.rectTransform.localScale = Vector3.one * m_MapScale;
        }
    }
}
