using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIManager :UIManagerBase,ISingleCoroutine
{
    public static new UIManager Instance { get; private set; }
    protected Transform tf_Control { get; private set; }
    protected Button btn_Reload { get; private set; }
    Action OnReload;
    Action<bool> OnMainDown;
    Image img_main;
    protected TouchDeltaManager m_TouchDelta { get; private set; }
    public Camera m_Camera { get; private set; }
    public CameraEffectManager m_Effect { get; private set; }
    CB_GenerateOverlayUIGrabBlurTexture m_Blur;
    public AtlasLoader m_CommonSprites { get; private set; }
    public AtlasLoader m_ActionSprites { get; private set; }
    public static void Activate(bool inGame)
    {
        GameObject uiObj = TResources.InstantiateUIManager();
        if (inGame)
            uiObj.AddComponent<GameUIManager>().Init();
        else
            uiObj.AddComponent<CampUIManager>().Init();
    } 
    protected override void Init()
    {
        base.Init();
        Instance = this;
        cvs_Camera.transform.Find("Settings").GetComponent<Button>().onClick.AddListener(() => { ShowPage<UI_Options>(true, 0f).SetInGame(GameManagerBase.Instance.B_InGame); });

        tf_Control = cvs_Camera.transform.Find("Control");
        img_main = tf_Control.Find("Main/Image").GetComponent<Image>();
        btn_Reload = tf_Control.Find("Reload").GetComponent<Button>();
        btn_Reload.onClick.AddListener(OnReloadButtonDown);
        tf_Control.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+= OnMainButtonDown;

        m_CommonSprites = TResources.GetUIAtlas_Common();
        m_ActionSprites = TResources.GetUIAtlas_Action();
        m_Camera = transform.Find("UICamera").GetComponent<Camera>();
        m_Effect = m_Camera.GetComponent<CameraEffectManager>();
        m_Blur = m_Effect.GetOrAddCameraEffect<CB_GenerateOverlayUIGrabBlurTexture>();
        m_Blur.SetEffect(1, 2f, 2);

        m_TouchDelta = transform.GetComponent<TouchDeltaManager>();
        OnOptionsChanged();
        OptionsManager.event_OptionChanged += OnOptionsChanged;

        UIPageBase.OnPageExit = OnPageExit;
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
        this.StopAllCoroutines();
        OptionsManager.event_OptionChanged -= OnOptionsChanged;
        UIPageBase.OnPageExit = null;
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }

    void OnOptionsChanged()=> UIT_JoyStick.Instance.SetMode(OptionsManager.m_OptionsData.m_JoyStickMode);
    bool CheckControlable() => !UIPageBase.m_PageOpening;
    public void DoBinding(Action<Vector2> _OnLeftDelta,Action<Vector2> _OnRightDelta,Action _OnReload,Action<bool> _OnMainDown )
    {
        m_TouchDelta.AddLRBinding(_OnLeftDelta, _OnRightDelta,CheckControlable);
        OnReload = _OnReload;
        OnMainDown = _OnMainDown;
    }
    public void RemoveBinding()
    {
        m_TouchDelta.RemoveAllBinding();
        OnReload = null;
        OnMainDown = null;
    }

    protected void OnReloadButtonDown()
    {
        OnReload?.Invoke();
    }
    protected void OnMainButtonDown(bool down,Vector2 pos)
    {
        OnMainDown?.Invoke(down);
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

    public T ShowPage<T>(bool animate,float bulletTime=1f) where T : UIPageBase
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageOpen,bulletTime);
        if (bulletTime!=1f)
            GameManagerBase.SetBulletTime(true,bulletTime);
        return base.ShowPage<T>(animate);
    }
    void OnPageExit()
    {
        GameManagerBase.SetBulletTime(false);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageClose);
    }
}
