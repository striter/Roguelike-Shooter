using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSpecialClasses;
using UnityEngine.UI;

public class UIC_PlayerInteract : UIControlBase {
    RectTransform rtf_InteractData;
    Transform tf_Container;
    Transform tf_TradePrice;
    UIT_TextExtend m_TradePrice;
    Transform tf_Weapon;
    Image m_WeaponBackground,m_WeaponImage;
    UIT_TextExtend m_WeaponName;
    Transform tf_Action;
    UIGI_ActionItemBase m_Action;
    Transform tf_Item;
    UIT_TextExtend m_ItemName;
    Transform tf_Intro;
    UI_WeaponActionHUD m_WeaponActionHUD;
    UIT_TextExtend m_Intro;
    InteractBase m_interact;
    protected override void Init()
    {
        base.Init();
        rtf_InteractData = transform.Find("InteractData").GetComponent<RectTransform>();
        tf_Container = rtf_InteractData.Find("Container");
        tf_TradePrice = tf_Container.Find("TradePrice");
        m_TradePrice = tf_TradePrice.Find("Amount").GetComponent<UIT_TextExtend>();
        tf_Weapon = tf_Container.Find("Weapon");
        m_WeaponBackground = tf_Weapon.Find("Background").GetComponent<Image>();
        m_WeaponImage = tf_Weapon.Find("WeaponImage").GetComponent<Image>();
        m_WeaponName = tf_Weapon.Find("WeaponName").GetComponent<UIT_TextExtend>();
        tf_Action = tf_Container.Find("Action");
        m_Action = tf_Action.Find("ActionItem").GetComponent<UIGI_ActionItemBase>();
        m_Action.Init();
        tf_Item = tf_Container.Find("Item");
        m_ItemName = tf_Item.Find("Name").GetComponent<UIT_TextExtend>();
        tf_Intro = tf_Container.Find("Intro");
        m_WeaponActionHUD = new UI_WeaponActionHUD(tf_Intro.Find("WeaponAction"));
        m_Intro = tf_Intro.Find("Intro").GetComponent<UIT_TextExtend>();
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
        rtf_InteractData.SetActivate(m_interact != null);
        if (!m_interact)
            return;
        rtf_InteractData.SetWorldViewPortAnchor(m_interact.transform.position, CameraController.Instance.m_Camera);

        bool tradeOn = false;
        InteractBase interactInfo = m_interact;
        switch (m_interact.m_InteractType)
        {
            case enum_Interaction.ContainerBattle:
                interactInfo = (m_interact as InteractContainerBattle).m_TradeInteract;
                break;
            case enum_Interaction.ContainerTrade:
                {
                    tradeOn = true;
                    InteractContainerTrade trade = m_interact as InteractContainerTrade;
                    m_TradePrice.text = trade.m_TradePrice.ToString();
                    interactInfo = trade.m_TradeInteract;
                }
                break;
        }
        tf_TradePrice.SetActivate(tradeOn);
        UpdateInfo(interactInfo);

        LayoutRebuilder.ForceRebuildLayoutImmediate(tf_Container as RectTransform);
    }
    void UpdateInfo(InteractBase interactInfo)
    {
        bool weaponOn=false;
        bool weaponActionOn = false;
        bool actionOn = false;
        bool itemOn = false;
        switch (interactInfo.m_InteractType)
        {
            case enum_Interaction.Weapon:
                {
                    weaponOn = true;
                    weaponActionOn = true;
                    WeaponBase weapon = (interactInfo as InteractWeapon).m_Weapon;
                    m_WeaponBackground.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Rarity.GetUIInteractBackground()];
                    m_WeaponImage.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Weapon.GetSpriteName()];
                    m_WeaponName.color = TCommon.GetHexColor(weapon.m_WeaponInfo.m_Rarity.GetUITextColor());
                    m_WeaponName.localizeKey = weapon.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();

                    m_WeaponActionHUD.SetInfo(weapon.m_WeaponAction);
                    if(weapon.m_WeaponAction!=null)
                        weapon.m_WeaponAction.SetActionIntro(m_Intro);
                    else
                        m_Intro.localizeKey = "UI_Weapon_ActionInvalidIntro";
                }
                break;
            case enum_Interaction.Action:
                {
                    actionOn = true;
                    InteractAction action = interactInfo as InteractAction;
                    m_Action.SetInfo(action.m_Action);
                    action.m_Action.SetActionIntro(m_Intro);
                }
                break;
            default:
                {
                    itemOn = true;
                    m_ItemName.localizeKey = interactInfo.GetNameLocalizeKey();
                    m_Intro.localizeKey = interactInfo.GetIntroLocalizeKey();
                }
                break;
        }
        tf_Weapon.SetActivate(weaponOn);
        m_WeaponActionHUD.transform.SetActivate(weaponActionOn);
        tf_Action.SetActivate(actionOn);
        tf_Item.SetActivate(itemOn);

        LayoutRebuilder.ForceRebuildLayoutImmediate(tf_Intro as RectTransform);
    }

    private void Update()
    {
        if (!m_interact)
            return;

        rtf_InteractData.SetWorldViewPortAnchor(m_interact.transform.position, CameraController.Instance.m_Camera, Time.deltaTime*10f);
    }

}
