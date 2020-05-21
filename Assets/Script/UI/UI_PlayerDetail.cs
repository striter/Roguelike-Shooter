using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerDetail : UIPage
{
    Transform m_AbilityInfo;
    Image m_AbilityImage;
    UIT_TextExtend m_AbilityTitle;
    UIT_TextExtend m_AbilityIntro;

    UIT_GridControlledSingleSelect<UIGI_DetailWeaponSelect> m_WeaponSelect;
    UIT_GridControlledSingleSelect<UIGI_DetailPerkSelect> m_PerkSelect;

    UIC_WeaponInfo m_WeaponDetail;


    Transform m_PerkInfo;
    UIT_TextExtend m_PerkDetail;
    UIT_TextExtend m_PerkName;
    UIT_TextExtend m_PerkIntro;
    Image m_PerkImage;


    protected override void Init()
    {
        base.Init();

        m_AbilityInfo = rtf_Container.Find("AbilityInfo");
        m_AbilityImage = m_AbilityInfo.Find("Image").GetComponent<Image>();
        m_AbilityTitle = m_AbilityInfo.Find("Title").GetComponent<UIT_TextExtend>();
        m_AbilityIntro = m_AbilityInfo.Find("Intro").GetComponent<UIT_TextExtend>();

        m_WeaponSelect = new UIT_GridControlledSingleSelect<UIGI_DetailWeaponSelect>(rtf_Container.Find("WeaponSelect/Grid"), OnWeaponSelectClick);
        m_PerkSelect = new UIT_GridControlledSingleSelect<UIGI_DetailPerkSelect>(rtf_Container.Find("PerkSelect/ScrollRect/Viewport/Content"), OnPerkSelectClick);

        m_WeaponDetail = new UIC_WeaponInfo(rtf_Container.Find("WeaponInfo"));

        m_PerkInfo = rtf_Container.Find("PerkInfo");
        m_PerkImage = m_PerkInfo.Find("Image").GetComponent<Image>();
        m_PerkName = m_PerkInfo.Find("Name").GetComponent<UIT_TextExtend>();
        m_PerkIntro = m_PerkInfo.Find("Intro").GetComponent<UIT_TextExtend>();
        m_PerkDetail = m_PerkInfo.Find("Detail").GetComponent<UIT_TextExtend>();
    }

    EntityCharacterPlayer m_Player;

    public override void OnPlay(bool doAnim, Action<UIPageBase> OnPageExit)
    {
        base.OnPlay(doAnim, OnPageExit);
        m_Player = GameManager.Instance.m_LocalPlayer;
        m_WeaponSelect.ClearGrid();
        if (m_Player.m_Weapon1)
            m_WeaponSelect.AddItem(0).Init(m_Player.m_Weapon1.m_WeaponInfo.m_Weapon);
        if (m_Player.m_Weapon2)
            m_WeaponSelect.AddItem(1).Init(m_Player.m_Weapon2.m_WeaponInfo.m_Weapon);

        m_PerkSelect.ClearGrid();
        m_Player.m_CharacterInfo.m_ExpirePerks.Traversal((int index, ExpirePlayerPerkBase perk) => { m_PerkSelect.AddItem(index).Init(perk); });

        SetAbilityInfo(m_Player);
        m_WeaponSelect.OnItemClick(0);
    }

    void OnWeaponSelectClick(int index)
    {
        m_PerkSelect.ClearHighlight();
        WeaponBase weapon = index == 0 ? m_Player.m_Weapon1 : m_Player.m_Weapon2;
        m_WeaponDetail.SetWeaponInfo(weapon.m_WeaponInfo,true,weapon.m_EnhanceLevel);
        m_WeaponDetail.transform.SetActivate(true);
        m_PerkInfo.SetActivate(false);
    }

    void OnPerkSelectClick(int index)
    {
        m_WeaponSelect.ClearHighlight();
        SetPerkInfo(m_Player.m_CharacterInfo.m_ExpirePerks[index]);
        m_WeaponDetail.transform.SetActivate(false);
        m_PerkInfo.SetActivate(true);
    }

    void SetAbilityInfo(EntityCharacterPlayer player)
    {
        m_AbilityIntro.formatText(player.m_Character.GetAbilityDetailLocalizeKey(), string.Format("<color=#FE9E00FF>{0}</color>", "?"), string.Format("<color=#FE9E00FF>{0}</color>", "¿"));
        m_AbilityTitle.text = string.Format("{0} <color=#FE9E00FF>LV{1}</color>", player.m_Character.GetNameLocalizeKey().GetKeyLocalized(),player.m_CharacterInfo.m_RankManager.m_Rank);
        m_AbilityImage.sprite = UIManager.Instance.m_CommonSprites[player.m_Character.GetAbilitySprite()];
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
