﻿using GameSetting;
using UnityEngine;
using UnityEngine.UI;
using TSpecialClasses;
public class UIC_PlayerStatus : UIControlBase
{
    Transform tf_Container;
    Animation m_Animation;
    EntityCharacterPlayer m_Player;

    Transform tf_OutBattle;
    Button btn_Bigmap;
    Button btn_ActionStorage;

    Transform tf_InBattle;
    UIC_ActionEnergy m_ActionEnergy;
    Button  btn_ActionShuffle;
    Image img_ShuffleFill;
    UIT_GridControllerGridItem<UIGI_ActionItemHold> m_ActionGrid;

    Transform tf_ExpireData;
    UIT_GridControllerGridItem<UIGI_ExpireInfoItem> m_ExpireGrid;

    RectTransform rtf_StatusData;
    GridLayoutGroup m_AmmoLayout;
    Transform tf_AmmoData;
    Image img_ReloadFill;
    float m_AmmoGridWidth;
    UIT_GridControllerGridItem<UIGI_AmmoItem> m_AmmoGrid;
    UIC_Numeric m_AmmoAmount, m_AmmoClipAmount;

    Transform tf_ArmorData;
    Image m_ArmorFill;
    UIC_Numeric m_ArmorAmount;

    Transform tf_HealthData;
    Image m_HealthFill;
    UIC_Numeric m_HealthAmount, m_MaxHealth;

    enum_Interaction m_lastInteract;
    RectTransform rtf_InteractData;
    UIT_TextExtend txt_interactName;
    UIT_TextExtend txt_interactPrice;
    UIGI_ActionSelectItem m_ActionData;

    Transform tf_WeaponData;
    UIT_TextExtend m_WeaponName;
    Image m_WeaponImage;
    UIT_TextExtend m_WeaponActionName;
    Transform tf_WeaponActionDetail;
    UIC_RarityLevel_BG m_WeaponActionRarity;

