using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Armory : UIPage {
    UIT_GridControllerGridItem<UIGI_ArmoryItem> m_UnlockedGrid, m_BlueprintGrid;
    Button m_UnlockButton;
    Text m_Title;
    enum_PlayerWeapon m_SelectingWeapon;
    protected override void Init()
    {
        base.Init();
        m_UnlockedGrid = new UIT_GridControllerGridItem<UIGI_ArmoryItem>(rtf_Container.Find("Unlocked"));
        m_BlueprintGrid = new UIT_GridControllerGridItem<UIGI_ArmoryItem>(rtf_Container.Find("Blueprints"));
        m_UnlockButton = rtf_Container.Find("Unlock").GetComponent<Button>();
        m_Title = rtf_Container.Find("Title").GetComponent<Text>();
        m_UnlockButton.onClick.AddListener(OnUnlockButtonClick);
    }

    public void Play()
    {
        m_UnlockedGrid.ClearGrid();
        m_BlueprintGrid.ClearGrid();
        GameDataManager.m_ArmoryData.m_WeaponBlueprints.Traversal((enum_PlayerWeapon weapon) =>
        {
            m_BlueprintGrid.AddItem((int)weapon).Play(OnWeaponClick);
        });
        GameDataManager.m_ArmoryData.m_WeaponsUnlocked.Traversal((enum_PlayerWeapon weapon) =>
        {
            m_UnlockedGrid.AddItem((int)weapon).Play(OnWeaponClick);
        });
        OnWeaponClick(enum_PlayerWeapon.Invalid);
    }

    void OnWeaponClick(enum_PlayerWeapon weapon)
    {
        m_SelectingWeapon = weapon;
        bool bluePrint = GameDataManager.m_ArmoryData.m_WeaponBlueprints.Contains(m_SelectingWeapon);
        m_UnlockButton.SetActivate(bluePrint);
        m_Title.text = bluePrint ?( "Blueprint:" + weapon + " " + GameDataManager.GetArmoryUnlockPrice(weapon)):"Unlocked:"+weapon;
        if (!bluePrint)
            return;
        m_UnlockButton.interactable = GameDataManager.CanArmoryUnlock(weapon);
    }

    void OnUnlockButtonClick()
    {
        GameDataManager.OnArmoryUnlock(m_SelectingWeapon);
        Play();
    }

}

