using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UI_CharacterSelect : UIPage {

    Transform m_CharacterStatus;
    Transform m_CharacterDetail;
    Text m_CharacterName, m_CharacterIntro,m_CharacterAbility;
    Button m_CharacterConfirm;
    UIT_GridControlledSingleSelect<UIGI_CharacterSelectItem> m_CharacterSelect;
    UIT_GridControllerComponent<Text> m_AttributesGrid;

    InteractCampCharacterSelect m_SelectModel;
    enum_PlayerCharacter m_SelectingCharacter;
    protected override void Init()
    {
        base.Init();
        m_CharacterStatus = rtf_Container.Find("CharacterStatus");

        m_CharacterDetail = rtf_Container.Find("CharacterDetail");
        m_CharacterName = m_CharacterDetail.Find("Name").GetComponent<Text>();
        m_CharacterIntro = m_CharacterDetail.Find("Intro").GetComponent<Text>();
        m_CharacterAbility = m_CharacterDetail.Find("Ability").GetComponent<Text>();
        m_CharacterConfirm = m_CharacterDetail.Find("Confirm").GetComponent<Button>();
        m_CharacterConfirm.onClick.AddListener(OnConfirmBtnClick);
        m_AttributesGrid = new UIT_GridControllerComponent<Text>(m_CharacterStatus.Find("Attributes"));
        m_CharacterSelect = new UIT_GridControlledSingleSelect<UIGI_CharacterSelectItem>(rtf_Container.Find("CharacterSelectGrid"),OnCharacterSelect);
        TCommon.TraversalEnum((enum_PlayerCharacter character) => { m_CharacterSelect.AddItem((int)character); });
    }

    public void Play(InteractCampCharacterSelect characterSelect)
    {
        m_SelectingCharacter = enum_PlayerCharacter.Invalid;
        CampManager.Instance.RecycleLocalCharacter();
        m_SelectModel = characterSelect;
        m_CharacterSelect.OnItemClick((int)GameDataManager.m_CharacterData.m_CharacterSelected);
    }

    void OnCharacterSelect(int index)=> UpdateCharacter((enum_PlayerCharacter)index);

    void OnConfirmBtnClick()=>OnCancelBtnClick();

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        CampManager.Instance.OnSwitchCharacter(m_SelectModel.m_Character);
    }

    void UpdateCharacter(enum_PlayerCharacter character)
    {
        if (m_SelectingCharacter == character)
            return;
        m_SelectingCharacter = character;

        m_CharacterName.text = character.GetNameLocalizeKey();
        m_CharacterIntro.text = character.GetIntroLocalizeKey();
        m_CharacterAbility.text = character.GetAbilityNameLocalizeKey();
        m_CharacterConfirm.interactable = GameDataManager.CanChangeCharacter(character);
        EntityCharacterPlayer characterModel = m_SelectModel.ShowCharacter(character);
        m_AttributesGrid.ClearGrid();
        m_AttributesGrid.AddItem().text = "Armor:" + characterModel.I_DefaultArmor;
        m_AttributesGrid.AddItem().text = "Move Speed:" + characterModel.F_MovementSpeed;
        m_AttributesGrid.AddItem().text = "Health:" + characterModel.I_MaxHealth;
    }
}
