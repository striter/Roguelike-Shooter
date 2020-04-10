using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Armory : UIPage {
    UIT_GridControllerGridItem<UIGI_ArmoryItem> m_UnlockedGrid, m_BlueprintGrid;
    Button m_UnlockButton;
    Text m_Title,m_ButtonTitle;
    enum_PlayerWeapon m_SelectingWeapon;
    protected override void Init()
    {
        base.Init();
        m_UnlockedGrid = new UIT_GridControllerGridItem<UIGI_ArmoryItem>(rtf_Container.Find("Unlocked"));
        m_BlueprintGrid = new UIT_GridControllerGridItem<UIGI_ArmoryItem>(rtf_Container.Find("Blueprints"));
        m_UnlockButton = rtf_Container.Find("Unlock").GetComponent<Button>();
        m_ButtonTitle = m_UnlockButton.GetComponentInChildren<Text>();
        m_Title = rtf_Container.Find("Title").GetComponent<Text>();
        m_UnlockButton.onClick.AddListener(OnUnlockButtonClick);
        InitArmory();
    }

    void InitArmory()
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


    bool m_Blueprint=false;
    bool m_Unlocked=false;
    bool m_Equipping=false;

    void OnWeaponClick(enum_PlayerWeapon weapon)
    {
        m_SelectingWeapon = weapon;
         m_Blueprint = GameDataManager.m_ArmoryData.m_WeaponBlueprints.Contains(m_SelectingWeapon);
         m_Unlocked = GameDataManager.m_ArmoryData.m_WeaponsUnlocked.Contains(m_SelectingWeapon);

        bool valid = m_Blueprint || m_Unlocked;
        m_Title.SetActivate(valid);
        m_UnlockButton.SetActivate(valid);
        if (!valid)
            return;

        m_Equipping = weapon == GameDataManager.m_ArmoryData.m_WeaponSelected;
        if(m_Blueprint)
        {
            m_Title.text = "Blueprint:" + weapon + " " + GameDataManager.GetArmoryUnlockPrice(weapon);
            m_UnlockButton.interactable = GameDataManager.CanArmoryUnlock(weapon);
            m_ButtonTitle.text = "Unlock";
;        }
        else
        {
            m_Title.text = (m_Equipping ? "Equipping:":"Unlocked:") + weapon;
            m_UnlockButton.interactable = !m_Equipping;
            m_ButtonTitle.text = "Equip";
        }
    }

    void OnUnlockButtonClick()
    {
        if (m_Blueprint)
        {
            GameDataManager.OnArmoryUnlock(m_SelectingWeapon);
            InitArmory();
        }

        if (m_Unlocked)
            GameDataManager.OnArmorySelect(m_SelectingWeapon);

        CampManager.Instance.m_LocalPlayer.ReforgeWeapon(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(m_SelectingWeapon)));
        OnWeaponClick(m_SelectingWeapon);
    }

}

