using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIManager :SimpleSingletonMono<UIManager>
{
    Canvas cvs_Overlay, cvs_Camera;
    Transform tf_Control, tf_Pages, tf_Tools;
    public Action OnReload;
    public Action<bool> OnMainDown;
    Image img_main;
    TouchDeltaManager m_TouchDelta;
    public AtlasLoader m_commonSprites { get; private set; }
    public T ShowPage<T>(bool animate) where T : UIPageBase => UIPageBase.ShowPage<T>(tf_Pages, animate);
    protected T ShowTools<T>() where T : UIToolsBase => UIToolsBase.Show<T>(tf_Tools);
    public static void Activate(bool inGame) => TResources.InstantiateUIManager().Init(inGame);
    public Camera m_Camera { get; private set; }
    protected void Init(bool inGame)
    {
        m_commonSprites = TResources.GetUIAtlas_Common();
        cvs_Overlay = transform.Find("Overlay").GetComponent<Canvas>();
        cvs_Camera = transform.Find("Camera").GetComponent<Canvas>();

        tf_Control = cvs_Camera.transform.Find("Control");
        tf_Tools = cvs_Camera.transform.Find("Tools");
        img_main = tf_Control.Find("Main/Image").GetComponent<Image>();
        tf_Control.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        tf_Control.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        tf_Control.Find("Settings").GetComponent<Button>().onClick.AddListener(() => { ShowPage<UI_Options>(inGame).SetInGame(inGame); });

        tf_Pages = cvs_Overlay.transform.Find("Pages");
        if (inGame) ShowTools<UI_EntityHealth>();
        ShowTools<UI_PlayerStatus>().SetInGame(inGame);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);

        m_TouchDelta = cvs_Camera.GetComponent<TouchDeltaManager>();
        OnOptionsChanged();
        OptionsManager.event_OptionChanged += OnOptionsChanged;

        if (inGame) cvs_Overlay.transform.Find("Test/SeedTest").GetComponent<Text>().text = GameManager.Instance.m_GameLevel.m_Seed;   //Test

        m_Camera = transform.Find("UICamera").GetComponent<Camera>();
        m_Camera.GetComponent<CameraEffectManager>().AddCameraEffect<CB_GenerateGlobalGaussianBlurTexture>().SetEffect(1, 2f,2);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        OptionsManager.event_OptionChanged -= OnOptionsChanged;
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }

    void OnOptionsChanged()=> UIT_JoyStick.Instance.SetMode(OptionsManager.m_OptionsData.m_JoyStickMode);
    public void DoBinding(Action<Vector2> _OnLeftDelta,Action<Vector2> _OnRightDelta,Action _OnReload,Action<bool> _OnMainDown )
    {
        m_TouchDelta.DoBinding(_OnLeftDelta,_OnRightDelta);
        OnReload = _OnReload;
        OnMainDown = _OnMainDown;
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
