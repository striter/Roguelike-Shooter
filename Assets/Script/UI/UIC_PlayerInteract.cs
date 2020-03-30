using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSpecialClasses;
using UnityEngine.UI;
using System;

public class UIC_PlayerInteract : UIControlBase
{
    InteractBase m_interact;
    RectTransform rtf_InteractData;
    Transform tf_Container;
    Transform tf_Top;
    UIT_TextExtend m_CommonTop;
    Transform tf_Weapon;

    Transform tf_Bottom;
    UIT_TextExtend m_BottomTips;
    Transform tf_Trade;

    UIT_TextExtend m_TradePrice;
    UIC_EquipmentNameFormatIntro m_EquipmentData;
    UIC_WeaponData m_weaponData;

    Transform tf_Common;
    UIT_TextExtend m_CommonIntro;
    Image m_CommonImage;
    protected override void Init()
    {
        base.Init();
        rtf_InteractData = transform.Find("InteractData").GetComponent<RectTransform>();
        tf_Container = rtf_InteractData.Find("Container");
        tf_Top = tf_Container.Find("InteractTop");
        m_CommonTop = tf_Top.Find("Common").GetComponent<UIT_TextExtend>();

        tf_Bottom = tf_Container.Find("InteractBottom");
        m_BottomTips = tf_Bottom.Find("Common").GetComponent<UIT_TextExtend>();
        tf_Trade = tf_Bottom.Find("Trade");
        m_TradePrice = tf_Trade.Find("Amount").GetComponent<UIT_TextExtend>();

        tf_Weapon = tf_Top.Find("Weapon");

        m_weaponData = new UIC_WeaponData(tf_Container.Find("WeaponData"));
        m_EquipmentData = new UIC_EquipmentNameFormatIntro(tf_Container.Find("EquipmentData"));

        tf_Common = tf_Container.Find("CommonData");
        m_CommonIntro = tf_Common.Find("Intro").GetComponent<UIT_TextExtend>();
        m_CommonImage = tf_Common.Find("Image").GetComponent<Image>();

        rtf_InteractData.SetActivate(false);
        TBroadCaster<enum_BC_UIStatus>.Add<InteractBase>(enum_BC_UIStatus.UI_PlayerInteractStatus,OnInteractStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<InteractBase>(enum_BC_UIStatus.UI_PlayerInteractStatus, OnInteractStatus);
    }
    
    void OnInteractStatus(InteractBase _interact)
    {
        m_interact = _interact;

        InteractBase targetItem = null;
        int tradePrice = -1;
        if (m_interact != null)
        {
            tradePrice = _interact.m_TradePrice;
            switch (m_interact.m_InteractType)
            {
                default:
                    targetItem = m_interact;
                    break;
                case enum_Interaction.TradeContainer:
                    InteractTradeContainer trade = m_interact as InteractTradeContainer;
                    targetItem = trade.m_TradeInteract;
                    break;
            }
        }
        if(UpdateInfo(targetItem,tradePrice))
        rtf_InteractData.SetWorldViewPortAnchor(m_interact.transform.position, CameraController.Instance.m_Camera);
        
    }
    bool UpdateInfo(InteractBase interactInfo,int price)
    {
        if (interactInfo == null)
        {
            rtf_InteractData.SetActivate(false);
            return false;
        }
        rtf_InteractData.SetActivate(true);
        bool isCommon = false;
        bool isWeapon = false;
        bool isAction = false;
        if (interactInfo != null)
        {
            switch (interactInfo.m_InteractType)
            {
                case enum_Interaction.PerkPickup:
                    isAction = true;
                    InteractPerkPickup actionInteract = interactInfo as InteractPerkPickup;
                    m_EquipmentData.SetInfo(PerkDataManager.GetPerkData(actionInteract.m_PerkID));
                    break;
                case enum_Interaction.WeaponPickup:
                    isWeapon = true;
                    InteractWeaponPickup weaponInteract = interactInfo as InteractWeaponPickup;
                    m_weaponData.UpdateInfo(weaponInteract.m_Weapon);
                    m_weaponData.UpdateAmmoInfo(weaponInteract.m_Weapon.m_WeaponInfo.m_ClipAmount, weaponInteract.m_Weapon.m_WeaponInfo.m_ClipAmount);
                    break;
                default:
                    isCommon = true;
                    m_CommonTop.localizeKey = interactInfo.GetUITitleKey();
                    m_CommonIntro.localizeKey = interactInfo.GetUIIntroKey();
                    m_CommonImage.sprite = UIManager.Instance.m_CommonSprites[interactInfo.m_InteractType.GetInteractIcon()];
                    break;
            }
        }
        m_CommonTop.SetActivate(isCommon);
        tf_Common.SetActivate(isCommon);
        tf_Weapon.SetActivate(isWeapon);
        m_weaponData.transform.SetActivate(isWeapon);
        m_EquipmentData.transform.SetActivate(isAction);
        bool tradeItem = price >= 0;
        tf_Trade.SetActivate(tradeItem);
        m_BottomTips.SetActivate(!tradeItem);
        if (!tradeItem)
            m_BottomTips.localizeKey = interactInfo.GetUIBottomKey();
        m_TradePrice.text = price.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tf_Container as RectTransform);
        return true;
    }

    private void Update()
    {
        if (!m_interact)
            return;

        rtf_InteractData.SetWorldViewPortAnchor(m_interact.transform.position, CameraController.Instance.m_Camera, Time.deltaTime*10f);
    }

}
