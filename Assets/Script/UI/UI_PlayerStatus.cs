using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStatus : SimpleSingletonMono<UI_PlayerStatus>
{
    Transform tf_Container,tf_Center,tf_Left;
    Text  txt_Health, txt_Armor;
    Slider sld_Reload;
    Image img_sld;
    UIT_GridControllerMono<UIGI_AmmoItem> m_AmmoGrid;
    UIT_GridControllerMono<UIGI_ActionItem> m_ActionGrid;
    EntityPlayerBase m_player;
    GridLayoutGroup m_GridLayout;
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
        m_ActionGrid =new UIT_GridControllerMono<UIGI_ActionItem>(tf_Left.Find("ActionGrid"));
    }
    private void Start()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityPlayerBase>(enum_BC_GameStatusChanged.PlayerInfoChanged, OnPlayerStatusChanegd);
    }
    protected void OnDestroy()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityPlayerBase>(enum_BC_GameStatusChanged.PlayerInfoChanged, OnPlayerStatusChanegd);
    }
    void OnPlayerStatusChanegd(EntityPlayerBase player)
    {
        m_player = player;
        txt_Health.text = ((int)player.m_HealthManager.m_CurrentHealth).ToString() + "/" + ((int)player.m_HealthManager.m_MaxHealth).ToString();
        txt_Armor.text = ((int)player.m_HealthManager.m_CurrentArmor).ToString() + "/" + ((int)player.m_HealthManager.m_DefaultArmor).ToString();

        OnAmmoStatus(player.m_WeaponCurrent);
        OnActionStatus(player.m_PlayerInfo);
    }
    void OnAmmoStatus(WeaponBase weaponInfo)
    {
        m_AmmoGrid.transform.SetActivate(weaponInfo != null);
        if (weaponInfo != null)
        {
            if (weaponInfo.B_Reloading)
            {
                sld_Reload.value = weaponInfo.F_ReloadStatus;
                m_AmmoGrid.ClearGrid();
                return;
            }

            img_sld.color = Color.Lerp(Color.red, Color.white, m_player.m_WeaponCurrent.F_ReloadStatus);
            if (m_AmmoGrid.I_Count != m_player.m_WeaponCurrent.I_ClipAmout)
            {
                m_AmmoGrid.ClearGrid();
                if (m_player.m_WeaponCurrent.I_ClipAmout <= 30)
                {
                    for (int i = 0; i < weaponInfo.I_ClipAmout; i++)
                        m_AmmoGrid.AddItem(i);

                    float size = (f_ammoGridLength - m_GridLayout.padding.bottom - m_GridLayout.padding.top - (m_player.m_WeaponCurrent.I_ClipAmout - 1) * m_GridLayout.spacing.y) / m_player.m_WeaponCurrent.I_ClipAmout;
                    m_GridLayout.cellSize = new Vector2(m_GridLayout.cellSize.x, size);
                }
            }

            Color ammoStatusColor = weaponInfo.F_AmmoStatus < .5f ? Color.Lerp(Color.red, Color.white, (weaponInfo.F_AmmoStatus / .5f)) : Color.white;
            if (m_player.m_WeaponCurrent.I_ClipAmout <= 30)
            {
                for (int i = 0; i < weaponInfo.I_ClipAmout; i++)
                    m_AmmoGrid.GetItem(i).Set((m_player.m_WeaponCurrent.B_Reloading || i > m_player.m_WeaponCurrent.I_AmmoLeft - 1) ? new Color(0, 0, 0, 0) : ammoStatusColor);
                sld_Reload.value = 0;
            }
            else
            {
                sld_Reload.value = weaponInfo.F_AmmoStatus;
                img_sld.color = ammoStatusColor;
            }
        }
    }
    void OnActionStatus(PlayerInfoManager actionInfos)
    {
        m_ActionGrid.ClearGrid();
    }
    protected void Update()
    {
        if (m_player)
            tf_Center.position =Vector3.Lerp(tf_Center.position, CameraController.MainCamera.WorldToScreenPoint(m_player.tf_Head.position),Time.deltaTime*10f);
    }
}
