using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStatus : SimpleSingletonMono<UI_PlayerStatus>
{
    Transform tf_Container,tf_PlayerData,tf_Left;
    Text  txt_Health, txt_Armor,txt_ActionAmount;
    Button btn_ActionStorage;
    Slider sld_Reload;
    Image img_sld;
    UIT_GridControllerMono<UIGI_AmmoItem> m_AmmoGrid;
    UIT_GridControllerMono<UIGI_ActionHoldItem> m_ActionGrid;
    UIT_GridControllerMono<UIGI_ExpireInfoItem> m_ExpireGrid;
    GridLayoutGroup m_GridLayout;
    EntityPlayerBase m_Player;
    float f_ammoGridLength;

    enum_Interaction m_lastInteract;
    Transform tf_InteractData;
    UIT_TextLocalization txt_interactName;
    UIT_TextLocalization txt_interactPrice;
    UIGI_ActionSelectItem m_ActionData;
    protected override void Awake()
    {
        base.Awake();
        tf_Container = transform.Find("Container");
        tf_PlayerData = tf_Container.Find("PlayerData");
        txt_Health = tf_PlayerData.Find("Health").GetComponent<Text>();
        txt_Armor = tf_PlayerData.Find("Armor").GetComponent<Text>();
        m_AmmoGrid = new UIT_GridControllerMono<UIGI_AmmoItem>(tf_PlayerData.Find("AmmoGrid"));
        m_GridLayout = m_AmmoGrid.transform.GetComponent<GridLayoutGroup>();
        f_ammoGridLength = m_AmmoGrid.transform.GetComponent<RectTransform>().sizeDelta.y;
        sld_Reload = m_AmmoGrid.transform.Find("Reload").GetComponent<Slider>();
        img_sld = sld_Reload.transform.Find("Fill").GetComponent<Image>();

        tf_Left = tf_Container.Find("Left");
        m_ActionGrid =new UIT_GridControllerMono<UIGI_ActionHoldItem>(tf_Left.Find("ActionGrid"));
        txt_ActionAmount = m_ActionGrid.transform.Find("ActionAmount").GetComponent<Text>();
        btn_ActionStorage = m_ActionGrid.transform.Find("ActionStorage").GetComponent<Button>();
        btn_ActionStorage.onClick.AddListener(OnActionStorageClick);
        m_ExpireGrid = new UIT_GridControllerMono<UIGI_ExpireInfoItem>(tf_Left.Find("ExpireGrid"));

        tf_InteractData = tf_Container.Find("InteractData");
        txt_interactName = tf_InteractData.Find("Container/InteractName").GetComponent<UIT_TextLocalization>();
        txt_interactPrice = tf_InteractData.Find("Container/InteractPrice").GetComponent<UIT_TextLocalization>();
        m_ActionData = tf_InteractData.Find("Container/ActionData").GetComponent<UIGI_ActionSelectItem>();
        m_ActionData.SetGridControlledItem(0, null);
    }
    private void Start()
    {
        TBroadCaster<enum_BC_UIStatus>.Add<EntityPlayerBase>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireStatus, OnExpireStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionStatus);
    }
    protected void OnDestroy()
    {
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityPlayerBase>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireStatus, OnExpireStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionStatus);
    }
    #region PlayerData/Interact
    void OnCommonStatus(EntityPlayerBase _player)
    {
        if (!m_Player)
            m_Player = _player;
        txt_ActionAmount.text = _player.m_PlayerInfo.m_ActionHolding.Count == 0 ? _player.m_PlayerInfo.m_ActionStored.Count.ToString() : _player.m_PlayerInfo.m_ActionAmount.ToString();
        tf_PlayerData.position = Vector3.Lerp(tf_PlayerData.position, CameraController.MainCamera.WorldToScreenPoint(m_Player.tf_Head.position), Time.deltaTime * 10f);
        
        if (_player.m_Interact==null)
        {
            tf_InteractData.SetActivate(false);
            m_lastInteract = enum_Interaction.Invalid;
            return;
        }

        if (tf_InteractData.SetActivate(true))
            tf_InteractData.position = CameraController.MainCamera.WorldToScreenPoint(_player.m_Interact.transform.position);
            tf_InteractData.position = Vector3.Lerp(tf_InteractData.position, CameraController.MainCamera.WorldToScreenPoint(_player.m_Interact.transform.position), Time.deltaTime * 10f);

        bool isTradeItem = _player.m_Interact.m_InteractType == enum_Interaction.Trade;
        txt_interactPrice.SetActivate(isTradeItem);
        if (isTradeItem)
        {
            txt_interactPrice.text = (_player.m_Interact as InteractTrade).m_TradePrice.ToString();
            SetInteractInfo((_player.m_Interact as InteractTrade).m_InteractTarget);
        }
        else
        {
            SetInteractInfo(_player.m_Interact);
        }
    }
    WeaponBase m_targetWeapon;
    void SetInteractInfo(InteractBase interact)
    {
        if (interact.m_InteractType == enum_Interaction.Weapon)
        {
            SetInteractWeaponInfo(interact as InteractWeapon);
            return;
        }

        if (m_lastInteract == interact.m_InteractType)
            return;
        m_lastInteract = interact.m_InteractType;
        m_ActionData.SetActivate(false);
        switch (interact.m_InteractType)
        {
            case enum_Interaction.Action:
                {
                    m_ActionData.SetActivate(true);
                    txt_interactName.localizeText = interact.m_InteractType.GetInteractTitle();
                    m_ActionData.SetInfo((interact as InteractAction).m_Action);
                }
                break;
            default:
                txt_interactName.localizeText = interact.m_InteractType.GetInteractTitle();
                break;
        }
    }
    void SetInteractWeaponInfo(InteractWeapon interactWeapon)
    {
        if (m_targetWeapon == interactWeapon.m_Weapon)
            return;
        m_targetWeapon = interactWeapon.m_Weapon;
        txt_interactName.localizeText = m_targetWeapon.m_WeaponInfo.m_Weapon.GetNameLocalizeKey();
        m_lastInteract = enum_Interaction.Invalid;
        m_ActionData.SetActivate(m_targetWeapon.m_WeaponAction.Count > 0);      //Test
        if (m_targetWeapon.m_WeaponAction.Count > 0)        //Test
            m_ActionData.SetInfo(m_targetWeapon.m_WeaponAction[0]);     //Test????
    }
    #endregion
    #region Health
    void OnHealthStatus(EntityHealth _healthManager)
    {
        txt_Health.text = ((int)_healthManager.m_CurrentHealth).ToString() + "/" + ((int)_healthManager.m_MaxHealth).ToString();
        txt_Armor.text = ((int)_healthManager.m_CurrentArmor).ToString() + "/" + ((int)_healthManager.m_DefaultArmor).ToString();
    }
    #endregion
    #region Ammo
    void OnAmmoStatus(WeaponBase weaponInfo)
    {
        m_AmmoGrid.transform.SetActivate(weaponInfo != null);
        if (weaponInfo == null)
            return;

        if (weaponInfo.B_Reloading)
        {
            sld_Reload.value = weaponInfo.F_ReloadStatus;
            m_AmmoGrid.ClearGrid();
            return;
        }

        img_sld.color = Color.Lerp(Color.red, Color.white, weaponInfo.F_ReloadStatus);
        if (m_AmmoGrid.I_Count != weaponInfo.I_ClipAmount)
        {
            m_AmmoGrid.ClearGrid();
            if (weaponInfo.I_ClipAmount <= 30)
            {
                for (int i = 0; i < weaponInfo.I_ClipAmount; i++)
                    m_AmmoGrid.AddItem(i);

                float size = (f_ammoGridLength - m_GridLayout.padding.bottom - m_GridLayout.padding.top - (weaponInfo.I_ClipAmount - 1) * m_GridLayout.spacing.y) / weaponInfo.I_ClipAmount;
                m_GridLayout.cellSize = new Vector2(m_GridLayout.cellSize.x, size);
            }
        }

        Color ammoStatusColor = weaponInfo.F_AmmoStatus < .5f ? Color.Lerp(Color.red, Color.white, (weaponInfo.F_AmmoStatus / .5f)) : Color.white;
        if (weaponInfo.I_ClipAmount <= 30)
        {
            for (int i = 0; i < weaponInfo.I_ClipAmount; i++)
                m_AmmoGrid.GetItem(i).Set((weaponInfo.B_Reloading || i > weaponInfo.I_AmmoLeft - 1) ? new Color(0, 0, 0, 0) : ammoStatusColor);
            sld_Reload.value = 0;
        }
        else
        {
            sld_Reload.value = weaponInfo.F_AmmoStatus;
            img_sld.color = ammoStatusColor;
        }
    }
    #endregion
    #region Action/Expire
    void OnActionStatus(PlayerInfoManager actionInfo)
    {
        m_ActionGrid.ClearGrid();
        for (int i = 0; i < actionInfo.m_ActionHolding.Count; i++)
            m_ActionGrid.AddItem(i).SetInfo(actionInfo.m_ActionHolding[i],OnActionUse);

        btn_ActionStorage.SetActivate(actionInfo.m_ActionHolding.Count==0);
    }
    void OnActionUse(int index)
    {
        m_Player.m_PlayerInfo.TryUseAction(index);
    }
    void OnActionStorageClick()
    {
        UIManager.Instance.ShowPage<UI_ActionStorage>(true).Show(m_Player.m_PlayerInfo.m_ActionStored);
    }
    void OnExpireStatus(PlayerInfoManager expireInfo)
    {
        m_ExpireGrid.ClearGrid();
        for (int i = 0; i < expireInfo.m_Expires.Count; i++)
            m_ExpireGrid.AddItem(i).SetInfo(expireInfo.m_Expires[i]);
    }
    #endregion
}
