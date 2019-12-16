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
    Transform tf_Weapon, tf_Equipment, tf_Ability;

    Transform tf_Bottom;
    UIT_TextExtend m_BottomTips;
    Transform tf_Trade, tf_Pickup;


    UIT_TextExtend m_TradePrice;
    UIC_ActionInteractData m_ActionData;
    Transform tf_WeaponData;
    UIC_ActionNameData m_weaponActionData;
    UIC_WeaponData m_weaponData;

    Transform tf_Common;
    UIT_TextExtend m_CommonIntro;

    Action OnInteractClick;
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
        tf_Ability = tf_Top.Find("Ability");
        tf_Equipment = tf_Top.Find("Equipment");

        tf_WeaponData = tf_Container.Find("WeaponData");
        m_weaponData = new UIC_WeaponData(tf_WeaponData.Find("Weapon"));
        m_weaponActionData = new UIC_ActionNameData(tf_WeaponData.Find("Action"));

        m_ActionData = new UIC_ActionInteractData(tf_Container.Find("ActionData"));

        tf_Common = tf_Container.Find("CommonData");
        m_CommonIntro = tf_Common.Find("Intro").GetComponent<UIT_TextExtend>();
        tf_Bottom.Find("Button").GetComponent<Button>().onClick.AddListener(OnInteractBtnClick);

        rtf_InteractData.SetActivate(false);
        TBroadCaster<enum_BC_UIStatus>.Add<InteractBase>(enum_BC_UIStatus.UI_PlayerInteractStatus,OnInteractStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<InteractBase>(enum_BC_UIStatus.UI_PlayerInteractStatus, OnInteractStatus);
    }

    public void DoBindings(Action _OnInteractClick) => OnInteractClick = _OnInteractClick;
    void OnInteractBtnClick()=>OnInteractClick?.Invoke();

    void OnInteractStatus(InteractBase _interact)
    {
        m_interact = _interact;

        int tradePrice = -1;
        InteractBase targetItem = null;
        if (m_interact != null)
        {
            switch (m_interact.m_InteractType)
            {
                default:
                    targetItem = m_interact;
                    break;
                case enum_Interaction.ContainerTrade:
                    InteractContainerTrade trade = m_interact as InteractContainerTrade;
                    tradePrice = trade.m_TradePrice;
                    targetItem = trade.m_TradeInteract;
                    break;
                case enum_Interaction.ContainerBattle:
                    InteractContainerBattle battle = m_interact as InteractContainerBattle;
                    targetItem = battle.m_TradeInteract;
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
        bool isAbility = false;
        bool isEquipment = false;
        if (interactInfo != null)
        {
            switch (interactInfo.m_InteractType)
            {
                case enum_Interaction.Action:
                    isAction = true;
                    InteractAction actionInteract = interactInfo as InteractAction;
                    m_ActionData.SetInfo(actionInteract.m_Action);
                    isAbility = actionInteract.m_Action.m_ActionType == enum_ActionType.Ability;
                    isEquipment = actionInteract.m_Action.m_ActionType == enum_ActionType.Equipment;
                    break;
                case enum_Interaction.Weapon:
                    isWeapon = true;
                    InteractWeapon weaponInteract = interactInfo as InteractWeapon;
                    m_weaponData.UpdateInfo(weaponInteract.m_Weapon);
                    m_weaponData.UpdateAmmoInfo(weaponInteract.m_Weapon.m_WeaponInfo.m_ClipAmount, weaponInteract.m_Weapon.m_WeaponInfo.m_ClipAmount);
                    bool actionValid = weaponInteract.m_Weapon.m_WeaponAction != null;
                    m_weaponActionData.transform.SetActivate(actionValid);
                    if (actionValid)
                        m_weaponActionData.SetInfo(weaponInteract.m_Weapon.m_WeaponAction);
                    break;
                default:
                    isCommon = true;
                    m_CommonTop.localizeKey = interactInfo.GetTitleLocalizeKey();
                    m_CommonIntro.localizeKey = interactInfo.GetIntroLocalizeKey();
                    break;
            }
        }
        m_CommonTop.SetActivate(isCommon);
        tf_Common.SetActivate(isCommon);
        tf_Weapon.SetActivate(isWeapon);
        tf_WeaponData.SetActivate(isWeapon);
        tf_Ability.SetActivate(isAbility);
        tf_Equipment.SetActivate(isEquipment);
        m_ActionData.transform.SetActivate(isAction);
        bool tradeItem = price >= 0;
        tf_Trade.SetActivate(tradeItem);
        m_BottomTips.SetActivate(!tradeItem);
        if (!tradeItem)
            m_BottomTips.localizeKey = interactInfo.GetBottomLocalizeKey();
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
