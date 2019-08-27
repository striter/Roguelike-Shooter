using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStatus : SimpleSingletonMono<UI_PlayerStatus>
{
    Transform tf_Container,tf_Center,tf_Left;
    Text  txt_Health, txt_Armor,txt_ActionAmount;
    Slider sld_Reload;
    Image img_sld;
    UIT_GridControllerMono<UIGI_AmmoItem> m_AmmoGrid;
    UIT_GridControllerMono<UIGI_ActionHoldItem> m_ActionGrid;
    GridLayoutGroup m_GridLayout;
    EntityPlayerBase m_Player;
    float f_ammoGridLength;
    protected override void Awake()
    {
        base.Awake();
        tf_Container = transform.Find("Container");
        tf_Center = tf_Container.Find("Center");
        txt_Health = tf_Center.Find("Health").GetComponent<Text>();
        txt_Armor = tf_Center.Find("Armor").GetComponent<Text>();
        m_AmmoGrid = new UIT_GridControllerMono<UIGI_AmmoItem>(tf_Center.Find("AmmoGrid"));
        m_GridLayout = m_AmmoGrid.transform.GetComponent<GridLayoutGroup>();
        f_ammoGridLength = m_AmmoGrid.transform.GetComponent<RectTransform>().sizeDelta.y;
        sld_Reload = m_AmmoGrid.transform.Find("Reload").GetComponent<Slider>();
        img_sld = sld_Reload.transform.Find("Fill").GetComponent<Image>();

        tf_Left = tf_Container.Find("Left");
        m_ActionGrid =new UIT_GridControllerMono<UIGI_ActionHoldItem>(tf_Left.Find("ActionGrid"));
        txt_ActionAmount = m_ActionGrid.transform.Find("ActionAmount").GetComponent<Text>();
    }
    private void Start()
    {
        TBroadCaster<enum_BC_UIStatus>.Add<EntityPlayerBase>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireStatus, OnExpireStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerActionManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionStatus);
    }
    protected void OnDestroy()
    {
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityPlayerBase>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireStatus, OnExpireStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerActionManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionStatus);
    }
    void OnCommonStatus(EntityPlayerBase _player)
    {
        if (!m_Player)
            m_Player = _player;
        txt_ActionAmount.text = _player.m_PlayerActions.m_ActionHodling.Count == 0 ? _player.m_PlayerActions.m_ActionStored.Count.ToString() : _player.m_PlayerActions.m_ActionAmount.ToString();
        tf_Center.position = Vector3.Lerp(tf_Center.position, CameraController.MainCamera.WorldToScreenPoint(m_Player.tf_Head.position), Time.deltaTime * 10f);
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

    void OnActionStatus(PlayerActionManager actionInfo)
    {
        m_ActionGrid.ClearGrid();
        for (int i = 0; i < actionInfo.m_ActionHodling.Count; i++)
            m_ActionGrid.AddItem(i).SetInfo(actionInfo.m_ActionHodling[i],OnActionUse);
    }
    void OnActionUse(int index)
    {
        m_Player.m_PlayerActions.TryUseAction(index);
    }

    void OnExpireStatus(PlayerInfoManager expireInfo)
    {

    }
}
