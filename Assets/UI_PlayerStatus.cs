using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStatus : SimpleSingletonMono<UI_PlayerStatus>
{
    Transform tf_Container;
    Text  txt_Health, txt_Armor;
    Slider sld_Reload;
    Image img_sld;
    UIT_GridControllerMono<UIGI_AmmoItem> m_Grid;
    EntityPlayerBase m_player;
    GridLayoutGroup m_GridLayout;
    protected override void Awake()
    {
        base.Awake();
        tf_Container = transform.Find("Container");
        txt_Health = tf_Container.Find("Health").GetComponent<Text>();
        txt_Armor = tf_Container.Find("Armor").GetComponent<Text>();
        m_Grid = new UIT_GridControllerMono<UIGI_AmmoItem>(tf_Container.Find("AmmoGrid"));
        m_GridLayout = m_Grid.transform.GetComponent<GridLayoutGroup>();
        sld_Reload = m_Grid.transform.Find("Reload").GetComponent<Slider>();
        img_sld = sld_Reload.transform.Find("Fill").GetComponent<Image>();
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
        txt_Armor.text = ((int)player.m_HealthManager.m_CurrentArmor).ToString() + "/" + ((int)player.m_HealthManager.m_MaxArmor).ToString();
        if (player.m_WeaponCurrent != null)
        {
            sld_Reload.value = player.m_WeaponCurrent.B_Reloading? player.m_WeaponCurrent.F_ReloadStatus:0;
            img_sld.color = player.m_WeaponCurrent.F_ReloadStatus < .5f ? Color.Lerp(Color.red, Color.white, m_player.m_WeaponCurrent.F_ReloadStatus / .5f) :Color.white;
            if (m_Grid.I_Count != m_player.m_WeaponCurrent.m_WeaponInfo.m_ClipAmount)
            {
                m_Grid.ClearGrid();
                for (int i = 0; i < player.m_WeaponCurrent.m_WeaponInfo.m_ClipAmount; i++)
                    m_Grid.AddItem(i);

                float size = (UIConst.F_IAmmoLineLength + m_GridLayout.padding.bottom + m_GridLayout.padding.top - (m_player.m_WeaponCurrent.m_WeaponInfo.m_ClipAmount - 1) * m_GridLayout.spacing.x) / m_player.m_WeaponCurrent.m_WeaponInfo.m_ClipAmount;
                m_GridLayout.cellSize = new Vector2( m_GridLayout.cellSize.x, size);
            }

            for (int i = 0; i < player.m_WeaponCurrent.m_WeaponInfo.m_ClipAmount; i++)
            {
                Color gridItemColor = player.m_WeaponCurrent.F_AmmoStatus < .3f ? Color.Lerp(Color.red, Color.white, m_player.m_WeaponCurrent.F_AmmoStatus / .3f) : Color.white;
                m_Grid.GetItem(i).Set((m_player.m_WeaponCurrent.B_Reloading || i > m_player.m_WeaponCurrent.I_AmmoLeft - 1) ? new Color(0, 0, 0, 0) : gridItemColor);
            }
        }
        else
        {
            m_Grid.ClearGrid();
            m_Grid.transform.SetActivate(false);
            sld_Reload.SetActivate(false);
        }
    }
    protected void Update()
    {
        if (m_player)
            tf_Container.position =Vector3.Lerp(tf_Container.position, CameraController.MainCamera.WorldToScreenPoint(m_player.tf_Head.position),Time.deltaTime*10f);
    }
}
