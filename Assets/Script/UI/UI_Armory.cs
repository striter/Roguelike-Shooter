using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Armory : UIPage {
    UIT_GridControllerGridItemScrollView<UIGI_ArmoryWeaponSelect> m_ArmoryGrid;
    UIC_WeaponInfo m_WeaponInfo;
    Button m_UnlockButton;
    enum_PlayerWeaponIdentity m_SelectingWeapon;
    protected override void Init()
    {
        base.Init();
        m_ArmoryGrid = new UIT_GridControllerGridItemScrollView<UIGI_ArmoryWeaponSelect>(rtf_Container.Find("WeaponSelect/ScrollRect"),36);
        m_WeaponInfo = new UIC_WeaponInfo(rtf_Container.Find("WeaponInfo"));
        m_UnlockButton = rtf_Container.Find("UnlockBtn").GetComponent<Button>();
        m_UnlockButton.onClick.AddListener(OnUnlockButtonClick);
    }

    public override void OnPlay(bool doAnim, Action<UIPageBase> OnPageExit)
    {
        base.OnPlay(doAnim, OnPageExit);
        m_SelectingWeapon = enum_PlayerWeaponIdentity.Invalid;
        OnWeaponClick(InitArmory());
    }

    enum_PlayerWeaponIdentity InitArmory()
    {
        enum_PlayerWeaponIdentity firstAvailableWeapon = enum_PlayerWeaponIdentity.Invalid;
        m_ArmoryGrid.ClearGrid();

        GameDataManager.m_ArmoryData.m_WeaponBlueprints.Traversal((enum_PlayerWeaponIdentity weapon) => { if (firstAvailableWeapon == enum_PlayerWeaponIdentity.Invalid) firstAvailableWeapon = weapon; m_ArmoryGrid.AddItem((int)weapon).Play(false, OnWeaponClick); });
        GameDataManager.m_ArmoryData.m_WeaponsUnlocked.Traversal((enum_PlayerWeaponIdentity weapon) => { if (firstAvailableWeapon == enum_PlayerWeaponIdentity.Invalid) firstAvailableWeapon = weapon; m_ArmoryGrid.AddItem((int)weapon).Play(true, OnWeaponClick); });
        return firstAvailableWeapon;
    }


    void OnWeaponClick(enum_PlayerWeaponIdentity weapon)
    {
        if (m_SelectingWeapon != enum_PlayerWeaponIdentity.Invalid)
            m_ArmoryGrid.GetItem((int)weapon).OnHighlight(false);
        m_SelectingWeapon = weapon;
        m_ArmoryGrid.GetItem((int)weapon).OnHighlight(true);

        bool m_Unlocked = GameDataManager.m_ArmoryData.m_WeaponsUnlocked.Contains(m_SelectingWeapon);

        m_UnlockButton.SetActivate(!m_Unlocked);
        m_WeaponInfo.SetWeaponInfo(GameObjectManager.GetWeaponData(weapon),m_Unlocked);
    }

    void OnUnlockButtonClick()
    {
        GameDataManager.OnArmoryUnlock(m_SelectingWeapon);
        InitArmory();
        OnWeaponClick(m_SelectingWeapon);
    }

}

