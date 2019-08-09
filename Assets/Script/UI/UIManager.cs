using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;
using TTiles;
public class UIManager : SingletonMono<UIManager> {

    Text txt_Ammo,txt_Mana,txt_Health,txt_Armor,txt_Coin,txt_Main;
    Image img_Reload;
    public static Action OnSwitch, OnReload;
    public static Action<bool> OnMainDown;
    protected override void Awake()
    {
        instance = this;
        txt_Ammo = transform.Find("Ammo").GetComponent<Text>();
        txt_Mana = transform.Find("Mana").GetComponent<Text>();
        txt_Health = transform.Find("Health").GetComponent<Text>();
        txt_Armor = transform.Find("Armor").GetComponent<Text>();
        txt_Coin = transform.Find("Coin").GetComponent<Text>();

        transform.Find("Switch").GetComponent<Button>().onClick.AddListener(()=> { OnSwitch?.Invoke(); });
        transform.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        transform.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        transform.Find("SporeBtn").GetComponent<Button>().onClick.AddListener(() => { UIPageBase.ShowPage<UI_SporeManager>(transform,true); });
        img_Reload = transform.Find("Reload").GetComponent<Image>();
        txt_Main = transform.Find("Main/Text").GetComponent<Text>();

    }
    private void Start()
    {
        UIPageBase.ShowPage<UI_BigmapControl>(transform, false);
        TBroadCaster<enum_BC_UIStatusChanged>.Add<EntityPlayerBase>(enum_BC_UIStatusChanged.PlayerInfoChanged, OnPlayerStatusChanegd);

    }
    private void OnDisable()
    {
        TBroadCaster<enum_BC_UIStatusChanged>.Remove<EntityPlayerBase>(enum_BC_UIStatusChanged.PlayerInfoChanged, OnPlayerStatusChanegd);
    }
    void OnPlayerStatusChanegd(EntityPlayerBase player)
    {
        txt_Ammo.text = player.m_WeaponCurrent.I_AmmoLeft.ToString()+"/"+player.m_WeaponCurrent.m_WeaponInfo.m_ClipAmount.ToString();
        txt_Mana.text = ((int)player.m_CurrentMana).ToString() + "/" + ((int)player.m_EntityInfo.F_MaxMana).ToString();
        txt_Health.text = ((int)player.m_HealthManager.m_CurrentHealth).ToString() + "/" + ((int)player.m_HealthManager.m_MaxHealth).ToString();
        txt_Armor.text =((int)player.m_HealthManager.m_CurrentArmor).ToString() + "/" + ((int)player.m_HealthManager.m_MaxArmor).ToString();
        txt_Coin.text = player.m_Coins.ToString();
        img_Reload.fillAmount = player.m_WeaponCurrent != null ? player.m_WeaponCurrent.F_ReloadStatus : 1;
        txt_Main.text = player.B_Interacting ? "Interact" : "Fire";
    }

}
