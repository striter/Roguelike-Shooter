using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;
using TTiles;
public class UIManager : SingletonMono<UIManager> {

    Text txt_Ammo,txt_Mana,txt_Health,txt_Armor,txt_Coin,txt_Main;
    RectTransform rtf_Pitch;
    Text txt_Pitch;
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

        rtf_Pitch = transform.Find("Pitch/Pitch").GetComponent<RectTransform>();
        txt_Pitch = rtf_Pitch.Find("Pitch").GetComponent<Text>();
        transform.Find("Switch").GetComponent<Button>().onClick.AddListener(()=> { OnSwitch?.Invoke(); });
        transform.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        transform.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        transform.Find("SporeBtn").GetComponent<Button>().onClick.AddListener(() => { UIPageBase.ShowPage<UI_SporeManager>(transform,true); });

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
        txt_Mana.text = ((int)player.m_CurrentMana).ToString() + "/" + ((int)player.m_EntityInfo.m_MaxMana).ToString();
        txt_Health.text = ((int)player.m_CurrentHealth).ToString() + "/" + ((int)player.m_EntityInfo.m_MaxHealth).ToString();
        txt_Armor.text =((int)player.m_CurrentArmor).ToString() + "/" + ((int)player.m_EntityInfo.m_MaxArmor).ToString();
        txt_Coin.text = player.m_Coins.ToString();

        txt_Pitch.text = ((int)player.m_Pitch).ToString();
        rtf_Pitch.anchoredPosition = new Vector2(0, (player.m_Pitch / 45f) * 900);
         
        txt_Main.text = player.B_Interacting ? "Interact" : "Fire";
    }

}
