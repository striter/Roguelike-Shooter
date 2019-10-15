﻿using GameSetting;
using UnityEngine;
using UnityEngine.UI;
public class UI_PlayerStatus : UIToolsBase
{
    Button btn_Bigmap;

    Transform tf_Container;
    EntityCharacterPlayer m_Player;

    Transform tf_ActionData;
    UIC_ActionAmount m_ActionAmount;
    Button btn_ActionStorage, btn_ActionShuffle;
    Slider sld_ShuffleCooldown;
    UIT_GridControllerMonoItem<UIGI_ActionHoldItem> m_ActionGrid;

    Transform tf_ExpireData;
    UIT_GridControllerMonoItem<UIGI_ExpireInfoItem> m_ExpireGrid;

    RectTransform rtf_StatusData;
    Image img_reloadFill;
    GridLayoutGroup m_AmmoLayout;
    Transform tf_AmmoData;
    Slider sld_Reload;
    float m_AmmoGridWidth;
    UIT_GridControllerMonoItem<UIGI_AmmoItem> m_AmmoGrid;
    UIC_Numeric m_AmmoAmount, m_AmmoClipAmount;

    Transform tf_ArmorData;
    Slider sld_Armor;
    UIC_Numeric m_ArmorAmount;

    Transform tf_HealthData;
    Slider sld_Health;
    UIC_Numeric m_HealthAmount, m_MaxHealth;

    enum_Interaction m_lastInteract;
    RectTransform rtf_InteractData;
    UIT_TextExtend txt_interactName;
    UIT_TextExtend txt_interactPrice;
    UIGI_ActionSelectItem m_ActionData;

    Transform tf_WeaponData;
    UIT_TextExtend m_WeaponName,m_WeaponAction;
    Image m_WeaponImage;
    UIC_RarityLevel m_WeaponActionRarity;

