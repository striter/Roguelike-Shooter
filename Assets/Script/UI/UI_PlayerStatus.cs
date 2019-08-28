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
    GridLayoutGroup m_GridLayout;
    EntityPlayerBase m_Player;
    float f_ammoGridLength;

    Transform tf_WeaponData;
    WeaponBase m_targetWeapon;
    Text txt_WeaponName;
    UIGI_ActionSelectItem m_weaponActionData;
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

        tf_WeaponData = tf_Container.Find("WeaponData");
        txt_WeaponName = tf_WeaponData.Find("Container/WeaponName").GetComponent<Text>();
        m_weaponActionData = tf_WeaponData.Find("Container/ActionData").GetComponent<UIGI_ActionSelectItem>();
        m_weaponActionData.SetGridControlledItem(0, null);
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
    void OnCommonStatus(EntityPlayerBase _player)
    {
        if (!m_Player)
            m_Player = _player;
        txt_ActionAmount.text = _player.m_PlayerInfo.m_ActionHolding.Count == 0 ? _player.m_PlayerInfo.m_ActionStored.Count.ToString() : _player.m_PlayerInfo.m_ActionAmount.ToString();
        tf_PlayerData.position = Vector3.Lerp(tf_PlayerData.position, CameraController.MainCamera.WorldToScreenPoint(m_Player.tf_Head.position), Time.deltaTime * 10f);

        
        if (_player.m_Interact==null||_player.m_Interact.m_InteractType != enum_Interaction.WeaponContainer)
        {
            m_targetWeapon = null;
            tf_WeaponData.SetActivate(false);
            return;
        }
        if (tf_WeaponData.SetActivate(true))
            tf_WeaponData.position = CameraController.MainCamera.WorldToScreenPoint(_player.m_Interact.transform.position);
        tf_WeaponData.position = Vector3.Lerp(tf_WeaponData.position, CameraController.MainCamera.WorldToScreenPoint(_player.m_Interact.transform.position), Time.deltaTime * 10f);

        WeaponBase targetWeapon = (_player.m_Interact as InteractWeaponContainer).m_Weapon;
        if (m_targetWeapon == targetWeapon)
            return;

        m_targetWeapon = targetWeapon;
        txt_WeaponName.text = m_targetWeapon.m_WeaponInfo.m_Weapon.GetNameLocalizeKey();

        if (m_targetWeapon.m_WeaponAction.Count > 0)
        {
            m_weaponActionData.SetInfo(m_targetWeapon.m_WeaponAction[0]);
            m_weaponActionData.SetActivate(true);
        }
        else
            m_weaponActionData.SetActivate(false);
    }

    void OnHealthStatus(EntityHealth _healthManager)
    {
        txt_Health.text = ((int)_healthManager.m_CurrentHealth).ToString() + "/" + ((int)_healthManager.m_MaxHealth).ToString();
        txt_Armor.text = ((int)_healthManager.m_CurrentArmor).ToString() + "/" + ((int)_healthManager.m_DefaultArmor).ToString();
    }

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

    }
}
