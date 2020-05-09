using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerDetail : UIPage
{
    EntityCharacterPlayer m_Player;

    Transform m_AbilityInfo;
    Image m_AbilityImage;
    UIT_TextExtend m_AbilityTitle;
    UIT_TextExtend m_AbilityIntro;

    UIT_GridControlledSingleSelect<UIGI_DetailWeaponSelect> m_WeaponSelect;
    UIT_GridControlledSingleSelect<UIGI_DetailPerkSelect> m_PerkSelect;

    Transform m_WeaponInfo;
    UIT_TextExtend m_WeaponName;
    UIT_TextExtend m_WeaponIntro;
    Image m_WeaponImage;
    Image m_WeaponBackground;
    UIT_TextExtend m_ClipSize;
    Transform m_WeaponScoreSliders;
    UIT_GridControllerClass<UIGC_WeaponScoreItem> m_WeaponScore;
    UIT_GridControllerClass<UIGC_WeaponTagItem> m_WeaponTag;
    
    Image m_Damage, m_FireRate, m_Stability, m_ProjectileSpeed;
    UIT_TextExtend m_DamageAmount, m_FireRateAmount, m_StabilityAmount, m_ProjectileSpeedAmount;

    Transform m_PerkInfo;
    UIT_TextExtend m_PerkDetail;
    UIT_TextExtend m_PerkName;
    UIT_TextExtend m_PerkIntro;
    Image m_PerkImage;

    protected override void Init()
    {
        base.Init();
        m_Player = GameManager.Instance.m_LocalPlayer;

        m_AbilityInfo = rtf_Container.Find("AbilityInfo");
        m_AbilityImage = m_AbilityInfo.Find("Image").GetComponent<Image>();
        m_AbilityTitle = m_AbilityInfo.Find("Title").GetComponent<UIT_TextExtend>();
        m_AbilityIntro = m_AbilityInfo.Find("Intro").GetComponent<UIT_TextExtend>();

        m_WeaponSelect = new UIT_GridControlledSingleSelect<UIGI_DetailWeaponSelect>(rtf_Container.Find("WeaponSelect/Grid"), OnWeaponSelectClick);
        if (m_Player.m_Weapon1)
            m_WeaponSelect.AddItem(0).Init(m_Player.m_Weapon1.m_WeaponInfo.m_Weapon);
        if (m_Player.m_Weapon2)
            m_WeaponSelect.AddItem(1).Init(m_Player.m_Weapon2.m_WeaponInfo.m_Weapon);

        m_PerkSelect = new UIT_GridControlledSingleSelect<UIGI_DetailPerkSelect>(rtf_Container.Find("PerkSelect/ScrollRect/Viewport/Content"), OnPerkSelectClick);
        m_Player.m_CharacterInfo.m_ExpirePerks.Traversal((int index, ExpirePlayerPerkBase perk) => { m_PerkSelect.AddItem(index).Init(perk); });

        m_WeaponInfo = rtf_Container.Find("WeaponInfo");
        m_WeaponBackground = m_WeaponInfo.Find("ImageBG").GetComponent<Image>();
        m_WeaponName = m_WeaponInfo.Find("Name").GetComponent<UIT_TextExtend>();
        m_WeaponIntro = m_WeaponInfo.Find("Intro").GetComponent<UIT_TextExtend>();
        m_WeaponImage = m_WeaponInfo.Find("Image").GetComponent<Image>();
        m_ClipSize = m_WeaponInfo.Find("ClipSize").GetComponent<UIT_TextExtend>();

        m_WeaponScore = new UIT_GridControllerClass<UIGC_WeaponScoreItem>(m_WeaponInfo.Find("ScoreGrid"));
        m_WeaponTag = new UIT_GridControllerClass<UIGC_WeaponTagItem>(m_WeaponInfo.Find("TagGrid"));
        for (int i = 0; i < UIConst.I_DetailWeaponTagMax; i++)
            m_WeaponTag.AddItem(i).SetTag(enum_UIWeaponTag.Invalid);

        m_WeaponScoreSliders = m_WeaponInfo.Find("ScoreSliders");
        m_Damage = m_WeaponScoreSliders.Find("Damage/Fill").GetComponent<Image>();
        m_DamageAmount = m_WeaponScoreSliders.Find("Damage/Amount").GetComponent<UIT_TextExtend>();
        m_FireRate = m_WeaponScoreSliders.Find("FireRate/Fill").GetComponent<Image>();
        m_FireRateAmount = m_WeaponScoreSliders.Find("FireRate/Amount").GetComponent<UIT_TextExtend>();
        m_Stability = m_WeaponScoreSliders.Find("Stability/Fill").GetComponent<Image>();
        m_StabilityAmount = m_WeaponScoreSliders.Find("Stability/Amount").GetComponent<UIT_TextExtend>();
        m_ProjectileSpeed = m_WeaponScoreSliders.Find("ProjectileSpeed/Fill").GetComponent<Image>();
        m_ProjectileSpeedAmount = m_WeaponScoreSliders.Find("ProjectileSpeed/Amount").GetComponent<UIT_TextExtend>();

        m_PerkInfo = rtf_Container.Find("PerkInfo");
        m_PerkImage = m_PerkInfo.Find("Image").GetComponent<Image>();
        m_PerkName = m_PerkInfo.Find("Name").GetComponent<UIT_TextExtend>();
        m_PerkIntro = m_PerkInfo.Find("Intro").GetComponent<UIT_TextExtend>();
        m_PerkDetail = m_PerkInfo.Find("Detail").GetComponent<UIT_TextExtend>();


        SetAbilityInfo(m_Player);
        m_WeaponSelect.OnItemClick(0);
    }

    void OnWeaponSelectClick(int index)
    {
        m_PerkSelect.ClearHighlight();
        SetWeaponInfo(index == 0 ? m_Player.m_Weapon1 : m_Player.m_Weapon2);
        m_WeaponInfo.SetActivate(true);
        m_PerkInfo.SetActivate(false);
    }

    void OnPerkSelectClick(int index)
    {
        m_WeaponSelect.ClearHighlight();
        SetPerkInfo(m_Player.m_CharacterInfo.m_ExpirePerks[index]);
        m_WeaponInfo.SetActivate(false);
        m_PerkInfo.SetActivate(true);
    }

    void SetAbilityInfo(EntityCharacterPlayer player)
    {
        m_AbilityIntro.formatText(player.m_Character.GetAbilityDetailLocalizeKey(), string.Format("<color=#FE9E00FF>{0}</color>", "?"), string.Format("<color=#FE9E00FF>{0}</color>", "¿"));
        m_AbilityTitle.text = string.Format("{0} <color=#FE9E00FF>LV{1}</color>", player.m_Character.GetNameLocalizeKey().GetKeyLocalized(),player.m_CharacterInfo.m_RankManager.m_Rank);
        m_AbilityImage.sprite = UIManager.Instance.m_CommonSprites[player.m_Character.GetAbilitySprite()];
    }

    void SetWeaponInfo(WeaponBase weapon)
    {
        SWeapon weaponInfo = weapon.m_WeaponInfo;
        m_WeaponImage.sprite = UIManager.Instance.m_WeaponSprites[weaponInfo.m_Weapon.GetDetailSprite()];
        m_WeaponName.localizeKey = weaponInfo.m_Weapon.GetNameLocalizeKey();
        m_WeaponName.color = TCommon.GetHexColor(weaponInfo.m_Rarity.GetUIColor());
        m_WeaponIntro.localizeKey = weaponInfo.m_Weapon.GetIntroLocalizeKey();
        m_WeaponBackground.sprite = GameUIManager.Instance.m_InGameSprites[weaponInfo.m_Rarity.GetUIDetailBackground()];
        m_ClipSize.text = weaponInfo.m_ClipAmount.ToString();

        m_WeaponScore.ClearGrid();
        int baseScore = (int)weapon.m_WeaponInfo.m_Rarity;
        int enhanceScore = weapon.m_EnhanceLevel;
        for (int i = 0; i < enhanceScore; i++)
            m_WeaponScore.AddItem().SetScore(true);
        for (int i = 0; i < baseScore; i++)
            m_WeaponScore.AddItem().SetScore(false);
        m_WeaponScore.Sort((a, b) => a.Key - b.Key);

        enum_UIWeaponTag[] tags = weapon.m_WeaponType.GetWeaponTags();
        for (int i = 0; i < UIConst.I_DetailWeaponTagMax; i++)
            m_WeaponTag.GetItem(UIConst.I_DetailWeaponTagMax -1- i).SetTag(i < tags.Length ? tags[i] : enum_UIWeaponTag.Invalid);

        m_DamageAmount.text = string.Format("{0:N1}", weaponInfo.m_UIDamage);
        m_Damage.fillAmount = UIExpression.GetUIWeaponDamageValue(weaponInfo.m_UIDamage);
        m_StabilityAmount.text = string.Format("{0:N1}", weaponInfo.m_UIStability);
        m_Stability.fillAmount = UIExpression.GetUIWeaponStabilityValue(weaponInfo.m_UIStability);
        m_FireRateAmount.text = string.Format("{0:N1}", weaponInfo.m_UIRPM);
        m_FireRate.fillAmount = UIExpression.GetUIWeaponRPMValue(weaponInfo.m_UIRPM);
        m_ProjectileSpeedAmount.text = string.Format("{0:N1}", weaponInfo.m_UISpeed);
        m_ProjectileSpeed.fillAmount = UIExpression.GetUIWeaponSpeedValue(weaponInfo.m_UISpeed);
    }

    void SetPerkInfo(ExpirePlayerPerkBase perk)
    {
        m_PerkImage.sprite = UIManager.Instance.m_ExpireSprites[perk.GetExpireSprite()];
        m_PerkName.localizeKey = perk.GetNameLocalizeKey();
        m_PerkName.color = TCommon.GetHexColor(perk.m_Rarity.GetUIColor());
        m_PerkDetail.formatText(perk.GetDetailLocalizeKey(), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value1), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value2), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value3));
        m_PerkIntro.localizeKey = perk.GetIntroLocalizeKey();
    }
}
