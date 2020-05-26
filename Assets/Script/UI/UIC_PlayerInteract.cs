using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSpecialClasses;
using UnityEngine.UI;
using System;

public class UIC_PlayerInteract : UIControlBase
{
    InteractBase m_Interact;
    RectTransform m_InteractData;
    Transform m_Container;

    Transform m_WeaponData;
    UIT_TextExtend m_WeaponName;
    Image m_WeaponImage;
    Text m_ClipSize;
    UIT_GridControllerClass<UIGC_WeaponScoreItem> m_WeaponScore;

    Transform m_PerkData;
    UIT_TextExtend m_PerkName;
    UIT_TextExtend m_PerkDetail;
    Image m_PerkImage;

    Transform m_CommonData;
    UIT_TextExtend m_CommonName;
    UIT_TextExtend m_CommonIntro;
    Image m_CommonImage;

    Transform tf_TradeBottom;
    UIT_TextExtend m_TradePrice;


    protected override void Init()
    {
        base.Init();
        m_InteractData = transform.Find("InteractData").GetComponent<RectTransform>();
        m_Container = m_InteractData.Find("Container");

        m_WeaponData = m_Container.Find("WeaponData");
        m_WeaponName = m_WeaponData.Find("Name").GetComponent<UIT_TextExtend>();
        m_WeaponImage = m_WeaponData.Find("Image").GetComponent<Image>();
        m_ClipSize = m_WeaponData.Find("ClipSize").GetComponent<Text>();
        m_WeaponScore = new UIT_GridControllerClass<UIGC_WeaponScoreItem>(m_WeaponData.Find("Score"));

        m_PerkData = m_Container.Find("PerkData");
        m_PerkName = m_PerkData.Find("Name").GetComponent<UIT_TextExtend>();
        m_PerkDetail = m_PerkData.Find("Detail").GetComponent<UIT_TextExtend>();
        m_PerkImage = m_PerkData.Find("Image").GetComponent<Image>();

        tf_TradeBottom = m_Container.Find("TradeBottom");
        m_TradePrice = tf_TradeBottom.Find("Price").GetComponent<UIT_TextExtend>();

        m_CommonData = m_Container.Find("CommonData");
        m_CommonName = m_CommonData.Find("Name").GetComponent<UIT_TextExtend>();
        m_CommonIntro = m_CommonData.Find("Intro").GetComponent<UIT_TextExtend>();
        m_CommonImage = m_CommonData.Find("Image").GetComponent<Image>();

        m_InteractData.SetActivate(false);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerInteractUpdate,OnInteractStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerInteractUpdate, OnInteractStatus);
    }
    
    void OnInteractStatus(EntityCharacterPlayer _player)
    {
        m_Interact = _player.m_Interact;

        InteractBase targetItem = null;
        int tradePrice = -1;
        if (m_Interact != null)
        {
            tradePrice = m_Interact.m_InteractType.IsBattleInteract()?(m_Interact as InteractBattleBase) .m_TradePrice:0;
            switch (m_Interact.m_InteractType)
            {
                default:
                    targetItem = m_Interact;
                    break;
                case enum_Interaction.TradeContainer:
                    InteractTradeContainer trade = m_Interact as InteractTradeContainer;
                    targetItem = trade.m_TradeInteract;
                    break;
            }
        }
        if(UpdateInfo(targetItem,tradePrice))
        m_InteractData.SetWorldViewPortAnchor(m_Interact.transform.position, CameraController.Instance.m_Camera);
        
    }
    bool UpdateInfo(InteractBase targetInteract,int price)
    {
        if (targetInteract == null)
        {
            m_InteractData.SetActivate(false);
            return false;
        }
        m_InteractData.SetActivate(true);
        bool isCommon = false;
        bool isWeapon = false;
        bool isPerk = false;
        if (targetInteract != null)
        {
            switch (targetInteract.m_InteractType)
            {
                case enum_Interaction.PerkPickup:
                    isPerk = true;
                    SetPerkInfo(GameDataManager.GetPlayerPerkData((targetInteract as InteractPerkPickup).m_PerkID));
                    break;
                case enum_Interaction.PickupWeapon:
                    isWeapon = true;
                    InteractPickupWeapon weaponInteract = targetInteract as InteractPickupWeapon;
                    SetWeaponInfo(weaponInteract.m_Weapon);
                    break;
                default:
                    isCommon = true;
                    SetInteractData(targetInteract);
                    break;
            }
        }
        m_CommonData.SetActivate(isCommon);
        m_WeaponData.SetActivate(isWeapon);
        m_PerkData.transform.SetActivate(isPerk);
        bool tradeItem = price > 0;
        tf_TradeBottom.SetActivate(tradeItem);
        m_TradePrice.text = price.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_Container as RectTransform);
        return true;
    }

    private void Update()
    {
        if (!m_Interact)
            return;

        m_InteractData.SetWorldViewPortAnchor(m_Interact.transform.position, CameraController.Instance.m_Camera, Time.deltaTime*10f);
    }

    void SetInteractData(InteractBase interact)
    {
        m_CommonName.localizeKey = interact.GetUITitleKey();
        m_CommonIntro.localizeKey = interact.GetUIIntroKey();
        m_CommonImage.sprite = UIManager.Instance.m_CommonSprites[interact.m_InteractType.GetInteractIcon()];
    }

    void SetWeaponInfo(WeaponBase weapon)
    {
        SWeaponInfos weaponInfo = weapon.m_WeaponInfo;
        m_WeaponImage.sprite = UIManager.Instance.m_WeaponSprites[weaponInfo.m_Weapon.GetSprite(true)];
        m_WeaponName.localizeKey = weaponInfo.m_Weapon.GetNameLocalizeKey();
        m_WeaponName.color = TCommon.GetHexColor(weaponInfo.m_Rarity.GetUIColor());
        m_ClipSize.text = weapon.I_ClipAmount.ToString();

        m_WeaponScore.ClearGrid();
        int baseScore = (int)weapon.m_WeaponInfo.m_Rarity;
        int enhanceScore = weapon.m_EnhanceLevel;
        for (int i = 0; i < baseScore; i++)
            m_WeaponScore.AddItem().SetScore(false);
        for (int i = 0; i < enhanceScore; i++)
            m_WeaponScore.AddItem().SetScore(true);
        m_WeaponScore.Sort((a, b) => a.Key - b.Key);
    }

    void SetPerkInfo(ExpirePlayerPerkBase perk)
    {
        m_PerkImage.sprite = UIManager.Instance.m_ExpireSprites[perk.GetExpireSprite()];
        m_PerkName.localizeKey = perk.GetNameLocalizeKey();
        m_PerkDetail.formatText(perk.GetDetailLocalizeKey(), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value1), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value2), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value3));
    }
}
