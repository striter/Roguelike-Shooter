using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIManager :SimpleSingletonMono<UIManager>,ISingleCoroutine
{
    public Vector2 m_FittedScale { get; private set; }
    Canvas cvs_Overlay, cvs_Camera;
    Transform tf_Control, tf_Pages, tf_Tools;
    public Action OnReload;
    public Action<bool> OnMainDown;
    Image img_main;
    TouchDeltaManager m_TouchDelta;
    public static void Activate(bool inGame) => TResources.InstantiateUIManager().Init(inGame);
    public Camera m_Camera { get; private set; }
    public CameraEffectManager m_Effect { get; private set; }
    CB_GenerateOverlayUIGrabBlurTexture m_Blur;
    protected void Init(bool inGame)
    {
        cvs_Overlay = transform.Find("Overlay").GetComponent<Canvas>();
        CanvasScaler m_Scaler = cvs_Overlay.GetComponent<CanvasScaler>();
        m_FittedScale = new Vector2((Screen.width / (float)Screen.height) / (m_Scaler.referenceResolution.x / m_Scaler.referenceResolution.y), 1f);
        cvs_Camera = transform.Find("Camera").GetComponent<Canvas>();

        tf_Control = cvs_Camera.transform.Find("Control");
        tf_Tools = cvs_Camera.transform.Find("Tools");
        img_main = tf_Control.Find("Main/Image").GetComponent<Image>();
        tf_Control.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        tf_Control.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        tf_Control.Find("Settings").GetComponent<Button>().onClick.AddListener(() => { ShowPage<UI_Options>(inGame,0f).SetInGame(inGame); });

        tf_Pages = cvs_Overlay.transform.Find("Pages");
        if (inGame) ShowTools<UI_EntityHealth>();
        ShowTools<UI_PlayerStatus>().SetInGame(inGame);


        if (inGame) cvs_Overlay.transform.Find("Test/SeedTest").GetComponent<Text>().text = GameManager.Instance.m_GameLevel.m_Seed;   //Test

        m_Camera = transform.Find("UICamera").GetComponent<Camera>();
        m_Effect = m_Camera.GetComponent<CameraEffectManager>();
        m_Blur = m_Effect.GetOrAddCameraEffect<CB_GenerateOverlayUIGrabBlurTexture>();
        m_Blur.SetEffect(1, 2f, 2);

        m_TouchDelta = cvs_Camera.GetComponent<TouchDeltaManager>();
        OnOptionsChanged();
        OptionsManager.event_OptionChanged += OnOptionsChanged;

        UIPageBase.OnPageExit = OnPageExit;
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);

        if (inGame)
        {
            m_InGameSprites.Check();
            m_CommonSprites.Check();
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.StopAllCoroutines();
        OptionsManager.event_OptionChanged -= OnOptionsChanged;
        UIPageBase.OnPageExit = null;
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }

    void OnOptionsChanged()=> UIT_JoyStick.Instance.SetMode(OptionsManager.m_OptionsData.m_JoyStickMode);
    public void DoBinding(Action<Vector2> _OnLeftDelta,Action<Vector2> _OnRightDelta,Action _OnReload,Action<bool> _OnMainDown )
    {
        m_TouchDelta.DoBinding(_OnLeftDelta, _OnRightDelta, () => !UIPageBase.m_PageOpening);
        OnReload = _OnReload;
        OnMainDown = _OnMainDown;
    }

    string m_mainSprite;
    void OnPlayerStatusChanged(EntityCharacterPlayer player)
    {
        string spriteName = UIEnumConvertions.GetMainSprite(player);
        if (spriteName == m_mainSprite)
            return;
        m_mainSprite = spriteName;
        img_main.sprite = m_CommonSprites[m_mainSprite];
    }

    public void OnGameFinished(GameLevelManager level,Action _OnButtonClick)
    {
        cvs_Camera.gameObject.SetActivate(false);
        ShowPage<UI_GameResult>(true).Play(level, _OnButtonClick);
    }

    public T ShowPage<T>(bool animate,float bulletTime=1f) where T : UIPageBase
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageOpen,bulletTime);
        if (bulletTime!=1f)
            GameManagerBase.SetBulletTime(true,bulletTime);
        return UIPageBase.ShowPage<T>(tf_Pages, animate);
    }
    void OnPageExit()
    {
        GameManagerBase.SetBulletTime(false);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageClose);
    }
    protected T ShowTools<T>() where T : UIToolsBase => UIToolsBase.Show<T>(tf_Tools);


    AtlasLoader m_inGameSprites;
    public AtlasLoader m_InGameSprites
    {
        get
        {
            if (m_inGameSprites == null)
                m_inGameSprites = TResources.GetUIAtlas_InGame();
            return m_inGameSprites;
        }
    }
    AtlasLoader m_actionSprites;
    public AtlasLoader m_ActionSprites
    {
        get
        {
            if (m_actionSprites == null)
                m_actionSprites = TResources.GetUIAtlas_Action();
            return m_actionSprites;
        }
    }
    AtlasLoader m_commonSprites;
    public AtlasLoader m_CommonSprites
    {
        get
        {
            if (m_commonSprites == null)
                m_commonSprites = TResources.GetUIAtlas_Common();
            return m_commonSprites;
        }
    }
}
