using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;
using TTiles;
public class UIManager : SingletonMono<UIManager> {
    Transform tf_Top;
    Text txt_Coin,txt_Main;
    public static Action OnReload;
    public static Action<bool> OnMainDown;
    protected override void Awake()
    {
        instance = this;
        tf_Top = transform.Find("Top");
        txt_Coin = tf_Top.Find("Coin").GetComponent<Text>();
        
        tf_Top.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        tf_Top.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        tf_Top.Find("SporeBtn").GetComponent<Button>().onClick.AddListener(() => { UIPageBase.ShowPage<UI_SporeManager>(transform,true); });
        txt_Main = tf_Top.Find("Main/Text").GetComponent<Text>();
    }
    private void Start()
    {
        UIPageBase.ShowPage<UI_BigmapControl>(transform, false);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityPlayerBase>(enum_BC_GameStatusChanged.PlayerInfoChanged, OnPlayerStatusChanged);
    }
    private void OnDestroy()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityPlayerBase>(enum_BC_GameStatusChanged.PlayerInfoChanged, OnPlayerStatusChanged);
    }
    void OnPlayerStatusChanged(EntityPlayerBase player)
    {
        txt_Coin.text = player.m_Coins.ToString();
        txt_Main.text = player.m_Equipment!=null ?"Use Equipment":player.m_Interact!=null? "Interact" : "Fire";
    }
}
