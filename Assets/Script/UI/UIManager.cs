using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIManager :UIManagerBase,ICoroutineHelperClass
{
    public static new UIManager Instance { get; private set; }
    Image m_OverlayBG;
    public Camera m_Camera { get; private set; }
    protected UIC_Control m_UIControl { get; private set; }
    protected UIC_PlayerInteract m_Interact { get; private set; }
    protected UIC_GameStatus m_PlayerStatus { get; private set; }
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
        m_OverlayBG = cvs_Overlay.transform.Find("OverlayBG").GetComponent<Image>();
        m_OverlayBG.SetActivate(false);

        m_CommonSprites = TResources.GetUIAtlas_Common();
        m_ActionSprites = TResources.GetUIAtlas_Action();
        m_WeaponSprites = TResources.GetUIAtlas_Weapon();

        m_Camera = transform.Find("UICamera").GetComponent<Camera>();
        m_Effect = m_Camera.GetComponent<CameraEffectManager>();
        m_Blur = m_Effect.GetOrAddCameraEffect<CB_GenerateOverlayUIGrabBlurTexture>();
        m_Blur.SetEffect(2, 2f, 2);
    }

    protected virtual void InitGameControls(bool inGame)
    {
        m_PlayerStatus = ShowControls<UIC_GameStatus>();
        m_UIControl = ShowControls<UIC_Control>().SetInGame(inGame);
        m_Interact = ShowControls<UIC_PlayerInteract>();
        m_Indicates = ShowControls<UIC_Indicates>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
        this.StopAllCoroutines();
    }


    public new T ShowMessageBox<T>() where T : UIMessageBoxBase => base.ShowMessageBox<T>();
    public T ShowPage<T>(bool animate,float bulletTime=1f) where T : UIPage
    {
        T page = base.ShowPage<T>(animate);
        if (page == null)
            return null;
        m_OverlayBG.SetActivate(true);
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_OverlayBG.color = TCommon.ColorAlpha(m_OverlayBG.color,value); },0,1, UIPageBase.F_AnimDuration, null,false));
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageOpen, bulletTime);
        if (bulletTime != 1f)
            GameManagerBase.SetBulletTime(true, bulletTime);
        return page;
    }

    protected override void OnPageExit()
    {
        base.OnPageExit();
        if (UIPageBase.I_PageCount > 0)
            return;

        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
            m_OverlayBG.color = TCommon.ColorAlpha(m_OverlayBG.color, value);
            if(value==0)  m_OverlayBG.SetActivate(false);
        }, 1, 0, UIPageBase.F_AnimDuration, null, false));
        GameManagerBase.SetBulletTime(false);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageClose);
    }

    public void DoBindings(EntityCharacterPlayer player, Action<Vector2> _OnLeftDelta, Action<Vector2> _OnRightDelta, Action<bool> _OnMainDown,Action<bool> _OnSubDown,Action _OnInteractClick, Action _OnSwap, Action _OnReload, Action _OnCharacterAbility)
    {
        m_UIControl.DoBinding(player, _OnLeftDelta, _OnRightDelta, _OnMainDown, _OnSubDown, _OnSwap, _OnReload, _OnCharacterAbility);
        m_Interact.DoBindings(_OnInteractClick);
    }

    public void RemoveBindings()
    {
        m_UIControl.RemoveBinding();
        m_Interact.DoBindings(null);
    }
}
