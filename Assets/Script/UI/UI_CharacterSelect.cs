using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UI_CharacterSelect : UIPage {

    Transform m_CharacterStatus;
    UIT_GridControllerGridItem<UIGI_EquipmentItemBase> m_EquipmentsGrid;
    UIT_GridControllerMono<Text> m_AttributesGrid;
    Text m_Passive;
    Transform m_CharacterDetail;
    Text m_CharacterName, m_CharacterIntro,m_CharacterAbility;
    Button m_CharacterConfirm;
    UIT_GridControlledSingleSelect<UIGI_CharacterSelectItem> m_CharacterSelect;

    InteractCampCharacterSelect m_SelectModel;
    enum_PlayerCharacter m_SelectingCharacter;
    protected override void Init()
    {
        base.Init();
        m_CharacterStatus = rtf_Container.Find("CharacterStatus");
        m_Passive = m_CharacterStatus.Find("Passive").GetComponent<Text>();
        m_AttributesGrid = new UIT_GridControllerMono<Text>(m_CharacterStatus.Find("Attributes"));
        m_EquipmentsGrid = new UIT_GridControllerGridItem<UIGI_EquipmentItemBase>(m_CharacterStatus.Find("Equipments"));

        m_CharacterDetail = rtf_Container.Find("CharacterDetail");
        m_CharacterName = m_CharacterDetail.Find("Name").GetComponent<Text>();
        m_CharacterIntro = m_CharacterDetail.Find("Intro").GetComponent<Text>();
        m_CharacterAbility = m_CharacterDetail.Find("Ability").GetComponent<Text>();
        m_CharacterConfirm = m_CharacterDetail.Find("Confirm").GetComponent<Button>();
        m_CharacterConfirm.onClick.AddListener(OnConfirmBtnClick);

        m_CharacterSelect = new UIT_GridControlledSingleSelect<UIGI_CharacterSelectItem>(rtf_Container.Find("CharacterSelectGrid"),OnCharacterSelect);
        TCommon.TraversalEnum((enum_PlayerCharacter character) => { m_CharacterSelect.AddItem((int)character); });
        m_SelectingCharacter = enum_PlayerCharacter.Invalid;
    }

    public void Play(InteractCampCharacterSelect characterSelect)
    {
        m_SelectModel = characterSelect;
        m_CharacterSelect.OnItemClick((int)GameDataManager.m_CharacterData.m_CharacterSelected);
    }

    void OnCharacterSelect(int index)=> UpdateCharacter((enum_PlayerCharacter)index);

    void OnConfirmBtnClick()
    {
        GameDataManager.SwitchCharacter(m_SelectingCharacter);
        CampManager.Instance.OnSwitchCharacter(m_SelectModel.m_Character);
        m_SelectModel.OnCharacterSwitch();
        OnCancelBtnClick();
    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        m_SelectModel.RecycleUnusedCharacter();
    }

    void UpdateCharacter(enum_PlayerCharacter character)
    {
        if (m_SelectingCharacter == character)
            return;
        m_SelectingCharacter = character;

        m_CharacterName.text = character.GetNameLocalizeKey();
        m_CharacterIntro.text = character.GetIntroLocalizeKey();
        m_CharacterAbility.text = character.GetAbilityLocalizeKey();
        m_CharacterConfirm.interactable = GameDataManager.CanChangeCharacter(character);

        EntityCharacterBase characterModel = m_SelectModel.ShowCharacter(character);
        ExpirePlayerUpgradeCombine upgrade= GameDataManager.CreateUpgradeCombination(GameDataManager.m_EquipmentDepotData.GetSelectedEquipments(), GameDataManager.m_CharacterData.GetUpgradeData(character));

        m_EquipmentsGrid.ClearGrid();
        upgrade.m_EquipmentData.Traversal((int index, EquipmentSaveData equipment) => { m_EquipmentsGrid.AddItem(index).Play(equipment); });
        m_Passive.SetActivate(upgrade.m_HavePassive);
        if (upgrade.m_HavePassive)
            m_Passive.text = upgrade.GetPassiveLocalizeKey();

        m_AttributesGrid.ClearGrid();
        TCommon.TraversalEnum((enum_CharacterUpgradeType type) =>
        {
            Text attribute = m_AttributesGrid.AddItem((int)type);
            attribute.text = type.ToString()+":";
            switch(type)
            {
                case enum_CharacterUpgradeType.Armor:
                    attribute.text += characterModel.I_DefaultArmor;
                    break;
                case enum_CharacterUpgradeType.Health:
                    attribute.text += characterModel.I_MaxHealth;
                    break;
                case enum_CharacterUpgradeType.MovementSpeed:
                    attribute.text += characterModel.F_MovementSpeed;
                    break;
                case enum_CharacterUpgradeType.CriticalRate:
                case enum_CharacterUpgradeType.Damage:
                case enum_CharacterUpgradeType.FireRate:
                    attribute.text += 0;
                    break;
            }
            attribute.text += "(+" + upgrade.m_UpgradeDatas[type] + ")";
        });
    }
}