    Text m_Coins;
    protected override void Init()
    {
        base.Init();
        tf_Container = transform.Find("Container");

        btn_Bigmap = tf_Container.Find("Bigmap").GetComponent<Button>();
        btn_Bigmap.onClick.AddListener(() => { UIManager.Instance.ShowPage<UI_MapControl>(true); });

        rtf_StatusData = tf_Container.Find("StatusData").GetComponent<RectTransform>();
        rtf_StatusData.localScale = UIManager.Instance.m_FittedScale;
        tf_AmmoData = rtf_StatusData.Find("AmmoData");
        m_AmmoGridWidth = tf_AmmoData.GetComponent<RectTransform>().sizeDelta.x;
        m_AmmoAmount = new UIC_Numeric(tf_AmmoData.Find("AmmoAmount"));
        m_AmmoClipAmount = new UIC_Numeric(m_AmmoAmount.transform.Find("ClipAmount"));
        m_AmmoGrid = new UIT_GridControllerMonoItem<UIGI_AmmoItem>(tf_AmmoData.Find("AmmoGrid"));
        m_AmmoLayout = m_AmmoGrid.transform.GetComponent<GridLayoutGroup>();
        sld_Reload = m_AmmoGrid.transform.Find("Reload").GetComponent<Slider>();
        img_reloadFill = sld_Reload.transform.Find("Fill").GetComponent<Image>();

        tf_ArmorData = rtf_StatusData.Find("ArmorData");
        sld_Armor = tf_ArmorData.Find("ArmorSlider").GetComponent<Slider>();
        m_ArmorAmount = new UIC_Numeric(tf_ArmorData.Find("ArmorAmount"));
        tf_HealthData = rtf_StatusData.Find("HealthData");
        sld_Health = tf_HealthData.Find("HealthSlider").GetComponent<Slider>();
        m_HealthAmount = new UIC_Numeric(tf_HealthData.Find("HealthAmount"));
        m_MaxHealth = new UIC_Numeric(m_HealthAmount.transform.Find("MaxHealth"));
        
        tf_ActionData = tf_Container.Find("ActionData");
        m_ActionAmount = new UIC_ActionAmount(tf_ActionData.Find("ActionAmount"));
        m_ActionGrid = new UIT_GridControllerMonoItem<UIGI_ActionHoldItem>(tf_ActionData.Find("ActionGrid"));
        btn_ActionStorage = tf_ActionData.Find("ActionStorage").GetComponent<Button>();
        btn_ActionStorage.onClick.AddListener(OnActionStorageClick);
        btn_ActionShuffle = tf_ActionData.Find("ActionShuffle").GetComponent<Button>();
        btn_ActionShuffle.onClick.AddListener(OnActionShuffleClick);
        sld_ShuffleCooldown = btn_ActionShuffle.transform.Find("ShuffleSlider").GetComponent<Slider>();

        tf_ExpireData = tf_Container.Find("ExpireData");
        m_ExpireGrid = new UIT_GridControllerMonoItem<UIGI_ExpireInfoItem>(tf_ExpireData.Find("ExpireGrid"));

        rtf_InteractData = tf_Container.Find("InteractData").GetComponent<RectTransform>();
        txt_interactName = rtf_InteractData.Find("Container/InteractName").GetComponent<UIT_TextExtend>();
        txt_interactPrice = rtf_InteractData.Find("Container/InteractPrice").GetComponent<UIT_TextExtend>();
        m_ActionData = rtf_InteractData.Find("Container/ActionData").GetComponent<UIGI_ActionSelectItem>();
        m_ActionData.Init(null);

        tf_WeaponData = tf_Container.Find("WeaponData");
        m_WeaponName = tf_WeaponData.Find("WeaponName").GetComponent<UIT_TextExtend>();
        m_WeaponImage = tf_WeaponData.Find("WeaponImage").GetComponent<Image>();
        m_WeaponAction = tf_WeaponData.Find("WeaponAction").GetComponent<UIT_TextExtend>();
        m_WeaponActionRarity = new UIC_RarityLevel(tf_WeaponData.Find("WeaponActionRarity"));
        tf_WeaponData.Find("WeaponDetailBtn").GetComponent<Button>().onClick.AddListener(() => { UIManager.Instance.ShowPage<UI_WeaponStatus>(true,.1f).SetInfo(m_Player.m_WeaponCurrent); });

        m_Coins = tf_Container.Find("CoinData/Data").GetComponent<Text>();
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireStatus, OnExpireStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireStatus, OnExpireStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<WeaponBase>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    public void SetInGame(bool inGame)
    {
        btn_Bigmap.SetActivate(inGame);
        btn_ActionShuffle.SetActivate(false);
        btn_ActionStorage.SetActivate(inGame);
    }
    
    void OnBattleStart()
    {
        btn_Bigmap.SetActivate(false);
        btn_ActionShuffle.SetActivate(true);
        btn_ActionStorage.SetActivate(false);
    }
    void OnBattleFinish()
    {
        btn_Bigmap.SetActivate(true);
        btn_ActionShuffle.SetActivate(false);
        btn_ActionStorage.SetActivate(true);
    }
    #region PlayerData/Interact
    void OnCommonStatus(EntityCharacterPlayer _player)
    {
        if (!m_Player)
            m_Player = _player;

        m_Coins.text=_player.m_PlayerInfo.m_Coins.ToString();
        m_ActionAmount.SetValue(_player.m_PlayerInfo.m_ActionAmount);
        sld_ShuffleCooldown.value = _player.m_PlayerInfo.f_shuffleScale;
        rtf_StatusData.SetWorldViewPortAnchor(m_Player.tf_Head.position, CameraController.Instance.m_Camera, Time.deltaTime * 10f);

        if (_player.m_Interact==null)
        {
            rtf_InteractData.SetActivate(false);
            m_lastInteract = enum_Interaction.Invalid;
            return;
        }

        if (rtf_InteractData.SetActivate(true))
            rtf_InteractData.SetWorldViewPortAnchor(m_Player.tf_Head.position, CameraController.Instance.m_Camera);
        rtf_InteractData.SetWorldViewPortAnchor(m_Player.tf_Head.position, CameraController.Instance.m_Camera, Time.deltaTime * 10f);

        int tradePrice = 0;
        if (_player.m_Interact.m_InteractType == enum_Interaction.ContainerTrade)
        {
            InteractContainerTrade trade = _player.m_Interact as InteractContainerTrade;
            tradePrice = trade.m_TradePrice;
            SetInteractInfo(trade.m_InteractTarget);
        }
        else if (_player.m_Interact.m_InteractType == enum_Interaction.ContainerBattle)
        {
            InteractContainerBattle battle = _player.m_Interact as InteractContainerBattle;
            SetInteractInfo(battle.m_InteractTarget);
        }
        else
        {
            SetInteractInfo(_player.m_Interact);
        }
        txt_interactPrice.SetActivate(tradePrice>0);
        if(tradePrice>0)
            txt_interactPrice.text = tradePrice.ToString();
    }
    WeaponBase m_targetInteractWeapon;
    void SetInteractInfo(InteractBase interact)
    {
        if (interact.m_InteractType == enum_Interaction.Weapon)
        {
            SetInteractWeaponInfo(interact as InteractWeapon);
            return;
        }
        m_targetInteractWeapon = null;

        if (m_lastInteract == interact.m_InteractType)
            return;
        m_lastInteract = interact.m_InteractType;
        m_ActionData.SetActivate(false);
        switch (interact.m_InteractType)
        {
            case enum_Interaction.PickupAction:
                {
                    m_ActionData.SetActivate(true);
                    txt_interactName.localizeText = interact.m_InteractType.GetLocalizeKey();
                    m_ActionData.SetInfo((interact as InteractPickupAction).m_Action);
                }
                break;
            default:
                txt_interactName.localizeText = interact.m_InteractType.GetLocalizeKey();
                break;
        }
    }
    void SetInteractWeaponInfo(InteractWeapon interactWeapon)
    {
        if (m_targetInteractWeapon == interactWeapon.m_Weapon)
            return;

        m_targetInteractWeapon = interactWeapon.m_Weapon;
        txt_interactName.localizeText = m_targetInteractWeapon.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();
        m_lastInteract = enum_Interaction.Invalid;
        m_ActionData.SetActivate(m_targetInteractWeapon.m_WeaponAction.Count > 0);      //Test
        if (m_targetInteractWeapon.m_WeaponAction.Count > 0)        //Test
            m_ActionData.SetInfo(m_targetInteractWeapon.m_WeaponAction[0]);     //Test????
    }
    #endregion
    #region Health Status
    void OnHealthStatus(EntityHealth _healthManager)
    {
        sld_Armor.value = _healthManager.m_CurrentArmor/UIConst.F_UIMaxArmor;
        sld_Health.value = _healthManager.F_MaxHealthValue;
        m_ArmorAmount.SetAmount((int)_healthManager.m_CurrentArmor);
        m_HealthAmount.SetAmount((int)_healthManager.m_CurrentHealth);
        m_MaxHealth.SetAmount((int)_healthManager.m_MaxHealth);
    }
    #endregion
    #region Weapon/Ammo
    void OnWeaponStatus(WeaponBase weaponInfo)
    {
        m_WeaponName.localizeText = weaponInfo.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();

        bool showWeaponAction = weaponInfo.m_WeaponAction.Count == 1;
        m_WeaponAction.SetActivate(showWeaponAction);
        m_WeaponActionRarity.transform.SetActivate(showWeaponAction);
        if (!showWeaponAction) return;
        m_WeaponAction.localizeText = weaponInfo.m_WeaponAction[0].GetNameLocalizeKey();
        m_WeaponActionRarity.SetLevel(weaponInfo.m_WeaponAction[0].m_rarity);
    }
    void OnAmmoStatus(WeaponBase weaponInfo)
    {
        tf_AmmoData.transform.SetActivate(weaponInfo != null);
        if (weaponInfo == null)
            return;

        m_AmmoAmount.SetAmount(weaponInfo.B_Reloading?0:weaponInfo.I_AmmoLeft);
        m_AmmoClipAmount.SetAmount(weaponInfo.I_ClipAmount);
        if (m_AmmoGrid.I_Count != weaponInfo.I_ClipAmount)
        {
            m_AmmoGrid.ClearGrid();
            if (weaponInfo.I_ClipAmount <= UIConst.I_AmmoCountToSlider)
            {
                for (int i = 0; i < weaponInfo.I_ClipAmount; i++)
                    m_AmmoGrid.AddItem(i);

                float size = (m_AmmoGridWidth - m_AmmoLayout.padding.right - m_AmmoLayout.padding.left - (weaponInfo.I_ClipAmount - 1) * m_AmmoLayout.spacing.x) / weaponInfo.I_ClipAmount;
                m_AmmoLayout.cellSize = new Vector2(size, m_AmmoLayout.cellSize.y);
            }
        }


        Color ammoStatusColor = weaponInfo.F_AmmoStatus < .5f ? Color.Lerp(Color.red, Color.white, (weaponInfo.F_AmmoStatus / .5f)) : Color.white;
        if (weaponInfo.I_ClipAmount <= UIConst.I_AmmoCountToSlider)
        {
            for (int i = 0; i < weaponInfo.I_ClipAmount; i++)
                m_AmmoGrid.GetItem(i).Set((weaponInfo.B_Reloading || i > weaponInfo.I_AmmoLeft - 1) ? new Color(0, 0, 0, 1) : ammoStatusColor);
            sld_Reload.value = 0;
        }
        else
        {
            sld_Reload.value = weaponInfo.F_AmmoStatus;
            img_reloadFill.color = ammoStatusColor;
        }

        if (weaponInfo.B_Reloading)
        {
            sld_Reload.value = weaponInfo.F_ReloadStatus;
            img_reloadFill.color = Color.Lerp(Color.red, Color.white, weaponInfo.F_ReloadStatus);
        }
    }
    #endregion
    #region Action/Expire
    void OnActionStatus(PlayerInfoManager actionInfo)
    {
        m_ActionGrid.ClearGrid();
        for (int i = 0; i < actionInfo.m_ActionHolding.Count; i++)
            m_ActionGrid.AddItem(i).SetInfo(actionInfo.m_ActionHolding[i],OnActionClick, OnActionPressDuration);
    }
    void OnActionClick(int index)
    {
        m_Player.m_PlayerInfo.TryUseAction(index);
    }
    void OnActionPressDuration()
    {
        UIManager.Instance.ShowPage<UI_ActionStorage>(false,0f).Show(false,m_Player.m_PlayerInfo) ;
    }
    void OnActionStorageClick()
    {
        UIManager.Instance.ShowPage<UI_ActionStorage>(true).Show(true,m_Player.m_PlayerInfo);
    }
    void OnActionShuffleClick()
    {
        m_Player.m_PlayerInfo.TryShuffle();
    }
    void OnExpireStatus(PlayerInfoManager expireInfo)
    {
        m_ExpireGrid.ClearGrid();
        for (int i = 0; i < expireInfo.m_Expires.Count; i++)
        {
            if (expireInfo.m_Expires[i].m_ExpireType == enum_ExpireType.Action&& (expireInfo.m_Expires[i] as ActionBase).m_ActionExpireType == enum_ActionType.WeaponPerk)
                    continue;

            m_ExpireGrid.AddItem(i).SetInfo(expireInfo.m_Expires[i]);
        }
    }
    #endregion
}
