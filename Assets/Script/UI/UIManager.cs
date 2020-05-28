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
    protected UIC_PlayerStatus m_PlayerStatus { get; private set; }
    public UIC_Indicates m_Indicate { get; private set; }
    public CameraEffectManager m_Effect { get; private set; }
    public AtlasLoader m_CommonSprites { get; private set; }
    public AtlasLoader m_ExpireSprites { get; private set; }
    public AtlasLoader m_WeaponSprites { get; private set; }
    public AtlasLoader m_CharacterSprites { get; private set; }
    protected CB_GenerateTransparentOverlayTexture m_BlurBG { get; private set; }
    public static void Activate(bool inGame)
    {
        GameObject uiObj = TResources.InstantiateUIManager();
        UIManager manager = null;
        if (inGame)
            manager = uiObj.AddComponent<GameUIManager>();
        else
            manager = uiObj.AddComponent<CampUIManager>();
        manager.Init();
    }

    protected override void Init()
    {
        base.Init();
        Instance = this;
        m_OverlayBG = cvs_Overlay.transform.Find("OverlayBG").GetComponent<Image>();
        m_OverlayBG.SetActivate(false);

        m_CommonSprites = TResources.GetUIAtlas_Common();
        m_ExpireSprites = TResources.GetUIAtlas_Expires();
        m_WeaponSprites = TResources.GetUIAtlas_Weapon();
        m_CharacterSprites = TResources.GetUIAtlas_Character();

        m_Camera = transform.Find("UICamera").GetComponent<Camera>();
        m_Effect = m_Camera.GetComponent<CameraEffectManager>();
        m_BlurBG = m_Effect.GetOrAddCameraEffect<CB_GenerateTransparentOverlayTexture>().SetOpaqueBlurTextureEnabled(false,2f,3,4);

        m_PlayerStatus = ShowControls<UIC_PlayerStatus>();
        m_UIControl = ShowControls<UIC_Control>();
        m_Interact = ShowControls<UIC_PlayerInteract>();
        m_Indicate = ShowControls<UIC_Indicates>();

        PreloadPage<UI_Options>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
        this.StopAllCoroutines();
    }


    public new T ShowMessageBox<T>() where T : UIMessageBoxBase => base.ShowMessageBox<T>();
    public T ShowPage<T>(bool animate,bool blurBG, float bulletTime=1f) where T : UIPage
    {
        T page = base.ShowPage<T>(animate);
        if (page == null)
            return null;
        SetBlurBackground(blurBG);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageOpen, bulletTime);
        if (bulletTime != 1f)
            GameManagerBase.Instance.SetBaseTimeScale(bulletTime);
        return page;
    }

    protected override void OnPageExit(UIPageBase page)
    {
        base.OnPageExit(page);
        if (m_PageOpening)
            return;

        SetBlurBackground(false);
        GameManagerBase.Instance.SetBaseTimeScale(1f);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PageClose);
    }

    void SetBlurBackground(bool enable)
    {
        if (enable)
        {
            m_OverlayBG.SetActivate(true);
            m_BlurBG.SetOpaqueBlurTextureEnabled(true,2f,2,3);
            this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_OverlayBG.color = TCommon.ColorAlpha(m_OverlayBG.color, value); }, 0, 1, UIPageBase.F_AnimDuration, null, false));
        }
        else
        {
            this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
                m_OverlayBG.color = TCommon.ColorAlpha(m_OverlayBG.color, value);
            }, 1, 0, UIPageBase.F_AnimDuration, () => {
                m_BlurBG.SetOpaqueBlurTextureEnabled(false);
                m_OverlayBG.SetActivate(false);
            }, false));
        }
    }

    public void DoBindings(EntityCharacterPlayer player, Action<Vector2> _OnLeftDelta, Action<Vector2> _OnRightDelta, Action<bool> _OnMainDown,Action<bool> _OnSubDown, Action<bool> _OnCharacterAbility)
    {
        m_UIControl.DoBinding(player, _OnLeftDelta, _OnRightDelta, _OnMainDown, _OnSubDown, _OnCharacterAbility);
    }

    public void RemoveBindings()
    {
        m_UIControl.RemoveBinding();
    }
}
