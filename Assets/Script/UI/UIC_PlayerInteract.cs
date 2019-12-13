using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSpecialClasses;
using UnityEngine.UI;

public class UIC_PlayerInteract : UIControlBase
{
    InteractBase m_interact;
    RectTransform rtf_InteractData;
    Transform tf_Container;
    Transform tf_Top;
    Transform tf_Weapon, tf_Action;
    Transform tf_Equipment, tf_Ability;

    Transform tf_Bottom;
    Transform tf_Trade, tf_Pickup;
    UIT_TextExtend m_TradePrice;
    UIC_ActionInteractData m_ActionData;
    Transform tf_WeaponData;
    UIC_WeaponActionData m_weaponActionData;
    UIC_WeaponData m_weaponData;

    protected override void Init()
    {
        base.Init();
        rtf_InteractData = transform.Find("InteractData").GetComponent<RectTransform>();
        tf_Container = rtf_InteractData.Find("Container");
        tf_Top = tf_Container.Find("InteractTop");
        tf_Weapon = tf_Top.Find("Weapon");
        tf_Action = tf_Top.Find("Action");
        tf_Ability = tf_Action.Find("Ability");
        tf_Equipment = tf_Action.Find("Equipment");

        tf_Bottom = tf_Container.Find("InteractBottom");
        tf_Trade = tf_Bottom.Find("Trade");
        m_TradePrice = tf_Trade.Find("Amount").GetComponent<UIT_TextExtend>();
        tf_Pickup = tf_Bottom.Find("Pickup");
        tf_WeaponData = tf_Container.Find("WeaponData");
        m_weaponData = new UIC_WeaponData(tf_WeaponData.Find("Weapon"));
        m_weaponActionData = new UIC_WeaponActionData(tf_WeaponData.Find("Action"));

        m_ActionData = new UIC_ActionInteractData(tf_Container.Find("ActionData"));

        rtf_InteractData.SetActivate(false);
        TBroadCaster<enum_BC_UIStatus>.Add<InteractBase>(enum_BC_UIStatus.UI_PlayerInteractStatus,OnInteractStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<InteractBase>(enum_BC_UIStatus.UI_PlayerInteractStatus, OnInteractStatus);
    }

    public void Play(UnityEngine.Events.UnityAction OnInteractClick)
    {
        tf_Bottom.Find("Button").GetComponent<Button>().onClick.AddListener(OnInteractClick);
    }

    void OnInteractStatus(InteractBase _interact)
    {
        m_interact = _interact;

        int tradePrice = -1;
        InteractBase targetItem = null;
        if (m_interact != null)
        {
            switch (m_interact.m_InteractType)
            {
                case enum_Interaction.Action:
                case enum_Interaction.Weapon:
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
        bool targetvalid = false;
        bool weaponShow = false;
        bool actionShow = false;
        if (interactInfo != null)
        {
            switch (interactInfo.m_InteractType)
            {
                case enum_Interaction.Action:
                    actionShow = true;
                    targetvalid = true;
                    InteractAction actionInteract = interactInfo as InteractAction;
                    m_ActionData.SetInfo(actionInteract.m_Action);
                    tf_Ability.SetActivate(actionInteract.m_Action.m_ActionType == enum_ActionType.Ability);
                    tf_Equipment.SetActivate(actionInteract.m_Action.m_ActionType == enum_ActionType.Equipment);
                    break;
                case enum_Interaction.Weapon:
                    weaponShow = true;
                    targetvalid = true;
                    InteractWeapon weaponInteract = interactInfo as InteractWeapon;
                    m_weaponData.UpdateInfo(weaponInteract.m_Weapon);
                    m_weaponData.UpdateAmmoInfo(weaponInteract.m_Weapon.I_AmmoLeft, weaponInteract.m_Weapon.I_ClipAmount);
                    bool actionValid = weaponInteract.m_Weapon.m_WeaponAction != null;
                    m_weaponActionData.transform.SetActivate(actionValid);
                    if (actionValid)
                    {
                        m_weaponActionData.SetInfo(weaponInteract.m_Weapon.m_WeaponAction);
                        m_weaponActionData.Tick(weaponInteract.m_Weapon.m_ActionEnergyRequirementLeft);
                    }
                    break;
                default:
                    targetvalid = false;
                    break;
            }
        }
        tf_Weapon.SetActivate(weaponShow);
        tf_Action.SetActivate(actionShow);
        tf_WeaponData.SetActivate(weaponShow);
        m_ActionData.transform.SetActivate(actionShow);
        tf_Trade.SetActivate(price >= 0);
        tf_Pickup.SetActivate(price < 0);
        m_TradePrice.text = price.ToString();
        rtf_InteractData.SetActivate(targetvalid);
        if(targetvalid)
            LayoutRebuilder.ForceRebuildLayoutImmediate(tf_Container as RectTransform);
        return targetvalid;
    }

    private void Update()
    {
        if (!m_interact)
            return;

        rtf_InteractData.SetWorldViewPortAnchor(m_interact.transform.position, CameraController.Instance.m_Camera, Time.deltaTime*10f);
    }

}
