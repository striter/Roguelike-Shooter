using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;
using TTiles;
using UnityEngine.U2D;

public class UI_GameManager : SingletonMono<UI_GameManager>
{
    public static Action OnReload;
    public static Action<bool> OnMainDown;
    Transform tf_Top,tf_Pages;
    Image img_fire,img_pickup,img_chat;
    Button btn_Bigmap;
    Image m_main;
    protected override void Awake()
    {
        instance = this;
        tf_Top = transform.Find("Top");
        tf_Top.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        tf_Top.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        img_fire = tf_Top.Find("Main/Fire").GetComponent<Image>();
        img_pickup = tf_Top.Find("Main/Pickup").GetComponent<Image>();
        img_chat= tf_Top.Find("Main/Chat").GetComponent<Image>();
        btn_Bigmap = transform.Find("Top/Bigmap").GetComponent<Button>();
        btn_Bigmap.onClick.AddListener(()=> { ShowPage<UI_BigmapControl>(true);  });
        tf_Pages = transform.Find("Pages");
        
        transform.Find("Test/SporeBtn").GetComponent<Button>().onClick.AddListener(() => { ShowPage<UI_SporeManager>(true); });
    }
    private void Start()
    {
        transform.Find("Test/SeedTest").GetComponent<Text>().text = GameManager.Instance.m_GameLevel.m_Seed;
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }
    private void OnDestroy()
    {
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }
    void OnBattleStart()
    {
        btn_Bigmap.SetActivate(false);
    }
    void OnBattleFinish()
    {
        btn_Bigmap.SetActivate(true);
    }
    public T ShowPage<T>(bool animate) where T : UIPageBase
    {
        return UIPageBase.ShowPage<T>(tf_Pages, animate);
    }
    void OnPlayerStatusChanged(EntityCharacterPlayer player)
    {
        Image mainImage=img_fire;
        if (player.m_Interact != null)
        {
            switch (player.m_Interact.m_InteractType)
            {
                case enum_Interaction.Invalid: Debug.LogError("???? Here");break;
                case enum_Interaction.ActionAdjustment:mainImage = img_chat;break;
                default:mainImage = img_pickup;break;
            }
        }
        if (m_main == mainImage)
            return;
        if(m_main)
            m_main.SetActivate(false);
        m_main = mainImage;
        m_main.SetActivate(true);
    }
}
