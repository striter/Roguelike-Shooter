using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIManager :SimpleSingletonMono<UIManager>
{
    public static Action OnReload;
    public static Action<bool> OnMainDown;
    Transform tf_Top,tf_Pages,tf_LowerTools;
    Image img_main;
    public AtlasLoader m_commonSprites { get; private set; }
    public T ShowPage<T>(bool animate) where T : UIPageBase => UIPageBase.ShowPage<T>(tf_Pages, animate);
    protected T ShowTools<T>() where T : UIToolsBase => UIToolsBase.Show<T>(tf_LowerTools);
    public static void Activate(bool inGame) => TResources.InstantiateUIManager().Init(inGame);
    protected void Init(bool inGame)
    {
        m_commonSprites = TResources.GetUIAtlas_Common();

        tf_Top = transform.Find("Top");
        tf_LowerTools = transform.Find("Lower");
        tf_Pages = transform.Find("Pages");
        img_main = tf_Top.Find("Main/Image").GetComponent<Image>();
        tf_Top.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        tf_Top.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        tf_Top.Find("Settings").GetComponent<Button>().onClick.AddListener(() => { ShowPage<UI_Options>(inGame).SetInGame(inGame); });
        if(inGame) ShowTools<UI_EntityHealth>();
        ShowTools<UI_PlayerStatus>().SetInGame(inGame);
        GetComponent<TouchDeltaManager>().SetJoystick(tf_Top.Find("JoyStick").GetComponent<UIT_JoyStick>(), enum_JoyStickMode.Retarget);
        if (inGame) transform.Find("Test/SeedTest").GetComponent<Text>().text = GameManager.Instance.m_GameLevel.m_Seed;   //Test
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }

    string m_mainSprite;
    void OnPlayerStatusChanged(EntityCharacterPlayer player)
    {
        string spriteName = "main_fire";
        if(player.m_Interact!=null)
            switch (player.m_Interact.m_InteractType)
            {
                case enum_Interaction.Invalid:Debug.LogError("Invalid Pharse Here!");break;
                case enum_Interaction.ActionAdjustment:spriteName = "main_chat";break;
                default:spriteName = "main_pickup";break;
            }

        if (spriteName == m_mainSprite)
            return;
        m_mainSprite = spriteName;
        img_main.sprite = m_commonSprites[m_mainSprite];
    }
}
