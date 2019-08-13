using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;
using TTiles;
public class UIManager : SingletonMono<UIManager> {
    Transform tf_Top;
    Text txt_Ammo,txt_Health,txt_Armor,txt_Coin,txt_Main;
    Image img_Reload;
    public static Action OnSwitch, OnReload;
    public static Action<bool> OnMainDown;
    protected override void Awake()
    {
        instance = this;
        tf_Top = transform.Find("Top");
        txt_Ammo = tf_Top.Find("Ammo").GetComponent<Text>();
        txt_Health = tf_Top.Find("Health").GetComponent<Text>();
        txt_Armor = tf_Top.Find("Armor").GetComponent<Text>();
        txt_Coin = tf_Top.Find("Coin").GetComponent<Text>();

        tf_Top.Find("Switch").GetComponent<Button>().onClick.AddListener(()=> { OnSwitch?.Invoke(); });
        tf_Top.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        tf_Top.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        tf_Top.Find("SporeBtn").GetComponent<Button>().onClick.AddListener(() => { UIPageBase.ShowPage<UI_SporeManager>(transform,true); });
        img_Reload = tf_Top.Find("Reload").GetComponent<Image>();
        txt_Main = tf_Top.Find("Main/Text").GetComponent<Text>();
    }
    private void Start()
    {
        UIPageBase.ShowPage<UI_BigmapControl>(transform, false);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityPlayerBase>(enum_BC_GameStatusChanged.PlayerInfoChanged, OnPlayerStatusChanegd);
    }
    private void OnDisable()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityPlayerBase>(enum_BC_GameStatusChanged.PlayerInfoChanged, OnPlayerStatusChanegd);
    }
    void OnPlayerStatusChanegd(EntityPlayerBase player)
    {
        txt_Ammo.text = player.m_WeaponCurrent.I_AmmoLeft.ToString()+"/"+player.m_WeaponCurrent.m_WeaponInfo.m_ClipAmount.ToString();
        txt_Health.text = ((int)player.m_HealthManager.m_CurrentHealth).ToString() + "/" + ((int)player.m_HealthManager.m_MaxHealth).ToString();
        txt_Armor.text =((int)player.m_HealthManager.m_CurrentArmor).ToString() + "/" + ((int)player.m_HealthManager.m_MaxArmor).ToString();
        txt_Coin.text = player.m_Coins.ToString();
        img_Reload.fillAmount = player.m_WeaponCurrent != null ? player.m_WeaponCurrent.F_ReloadStatus : 1;
        txt_Main.text = player.B_Interacting ? "Interact" : "Fire";
    }
}