    DurationLerp m_HealthLerp, m_ArmorLerp, m_EnergyLerp;
    protected override void Init()
    {
        base.Init();
        tf_Container = transform.Find("Container");
        m_Animation = tf_Container.GetComponent<Animation>();

        tf_OutBattle = tf_Container.Find("OutBattle");
        btn_ActionStorage = tf_OutBattle.Find("ActionStorage").GetComponent<Button>();
        btn_ActionStorage.onClick.AddListener(OnActionStorageClick);
        btn_Bigmap = tf_OutBattle.Find("Bigmap").GetComponent<Button>();
        btn_Bigmap.onClick.AddListener(OnMapControlClick  );

        tf_InBattle = tf_Container.Find("InBattle");
        m_ActionEnergy = new UIC_ActionEnergy(tf_InBattle.Find("ActionEnergy"));
        m_ActionGrid = new UIT_GridControllerGridItem<UIGI_ActionItemHold>(tf_InBattle.Find("ActionGrid"));
        btn_ActionShuffle = tf_InBattle.Find("ActionShuffle").GetComponent<Button>();
        btn_ActionShuffle.onClick.AddListener(OnActionShuffleClick);
        img_ShuffleFill = btn_ActionShuffle.transform.Find("ShuffleFill").GetComponent<Image>();

        rtf_StatusData = tf_Container.Find("StatusData").GetComponent<RectTransform>();
        rtf_StatusData.localScale = UIManagerBase.m_FitScale;
        tf_AmmoData = rtf_StatusData.Find("Container/AmmoData");
        m_AmmoGridWidth = tf_AmmoData.GetComponent<RectTransform>().sizeDelta.x;
        m_AmmoAmount = new UIC_Numeric(tf_AmmoData.Find("AmmoAmount"));
        m_AmmoClipAmount = new UIC_Numeric(m_AmmoAmount.transform.Find("ClipAmount"));
        m_AmmoGrid = new UIT_GridControllerGridItem<UIGI_AmmoItem>(tf_AmmoData.Find("AmmoGrid"));
        m_AmmoLayout = m_AmmoGrid.transform.GetComponent<GridLayoutGroup>();
        img_ReloadFill = m_AmmoGrid.transform.Find("Reload").GetComponent<Image>();

        tf_ArmorData = rtf_StatusData.Find("Container/ArmorData");
        m_ArmorFill = tf_ArmorData.Find("Fill").GetComponent<Image>();
        m_ArmorAmount = new UIC_Numeric(tf_ArmorData.Find("ArmorAmount"));
        tf_HealthData = rtf_StatusData.Find("Container/HealthData");
        m_HealthFill = tf_HealthData.Find("Fill").GetComponent<Image>();
        m_HealthAmount = new UIC_Numeric(tf_HealthData.Find("HealthAmount"));
        m_MaxHealth = new UIC_Numeric(m_HealthAmount.transform.Find("MaxHealth"));
        
        tf_ExpireData = tf_Container.Find("ExpireData");
        m_ExpireGrid = new UIT_GridControllerGridItem<UIGI_ExpireInfoItem>(tf_ExpireData.Find("ExpireGrid"));

        rtf_InteractData = tf_Container.Find("InteractData").GetComponent<RectTransform>();
        txt_interactName = rtf_InteractData.Find("Container/InteractName").GetComponent<UIT_TextExtend>();
        txt_interactPrice = rtf_InteractData.Find("Container/InteractPrice").GetComponent<UIT_TextExtend>();
        m_ActionData = rtf_InteractData.Find("Container/ActionData").GetComponent<UIGI_ActionSelectItem>();
        m_ActionData.Init();

        tf_WeaponData = tf_Container.Find("WeaponData");
        m_WeaponName = tf_WeaponData.Find("WeaponName").GetComponent<UIT_TextExtend>();
        m_WeaponImage = tf_WeaponData.Find("WeaponImage").GetComponent<Image>();
        m_WeaponActionName = tf_WeaponData.Find("ActionName").GetComponent<UIT_TextExtend>();
        tf_WeaponActionDetail = tf_WeaponData.Find("ActionDetail");
        m_WeaponActionRarity = new UIC_RarityLevel_BG(tf_WeaponActionDetail.Find("ActionRarity"));
        tf_WeaponData.Find("WeaponDetailBtn").GetComponent<Button>().onClick.AddListener(() => { UIManager.Instance.ShowPage<UI_WeaponStatus>(true,0f).SetInfo(m_Player.m_WeaponCurrent); });

        m_HealthLerp = new DurationLerp(0f, .25f);
        m_ArmorLerp = new DurationLerp(0f, .25f);
        m_EnergyLerp = new DurationLerp(0f, .5f);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireStatus, OnExpireStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);

