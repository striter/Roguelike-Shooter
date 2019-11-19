using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIManager :UIManagerBase,ISingleCoroutine
{
    public static new UIManager Instance { get; private set; }
    Image m_setting, m_OverlayBG;
    public Camera m_Camera { get; private set; }
    public UIC_CharacterControl m_CharacterControl { get; private set; }
    public UIC_GameControl m_GameControl { get; private set; }
    public UIC_PlayerStatus m_PlayerStatus { get; private set; }
    public UIC_Indicates m_Indicates { get; private set; }

    public CameraEffectManager m_Effect { get; private set; }
    CB_GenerateOverlayUIGrabBlurTexture m_Blur;
    public AtlasLoader m_CommonSprites { get; private set; }
    public AtlasLoader m_ActionSprites { get; private set; }
    public AtlasLoader m_WeaponSprites { get; private set; }
    public static void Activate(bool inGame)
    {
        GameObject uiObj = TResources.InstantiateUIManager();
        UIManager manager = null;
        if (inGame)
            manager = uiObj.AddComponent<GameUIManager>();
        else
            manager = uiObj.AddComponent<CampUIManager>();
        manager.Init();
        manager.InitGameControls(inGame);
    }

    protected override void Init()
    {
        base.Init();
        Instance = this;
        cvs_Camera.transform.Find("Settings").GetComponent<Button>().onClick.AddListener(OnSettingBtnClick);
        m_setting = cvs_Camera.transform.Find("Settings/Image").GetComponent<Image>();

        m_OverlayBG = cvs_Overlay.transform.Find("OverlayBG").GetComponent<Image>();
        m_OverlayBG.SetActivate(false);

        m_CommonSprites = TResources.GetUIAtlas_Common();
        m_ActionSprites = TResources.GetUIAtlas_Action();
        m_WeaponSprites = TResources.GetUIAtlas_Weapon();

        m_Camera = transform.Find("UICamera").GetComponent<Camera>();
        m_Effect = m_Camera.GetComponent<CameraEffectManager>();
        m_Blur = m_Effect.GetOrAddCameraEffect<CB_GenerateOverlayUIGrabBlurTexture>();
        m_Blur.SetEffect(1, 2f, 2);
    }

    protected virtual void InitGameControls(bool inGame)
    {
        m_Indicates = ShowControls<UIC_Indicates>(true);
        m_GameControl = ShowControls<UIC_GameControl>().SetInGame(inGame);
        m_PlayerStatus = ShowControls<UIC_PlayerStatus>();
        m_CharacterControl = ShowControls<UIC_CharacterControl>();
        ShowControls<UIC_PlayerInteract>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
        this.StopAllCoroutines();
    }


    public new T ShowMessageBox<T>() where T : UIMessageBoxBase => base.ShowMessageBox<T>();
    public T ShowPage<T>(bool animate,float bulletTime=1f) where T : UIPageBase
    {
        if (UIPageBase.Opening<T>())
            return null;

        T page = base.ShowPage<T>(animate);
        if(page!=null)
        {
            m_OverlayBG.SetActivate(true);
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageOpen, bulletTime);
            if (bulletTime != 1f)
                GameManagerBase.SetBulletTime(true, bulletTime);
        }
        return page;
    }

    protected override void OnPageExit()
    {
        base.OnPageExit();
        if (UIPageBase.I_PageCount > 0)
            return;

        m_OverlayBG.SetActivate(false);
        GameManagerBase.SetBulletTime(false);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageClose);
    }


    void OnSettingBtnClick()
    {
        if (OnSettingClick != null)
        {
            OnSettingClick();
            return;
        }
        ShowPage<UI_Options>(true, 0f).SetInGame(GameManagerBase.Instance.B_InGame);
    }
    Action OnSettingClick;
    protected void OverrideSetting(Action Override=null)
    {
        OnSettingClick = Override;
        m_setting.sprite = m_CommonSprites[Override == null ? "icon_setting" : "icon_close"];
        m_setting.SetNativeSize();
    }
}
