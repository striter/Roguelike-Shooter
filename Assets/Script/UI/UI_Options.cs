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
        bool m_highLight;
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
    Button btn_ReturnToCamp;

    protected override void Init()
    {
        base.Init();
        Transform tf_selection = tf_Container.Find("Selections");
        Transform tf_page = tf_Container.Find("Pages");
        TCommon.TraversalEnum((enum_OptionSelection selection) =>
        {
            m_Selections.Add(selection,new SelectionItem(tf_selection.Find(selection.ToString()),selection,OnSelectionClick));
            m_Page.Add(selection, tf_page.Find(selection.ToString()));
        });
        OnSelectionClick(enum_OptionSelection.Display);
        
        new BoolToggle(m_Page[enum_OptionSelection.Display].Find("FrameRate/BtnToggle"), OnFrequencyClicked, GetFrameRateOption());
        new ValueToggle<enum_Option_ScreenEffect>(m_Page[enum_OptionSelection.Display].Find("ScreenEffect/BtnToggle"), OnScreenEffectClicked, GetScreenEffectOption(),new List<enum_Option_ScreenEffect> {  enum_Option_ScreenEffect.Normal, enum_Option_ScreenEffect.High});
        new BoolToggle(m_Page[enum_OptionSelection.Display].Find("Region/BtnToggle"), OnRegionClicked, GetRegionOption());
        new SliderStatus(m_Page[enum_OptionSelection.Sound].Find("MusicVolume/Slider"), OnMusicVolumeChanged, OptionsManager.m_OptionsData.m_MusicVolumeTap);
        new SliderStatus(m_Page[enum_OptionSelection.Sound].Find("VFXVolume/Slider"), OnVFXVolumeChanged, OptionsManager.m_OptionsData.m_VFXVolumeTap);
        new BoolToggle(m_Page[enum_OptionSelection.Control].Find("JoystickMode/BtnToggle"), OnJoystickClicked, GetJoystickOption());
        new SliderStatus(m_Page[enum_OptionSelection.Control].Find("Sensitive/Slider"), OnSensitiveChanged, OptionsManager.m_OptionsData.m_SensitiveTap);

        btn_ReturnToCamp = tf_Container.Find("BtnReturn").GetComponent<Button>();
        btn_ReturnToCamp.onClick.AddListener(OnMainmenuBtnClick);

        m_Page[enum_OptionSelection.Display].Find("Region").SetActivate(false);     //Test
    }

    public void SetInGame(bool inGame)
    {
        btn_ReturnToCamp.SetActivate(inGame);
    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        OptionsManager.Save();
    }

    void OnSelectionClick(enum_OptionSelection curselection)
    {
        m_currentSelection = curselection;
        m_Selections.Traversal((enum_OptionSelection selection, SelectionItem item)=> { item.SetHighligh(m_currentSelection == item.m_selection); });
        m_Page.Traversal((enum_OptionSelection selection, Transform item) => { item.SetActivate(selection == m_currentSelection); });
    }

    bool GetFrameRateOption() =>  OptionsManager.m_OptionsData.m_FrameRate == enum_Option_FrameRate.Normal;
    bool OnFrequencyClicked()
    {
        OptionsManager.m_OptionsData.m_FrameRate = GetFrameRateOption() ? enum_Option_FrameRate.High : enum_Option_FrameRate.Normal;
        OptionsManager.OnOptionChanged();
        return GetFrameRateOption();
    }

    enum_Option_ScreenEffect GetScreenEffectOption() => OptionsManager.m_OptionsData.m_ScreenEffect;
    enum_Option_ScreenEffect OnScreenEffectClicked()
    {
        OptionsManager.m_OptionsData.m_ScreenEffect++;
        if (OptionsManager.m_OptionsData.m_ScreenEffect > enum_Option_ScreenEffect.High)
            OptionsManager.m_OptionsData.m_ScreenEffect = enum_Option_ScreenEffect.Normal;
        OptionsManager.OnOptionChanged();
        return GetScreenEffectOption();
    }

    bool GetRegionOption() => OptionsManager.m_OptionsData.m_Region == enum_Option_LanguageRegion.EN;
    bool OnRegionClicked()
    {
        OptionsManager.m_OptionsData.m_Region = GetRegionOption() ? enum_Option_LanguageRegion.CN : enum_Option_LanguageRegion.EN;
        OptionsManager.OnOptionChanged();
        return GetRegionOption();
    }
    bool GetJoystickOption() => OptionsManager.m_OptionsData.m_JoyStickMode == enum_Option_JoyStickMode.Retarget;
    bool OnJoystickClicked()
    {
        OptionsManager.m_OptionsData.m_JoyStickMode = GetJoystickOption() ? enum_Option_JoyStickMode.Stational : enum_Option_JoyStickMode.Retarget;
        OptionsManager.OnOptionChanged();
        return GetJoystickOption();
    }

    void OnMusicVolumeChanged(float value)
    {
        OptionsManager.m_OptionsData.m_MusicVolumeTap = (int)value;
        OptionsManager.OnOptionChanged();
    }
    void OnVFXVolumeChanged(float value)
    {
        OptionsManager.m_OptionsData.m_VFXVolumeTap = (int)value;
        OptionsManager.OnOptionChanged();
    }
    void OnSensitiveChanged(float value)
    {
        OptionsManager.m_OptionsData.m_SensitiveTap = (int)value;
        OptionsManager.OnOptionChanged();
    } 
    void OnMainmenuBtnClick()=> UIManager.Instance.ShowMessageBox<UIM_Intro>().Play("UI_Title_ExitGame","UI_Intro_ExitGame",GameManager.Instance.OnExitGame);

}