        SetInBattle(false,false);
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
        if (inGame)
            return;
        tf_InBattle.SetActivate(false);
        tf_OutBattle.SetActivate(false);
    }

    void SetInBattle(bool inBattle,bool anim=true)
    {
        m_ActionGrid.ClearGrid();
        btn_Bigmap.interactable = !inBattle;
        btn_ActionStorage.interactable = !inBattle;
        btn_ActionShuffle.interactable = inBattle;
        if (!anim)
            return;
        m_Animation[m_Animation.clip.name].speed = inBattle ? 1 : -1;
        m_Animation[m_Animation.clip.name].normalizedTime = inBattle ? 0 : 1;
        m_Animation.Play(m_Animation.clip.name);
    }
    void OnBattleStart()=>SetInBattle(true);
    void OnBattleFinish()=> SetInBattle(false);


    void OnMapControlClick()
    {
        if (GameManager.Instance.B_ShowChestTips)
            UIManager.Instance.ShowTip("UI_Tips_ChestUnOpened", enum_UITipsType.Error);

        UIManager.Instance.ShowPage<UI_MapControl>(true);
    }
    #region PlayerData/Interact
    private void Update()
    {
        if (!m_Player)
            return;

        if (m_EnergyLerp.TickLerp(Time.deltaTime))
            m_ActionEnergy.SetValue(m_EnergyLerp.m_value);
        if (m_HealthLerp.TickLerp(Time.deltaTime))
            m_HealthFill.fillAmount = m_HealthLerp.m_value;
        if (m_ArmorLerp.TickLerp(Time.deltaTime))
            m_ArmorFill.fillAmount = m_ArmorLerp.m_value;
    }

    void OnCommonStatus(EntityCharacterPlayer _player)
    {
        if (!m_Player)
            m_Player = _player;

        m_EnergyLerp.ChangeValue(_player.m_PlayerInfo.m_ActionEnergy);
        img_ShuffleFill.fillAmount = _player.m_PlayerInfo.f_shuffleScale;
        rtf_StatusData.SetWorldViewPortAnchor(m_Player.tf_Status.position, CameraController.Instance.m_Camera, Time.deltaTime * 10f);

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
                    txt_interactName.autoLocalizeText = interact.m_InteractType.GetLocalizeKey();
                    m_ActionData.SetInfo((interact as InteractPickupAction).m_Action);
                }
                break;
            default:
                txt_interactName.autoLocalizeText = interact.m_InteractType.GetLocalizeKey();
                break;
        }
    }
    void SetInteractWeaponInfo(InteractWeapon interactWeapon)
    {
        if (m_targetInteractWeapon == interactWeapon.m_Weapon)
            return;

        m_targetInteractWeapon = interactWeapon.m_Weapon;
        txt_interactName.autoLocalizeText = m_targetInteractWeapon.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();
        m_lastInteract = enum_Interaction.Invalid;
        m_ActionData.SetActivate(m_targetInteractWeapon.m_WeaponAction != null);
        if (m_targetInteractWeapon.m_WeaponAction!=null)
            m_ActionData.SetInfo(m_targetInteractWeapon.m_WeaponAction);
    }
    #endregion
    #region Health Status
    void OnHealthStatus(EntityHealth _healthManager)
    {
        m_ArmorLerp.ChangeValue(_healthManager.m_CurrentArmor / UIConst.F_UIMaxArmor);
        m_HealthLerp.ChangeValue(_healthManager.F_HealthMaxScale);
        m_ArmorAmount.SetAmount((int)_healthManager.m_CurrentArmor);
        m_HealthAmount.SetAmount((int)_healthManager.m_CurrentHealth);
        m_MaxHealth.SetAmount((int)_healthManager.m_MaxHealth);
    }
    #endregion
    #region Weapon/Ammo
    void OnWeaponStatus(WeaponBase weaponInfo)
    {
        m_WeaponName.autoLocalizeText = weaponInfo.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();

        bool showWeaponAction = weaponInfo.m_WeaponAction != null;
        tf_WeaponActionDetail.SetActivate(showWeaponAction);
        m_WeaponActionName.autoLocalizeText = showWeaponAction ? weaponInfo.m_WeaponAction.GetNameLocalizeKey() : "UI_WeaponStatus_ActionInvalidName";
        if (showWeaponAction) m_WeaponActionRarity.SetLevel(weaponInfo.m_WeaponAction.m_rarity);
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
            img_ReloadFill.fillAmount = 0;
        }
        else
        {
            img_ReloadFill.fillAmount = weaponInfo.F_AmmoStatus;
            img_ReloadFill.color = ammoStatusColor;
        }

        if (weaponInfo.B_Reloading)
        {
            img_ReloadFill.fillAmount = weaponInfo.F_ReloadStatus;
            img_ReloadFill.color = Color.Lerp(Color.red, Color.white, weaponInfo.F_ReloadStatus);
        }
    }
    #endregion
    #region Action/Expire
    void OnActionStatus(PlayerInfoManager playerInfo)
    {
        m_ActionGrid.ClearGrid();
        for (int i = 0; i < playerInfo.m_BattleActionPicking.Count; i++)
            m_ActionGrid.AddItem(i).SetInfo(playerInfo,playerInfo.m_BattleActionPicking[i],OnActionClick, OnActionPressDuration);
    }
    void OnActionClick(int index)
    {
        m_Player.m_PlayerInfo.TryUseHoldingAction(index);
    }
    void OnActionPressDuration()
    {
        UIManager.Instance.ShowPage<UI_ActionBattle>(false,0f).Show(m_Player.m_PlayerInfo,m_ActionEnergy) ;
    }
    void OnActionStorageClick()
    {
        UIManager.Instance.ShowPage<UI_ActionPack>(true).Show(m_Player.m_PlayerInfo);
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
            if (expireInfo.m_Expires[i].m_ExpireType == enum_ExpireType.Action&& (expireInfo.m_Expires[i] as ActionBase).m_ActionType == enum_ActionType.WeaponPerk)
                    continue;

            m_ExpireGrid.AddItem(i).SetInfo(expireInfo.m_Expires[i]);
        }
    }
    #endregion
}