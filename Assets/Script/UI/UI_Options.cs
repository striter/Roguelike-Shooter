using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UI_Options : UIPage {
    class ValueToggle<T>
    {
        Func<T> OnToggle;
        Dictionary<T, Image> m_ToggleImages=new Dictionary<T, Image>();
        public ValueToggle(Transform _transform, Func<T> _OnToggle, T showState,List<T> values)
        {
            _transform.GetComponent<Button>().onClick.AddListener(OnButtonClick);
            values.Traversal((T value) => { m_ToggleImages.Add(value, _transform.Find(value.ToString()).GetComponent<Image>()); });
            OnToggle = _OnToggle;
            SetShowState(showState);
        }
        void SetShowState(T showState)
        {
            m_ToggleImages.Traversal((T value, Image image) => { image.SetActivate(value.Equals(showState)); });
        }

        protected virtual void OnButtonClick()
        {
            SetShowState(OnToggle());
        }
    }
    class BoolToggle : ValueToggle<bool>
    {
        static readonly List<bool> values = new List<bool> { true, false };
        public BoolToggle(Transform _transform, Func<bool> _OnToggle, bool showState) : base(_transform, _OnToggle, showState, values)
        {
        }
    }
    class SelectionItem
    {
        public enum_OptionSelection m_selection { get; private set; }
        Transform m_highlight, m_dehighlight;
        Action<enum_OptionSelection> OnSelectionClick;
        public SelectionItem(Transform _transform,enum_OptionSelection _selection,Action<enum_OptionSelection> _OnSelectionClick)
        {
            m_highlight = _transform.Find("Highlight");
            m_dehighlight = _transform.Find("Dehighlight");
            m_selection = _selection;
            _transform.GetComponent<Button>().onClick.AddListener(OnButtonClick);
            OnSelectionClick = _OnSelectionClick;
            SetHighligh(false);
        }

        void OnButtonClick()
        {
            OnSelectionClick(m_selection);
        }

        public void SetHighligh(bool highlight)
        {
            m_highlight.SetActivate(highlight);
            m_dehighlight.SetActivate(!highlight);
        }
    }
    class SliderStatus
    {
        Action<float> OnAmount;
        Text m_Amount;
        public SliderStatus(Transform _transform, Action<float> _OnAmount, float startAmount)
        {

            Slider slider = _transform.GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnAmountChanged);
            m_Amount = slider.handleRect.transform.Find("Amount").GetComponent<Text>();
            OnAmount = _OnAmount;
            slider.value = startAmount;
            m_Amount.text = startAmount.ToString();
        }

        void OnAmountChanged(float value)
        {
            m_Amount.text = value.ToString();
            OnAmount(value);
        }
    }
    enum enum_OptionSelection
    {
        Invalid=-1,
        Display=1,
        Sound=2,
        Control=3
    }

    enum_OptionSelection m_currentSelection;
    Dictionary<enum_OptionSelection, SelectionItem> m_Selections=new Dictionary<enum_OptionSelection, SelectionItem>();
    Dictionary<enum_OptionSelection, Transform> m_Page=new Dictionary<enum_OptionSelection, Transform>();
    enum ButtonEvent
    {
        BtnBgm = 0,
        BtnSoundEffect = 1,
        BtnRocker = 2,
        BtnFrame = 3,
        BtnFps=4,

        BtnMax,
    }
    [SerializeField] Button btn_ReturnToCamp;
    [SerializeField] Button[] m_buttonList = new Button[(int)ButtonEvent.BtnMax];
    [SerializeField] Image[] m_musicImageList = new Image[2];
    [SerializeField] Image[] m_rockerImageList = new Image[2];
    [SerializeField] Image[] m_frameImageList = new Image[3];
    [SerializeField] Image[] m_fpsImageList = new Image[2];
    [SerializeField] UIT_TextExtend m_level, m_abilityTips, m_setUpTips;

    UIT_GridControlledSingleSelect<UIGI_DetailPerkSelect> m_PerkSelect;
    EntityCharacterPlayer m_Player;
    protected override void Init()
    {
        base.Init();
        //Transform tf_selection = rtf_Container.Find("Selections");
        //Transform tf_page = rtf_Container.Find("Pages");
        //TCommon.TraversalEnum((enum_OptionSelection selection) =>
        //{
        //    m_Selections.Add(selection,new SelectionItem(tf_selection.Find(selection.ToString()),selection,OnSelectionClick));
        //    m_Page.Add(selection, tf_page.Find(selection.ToString()));
        //});
        OnSelectionClick(enum_OptionSelection.Display);

        //new BoolToggle(m_Page[enum_OptionSelection.Display].Find("FrameRate/BtnToggle"), OnFrequencyClicked, GetFrameRateOption());
        //new ValueToggle<enum_Option_Effect>(m_Page[enum_OptionSelection.Display].Find("Effect/BtnToggle"), OnEffectClicked, GetEffectOption(), new List<enum_Option_Effect> { enum_Option_Effect.Off, enum_Option_Effect.Normal, enum_Option_Effect.High });
        //new BoolToggle(m_Page[enum_OptionSelection.Display].Find("Region/BtnToggle"), OnRegionClicked, GetRegionOption());
        //new BoolToggle(m_Page[enum_OptionSelection.Display].Find("Shadow/BtnToggle"), OnShadowClicked, GetShadowOption());
        //new SliderStatus(m_Page[enum_OptionSelection.Sound].Find("MusicVolume/Slider"), OnMusicVolumeChanged, OptionsDataManager.m_OptionsData.m_MusicVolumeTap);
        //new SliderStatus(m_Page[enum_OptionSelection.Sound].Find("VFXVolume/Slider"), OnVFXVolumeChanged, OptionsDataManager.m_OptionsData.m_VFXVolumeTap);
        //new BoolToggle(m_Page[enum_OptionSelection.Control].Find("JoystickMode/BtnToggle"), OnJoystickClicked, GetJoystickOption());
        //new SliderStatus(m_Page[enum_OptionSelection.Control].Find("Sensitive/Slider"), OnSensitiveChanged, OptionsDataManager.m_OptionsData.m_SensitiveTap);

        //btn_ReturnToCamp = rtf_Container.Find("BtnReturn").GetComponent<Button>();


        MusicSwitch(m_musicImageList[0], OptionsDataManager.m_OptionsData.m_MusicVolumeTap);
        MusicSwitch(m_musicImageList[1], OptionsDataManager.m_OptionsData.m_VFXVolumeTap);
        SetSwitch(m_rockerImageList, Convert.ToInt32(GetJoystickOption()));
        SetSwitch(m_frameImageList, (int)GetEffectOption());
        SetSwitch(m_fpsImageList, Convert.ToInt32(GetFrameRateOption()));


        for (int i = 0; i < m_buttonList.Length; i++)
        {
            int num = i;
            m_buttonList[i].onClick.AddListener(delegate () { OnClick(num); });
        }
        btn_ReturnToCamp.onClick.AddListener(OnMainmenuBtnClick);


        m_PerkSelect = new UIT_GridControlledSingleSelect<UIGI_DetailPerkSelect>(rtf_Container.Find("ScrollRect/Viewport/Content"), OnPerkSelectClick);
    }
    public override void OnPlay(bool doAnim, Action<UIPageBase> OnPageExit)
    {
        base.OnPlay(doAnim, OnPageExit);
        m_level.localizeKey = BattleManager.Instance.m_BattleProgress.m_Stage.GetLocalizeKey();
        m_abilityTips.text = "";
        m_setUpTips.text = "";
        m_PerkSelect.ClearGrid();
        m_Player = BattleManager.Instance.m_LocalPlayer;
        m_Player.m_CharacterInfo.m_ExpirePerks.Traversal((int index, ExpirePlayerPerkBase perk) => { m_PerkSelect.AddItem(index).Init(perk); });
    }
    void MusicSwitch(Image image, int num)
    {
        if (num > 0)
            image.color = Color.white;
        else
            image.color = new Color(0.5f, 0.5f, 0.5f);

    }
    void SetSwitch(Image[] imageList, int num)
    {
        for (int i = 0; i < imageList.Length; i++)
        {
            if (i == num)
            {
                imageList[i].SetActivate(true);
            }
            else
            {
                imageList[i].SetActivate(false);
            }
        }
    }
    /// <summary>
    /// 能力点击
    /// </summary>
    /// <param name="index"></param>
    void OnPerkSelectClick(int index)
    {
        ExpirePlayerPerkBase perk = m_Player.m_CharacterInfo.m_ExpirePerks[index];
        m_abilityTips.text= TLocalization.GetKeyLocalized(perk.GetNameLocalizeKey())+":" +string.Format(TLocalization.GetKeyLocalized(perk.GetDetailLocalizeKey()), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value1), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value2), string.Format("<color=#FFDA6BFF>{0}</color>", perk.Value3));

    }
    public void OnClick(int num)
    {
        switch ((ButtonEvent)num)
        {
            case ButtonEvent.BtnBgm:
                {
                    if (OptionsDataManager.m_OptionsData.m_MusicVolumeTap > 0)
                    {
                        OnMusicVolumeChanged(0);
                        m_setUpTips.localizeKey = "UI_Options_Music_Close";
                    }
                    else
                    {
                        OnMusicVolumeChanged(10);
                        m_setUpTips.localizeKey = "UI_Options_Music_Open";
                    }
                    MusicSwitch(m_musicImageList[0], OptionsDataManager.m_OptionsData.m_MusicVolumeTap);
                }
                break;
            case ButtonEvent.BtnSoundEffect:
                {
                    if (OptionsDataManager.m_OptionsData.m_VFXVolumeTap > 0)
                    {
                        OnVFXVolumeChanged(0);
                        m_setUpTips.localizeKey = "UI_Options_Soundeffect_Close";
                    }
                    else
                    {
                        OnVFXVolumeChanged(10);
                        m_setUpTips.localizeKey = "UI_Options_Soundeffect_Open";
                    }
                    MusicSwitch(m_musicImageList[1], OptionsDataManager.m_OptionsData.m_VFXVolumeTap);
                }
                break;
            case ButtonEvent.BtnRocker:
                OnJoystickClicked();
                SetSwitch(m_rockerImageList, Convert.ToInt32(GetJoystickOption()));
                m_setUpTips.localizeKey = "UI_Options_Rocker_"+ GetJoystickOption();
                break;
            case ButtonEvent.BtnFrame:
                OnEffectClicked();
                SetSwitch(m_frameImageList, (int)GetEffectOption());
                m_setUpTips.localizeKey = "UI_Options_Frame_" + (int)GetEffectOption();
                break;
            case ButtonEvent.BtnFps:
                OnFrequencyClicked();
                SetSwitch(m_fpsImageList, Convert.ToInt32(GetFrameRateOption()));
                m_setUpTips.localizeKey = "UI_Options_Fps_" + GetFrameRateOption();
                break;
        }
    }
    public void SetInGame(bool inGame)
    {
        btn_ReturnToCamp.SetActivate(inGame);
    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        OptionsDataManager.Save();
    }

    void OnSelectionClick(enum_OptionSelection curselection)
    {
        m_currentSelection = curselection;
        m_Selections.Traversal((enum_OptionSelection selection, SelectionItem item)=> { item.SetHighligh(m_currentSelection == item.m_selection); });
        m_Page.Traversal((enum_OptionSelection selection, Transform item) => { item.SetActivate(selection == m_currentSelection); });
    }
    /// <summary>
    /// 获取fps设置
    /// </summary>
    /// <returns></returns>
    bool GetFrameRateOption() =>  OptionsDataManager.m_OptionsData.m_FrameRate == enum_Option_FrameRate.Normal;
    /// <summary>
    /// 设置fps
    /// </summary>
    /// <returns></returns>
    bool OnFrequencyClicked()
    {
        OptionsDataManager.m_OptionsData.m_FrameRate = GetFrameRateOption() ? enum_Option_FrameRate.High : enum_Option_FrameRate.Normal;
        OptionsDataManager.OnOptionChanged();
        return GetFrameRateOption();
    }
    /// <summary>
    /// 获取画面效果设置
    /// </summary>
    /// <returns></returns>
    enum_Option_Effect GetEffectOption() => OptionsDataManager.m_OptionsData.m_Effect;
    /// <summary>
    /// 设置画面效果
    /// </summary>
    /// <returns></returns>
    enum_Option_Effect OnEffectClicked()
    {
        OptionsDataManager.m_OptionsData.m_Effect++;
        if (OptionsDataManager.m_OptionsData.m_Effect > enum_Option_Effect.High)
            OptionsDataManager.m_OptionsData.m_Effect = enum_Option_Effect.Off;
        OptionsDataManager.OnOptionChanged();

        if (OptionsDataManager.m_OptionsData.m_Effect == enum_Option_Effect.Off)
        {
            OnShadowClicked(true);
        }
        else
        {
            OnShadowClicked(false);
        }
        return GetEffectOption();
    }

    bool GetRegionOption() => OptionsDataManager.m_OptionsData.m_Region == enum_Option_LanguageRegion.EN;
    bool OnRegionClicked()
    {
        OptionsDataManager.m_OptionsData.m_Region = GetRegionOption() ? enum_Option_LanguageRegion.CN : enum_Option_LanguageRegion.EN;
        OptionsDataManager.OnOptionChanged();
        return GetRegionOption();
    }

    bool GetShadowOption() => OptionsDataManager.m_OptionsData.m_ShadowOff;
    bool OnShadowClicked( bool ShadowOff)
    {
        OptionsDataManager.m_OptionsData.m_ShadowOff = ShadowOff;//!OptionsDataManager.m_OptionsData.m_ShadowOff;
        OptionsDataManager.OnOptionChanged();
        return GetShadowOption();
    }
    /// <summary>
    /// 获取摇杆设置
    /// </summary>
    /// <returns></returns>
    bool GetJoystickOption() => OptionsDataManager.m_OptionsData.m_JoyStickMode == enum_Option_JoyStickMode.Retarget;
    /// <summary>
    /// 设置摇杆模式
    /// </summary>
    /// <returns></returns>
    bool OnJoystickClicked()
    {
        OptionsDataManager.m_OptionsData.m_JoyStickMode = GetJoystickOption() ? enum_Option_JoyStickMode.Stational : enum_Option_JoyStickMode.Retarget;
        OptionsDataManager.OnOptionChanged();
        return GetJoystickOption();
    }
    /// <summary>
    /// 设置音乐
    /// </summary>
    /// <param name="value"></param>
    void OnMusicVolumeChanged(float value)
    {
        OptionsDataManager.m_OptionsData.m_MusicVolumeTap = (int)value;
        OptionsDataManager.OnOptionChanged();
    }
    /// <summary>
    /// 设置音效
    /// </summary>
    /// <param name="value"></param>
    void OnVFXVolumeChanged(float value)
    {
        OptionsDataManager.m_OptionsData.m_VFXVolumeTap = (int)value;
        OptionsDataManager.OnOptionChanged();
    }
    /// <summary>
    /// 设置灵敏度
    /// </summary>
    /// <param name="value"></param>
    void OnSensitiveChanged(float value)
    {
        OptionsDataManager.m_OptionsData.m_SensitiveTap = (int)value;
        OptionsDataManager.OnOptionChanged();
    } 
    void OnMainmenuBtnClick()=> UIManager.Instance.ShowMessageBox<UIM_Intro>().Play("UI_Title_ExitGame","UI_Intro_ExitGame",BattleManager.Instance.OnGameExit);

}
