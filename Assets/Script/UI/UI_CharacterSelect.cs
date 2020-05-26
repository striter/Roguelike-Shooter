using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UI_CharacterSelect : UIPage {
    Transform m_Attributes;
    Text m_AttributeHealth, m_AttributeArmor, m_AttributeMovement, m_AttributeCritical;
    UIC_WeaponInfoName m_AttributeWeapon;

    UIT_EventTriggerListener m_DragContainer;

    Transform m_AttributeStatus;
    Transform m_AttributeMaxEnhance;
    UIT_TextExtend m_AttributeUpgradeTitle;
    Button m_AttributeButton;
    UIT_TextExtend m_AttributeButtonTitle;
    Text m_AttributeButtonAmount;

    Transform m_Character;
    UIT_TextExtend m_CharacterName;
    UIT_GridControllerComponent<Image> m_CharacterEnhanceGrid;
    UIT_TextExtend m_CharacterIntro;
    UIC_CharacterAbility m_CharacterAbility;
    Button m_CharacterSkinBtn;
    UIC_Button m_CharacterConfirmBtn;

    UIT_GridControlledSingleSelect<UIGI_CharacterSelectItem> m_CharacterSelectGrid;

    InteractCampCharacterSelect m_ModelViewer;
    enum_PlayerCharacter m_SelectCharacter;
    protected override void Init()
    {
        base.Init();
        m_Attributes = rtf_Container.Find("Attributes");
        m_AttributeHealth = m_Attributes.Find("Health/Amount").GetComponent<Text>();
        m_AttributeArmor = m_Attributes.Find("Armor/Amount").GetComponent<Text>();
        m_AttributeMovement = m_Attributes.Find("Movement/Amount").GetComponent<Text>();
        m_AttributeCritical = m_Attributes.Find("Critical/Amount").GetComponent<Text>();
        m_AttributeWeapon = new UIC_WeaponInfoName(m_Attributes.Find("Weapon/Info"));
        m_AttributeStatus = m_Attributes.Find("Status");
        m_AttributeMaxEnhance = m_AttributeStatus.Find("MaxEnhance");
        m_AttributeUpgradeTitle = m_AttributeStatus.Find("UpgradeTitle").GetComponent<UIT_TextExtend>();
        m_AttributeButton = m_AttributeStatus.Find("Button").GetComponent<Button>();
        m_AttributeButton.onClick.AddListener(OnAttributeButtonClick);
        m_AttributeButtonTitle = m_AttributeButton.transform.Find("Title").GetComponent<UIT_TextExtend>();
        m_AttributeButtonAmount = m_AttributeButton.transform.Find("Amount").GetComponent<Text>();

        m_Character = rtf_Container.Find("Character");
        m_CharacterName = m_Character.Find("Name").GetComponent<UIT_TextExtend>();
        m_CharacterEnhanceGrid = new UIT_GridControllerComponent<Image>(m_Character.Find("Enhance"));
        m_CharacterIntro = m_Character.Find("Intro").GetComponent<UIT_TextExtend>();
        m_CharacterAbility = new UIC_CharacterAbility(m_Character.Find("Ability"));
        m_CharacterConfirmBtn = new UIC_Button(m_Character.Find("ConfirmBtn"),OnCharacterButtonClick);

        m_DragContainer = rtf_Container.Find("DragContainer").GetComponent<UIT_EventTriggerListener>();

        m_CharacterSelectGrid = new UIT_GridControlledSingleSelect<UIGI_CharacterSelectItem>(rtf_Container.Find("CharacterSelect/Viewport/Content"),OnCharacterSelect);
    }

    public void Play(InteractCampCharacterSelect characterSelect)
    {
        CampManager.Instance.RecycleLocalCharacter();
        m_ModelViewer = characterSelect;
        m_DragContainer.D_OnDragDelta = m_ModelViewer.RotateCharacter;
        UpdateAllCharacterInfo(GameDataManager.m_CharacterData.m_CharacterSelected);
    }
    
    void UpdateAllCharacterInfo(enum_PlayerCharacter highlightCharacter)
    {
        m_CharacterSelectGrid.ClearGrid();
        TCommon.TraversalEnum((enum_PlayerCharacter character) => { m_CharacterSelectGrid.AddItem((int)character); });

        m_SelectCharacter = enum_PlayerCharacter.Invalid;
        m_CharacterSelectGrid.OnItemClick((int)highlightCharacter);
    }

    void OnCharacterSelect(int charcterIndex)
    {
        enum_PlayerCharacter character = (enum_PlayerCharacter)charcterIndex;
        if (m_SelectCharacter == character)
            return;
        m_SelectCharacter = character;
        m_ModelViewer.OnGenerateCharacter(m_SelectCharacter);
        UpdateSelectedInfo();
    }

    void UpdateSelectedInfo()
    {
        EntityCharacterPlayer _model = m_ModelViewer.m_CharacterModel;
        PlayerCharacterCultivateSaveData cultivateData = GameDataManager.m_CharacterData.GetCharacterCultivateDetail(m_SelectCharacter);

        m_AttributeHealth.text = (_model.I_MaxHealth + (cultivateData.m_Enhance >= enum_PlayerCharacterEnhance.Health ? GameConst.I_PlayerEnhanceMaxHealthAdditive : 0f)).ToString();
        m_AttributeArmor.text = (_model.I_DefaultArmor + (cultivateData.m_Enhance >= enum_PlayerCharacterEnhance.Armor ? GameConst.I_PlayerEnhanceMaxArmorAddtive : 0f)).ToString();
        m_AttributeMovement.text = (_model.F_MovementSpeed + (cultivateData.m_Enhance >= enum_PlayerCharacterEnhance.MovementSpeed ? GameConst.F_PlayerEnhanceMovementSpeedAdditive : 0f)).ToString();
        m_AttributeCritical.text = (_model.F_CriticalRate+(cultivateData .m_Enhance>= enum_PlayerCharacterEnhance.Critical?GameConst.F_PlayerEnhanceCriticalRateAdditive:0f)).ToString();
        m_AttributeWeapon.SetWeaponInfo(GameDataManager.GetWeaponProperties(GameConst.m_CharacterStartWeapon[m_SelectCharacter]),true, cultivateData.m_Enhance>= enum_PlayerCharacterEnhance.StartWeapon?1:0);

        bool unlocked = GameDataManager.CheckCharacterUnlocked(m_SelectCharacter);
        bool showUpgradeTitle = false;
        bool showStatusButton = false;
        bool maxEnhance = false;
        if(!unlocked)
        {
            showStatusButton = true;
            m_AttributeButtonTitle.localizeKey = "UI_CharacterSelect_Unlock";
            m_AttributeButtonAmount.text = GameDataManager.GetCharacterUnlockPrice(m_SelectCharacter).ToString();
        }
        else if(GameDataManager.CheckCharacterEnhancable(m_SelectCharacter))
        {
            showStatusButton = true;
            showUpgradeTitle = true;
            m_AttributeUpgradeTitle.localizeKey = cultivateData.NextEnhance().GetIntroLocalizeKey();
            m_AttributeButtonTitle.localizeKey = "UI_CharacterSelect_Enhance";
            m_AttributeButtonAmount.text = GameDataManager.GetCharacterEnhancePrice(m_SelectCharacter).ToString();
        }
        else
        {
            maxEnhance = true;
        }
        m_AttributeButton.SetActivate(showStatusButton);
        m_AttributeUpgradeTitle.SetActivate(showUpgradeTitle);
        m_AttributeMaxEnhance.SetActivate(maxEnhance);

        m_CharacterName.localizeKey = _model.GetNameLocalizeKey();
        m_CharacterIntro.localizeKey = _model.GetIntroLocalizeKey();
        m_CharacterAbility.SetAbilityInfo(m_SelectCharacter);
        m_CharacterEnhanceGrid.ClearGrid();
        for (int i = 0; i < (int)cultivateData.m_Enhance; i++)
            m_CharacterEnhanceGrid.AddItem(i);

        bool equipping = GameDataManager.m_CharacterData.m_CharacterSelected == m_SelectCharacter;
        m_CharacterConfirmBtn.SetInteractable(unlocked&&!equipping);
    }


    void OnAttributeButtonClick()
    {
        if(GameDataManager.CanCharacterUnlock(m_SelectCharacter))
        {
            GameDataManager.DoUnlockCharacter(m_SelectCharacter);
            UpdateAllCharacterInfo(m_SelectCharacter);
            return;
        }

        if(GameDataManager.CanEnhanceCharacter(m_SelectCharacter))
        {
            GameDataManager.DoEnhanceCharacter(m_SelectCharacter);
            UpdateAllCharacterInfo(m_SelectCharacter);
            return;
        }
    }

    void OnCharacterButtonClick()
    {
        if (GameDataManager.CheckCharacterEquipping(m_SelectCharacter) || !GameDataManager.CheckCharacterUnlocked(m_SelectCharacter))
            return;
        GameDataManager.DoSwitchCharacter(m_SelectCharacter);
        UpdateAllCharacterInfo(m_SelectCharacter);
    }

    protected override void OnHideFinished()
    {
        base.OnHideFinished();
        m_ModelViewer.OnGenerateCharacter(GameDataManager.m_CharacterData.m_CharacterSelected);
        CampManager.Instance.OnSetCharacter(m_ModelViewer.m_CharacterModel);
        m_ModelViewer.OnCharacterSelected();
    }
}
