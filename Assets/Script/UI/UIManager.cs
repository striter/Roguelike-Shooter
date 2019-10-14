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
    public AtlasLoader m_commonSprites { get; private set; }
    public T ShowPage<T>(bool animate) where T : UIPageBase => UIPageBase.ShowPage<T>(tf_Pages, animate);
    protected T ShowTools<T>() where T : UIToolsBase => UIToolsBase.Show<T>(tf_Tools);
    public static void Activate(bool inGame) => TResources.InstantiateUIManager().Init(inGame);
    public Camera m_Camera { get; private set; }
    public CameraEffectManager m_Effect { get; private set; }
    CB_GenerateGlobalGaussianBlurTexture m_Blur;
    protected void Init(bool inGame)
    {
        m_commonSprites = TResources.GetUIAtlas_Common();
        cvs_Overlay = transform.Find("Overlay").GetComponent<Canvas>();
        CanvasScaler m_Scaler = cvs_Overlay.GetComponent<CanvasScaler>();
        m_FittedScale = new Vector2((Screen.width / (float)Screen.height) / (m_Scaler.referenceResolution.x / m_Scaler.referenceResolution.y), 1f);
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

        m_TouchDelta = cvs_Camera.GetComponent<TouchDeltaManager>();
        OnOptionsChanged();
        OptionsManager.event_OptionChanged += OnOptionsChanged;

        if (inGame) cvs_Overlay.transform.Find("Test/SeedTest").GetComponent<Text>().text = GameManager.Instance.m_GameLevel.m_Seed;   //Test

        m_Camera = transform.Find("UICamera").GetComponent<Camera>();
        m_Effect = m_Camera.GetComponent<CameraEffectManager>();
        m_Blur = m_Effect.AddCameraEffect<CB_GenerateGlobalGaussianBlurTexture>();
        m_Blur.SetEffect(1, 2f, 2);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.StopAllCoroutines();
        OptionsManager.event_OptionChanged -= OnOptionsChanged;
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }

    void OnOptionsChanged()=> UIT_JoyStick.Instance.SetMode(OptionsManager.m_OptionsData.m_JoyStickMode);
    public void DoBinding(Action<Vector2> _OnLeftDelta,Action<Vector2> _OnRightDelta,Action _OnReload,Action<bool> _OnMainDown )
    {
        m_TouchDelta.DoBinding(_OnLeftDelta, _OnRightDelta, () => !UIPageBase.m_PageOpening;);
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
        img_main.sprite = m_commonSprites[m_mainSprite];
    }

    public void OnGameFinished(bool win, float levelScore, float killScore, float credit, Action _OnButtonClick)
    {
        cvs_Camera.gameObject.SetActivate(false);
        ShowPage<UI_GameResult>(true).Play(win, levelScore, killScore, credit, _OnButtonClick);
    }

}
