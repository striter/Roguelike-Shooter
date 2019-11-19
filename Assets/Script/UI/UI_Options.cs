using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UI_Options : UIPageBase {
    class BoolToggle : ValueToggle<bool>
    {
        static readonly List<bool> values = new List<bool> { true, false };
        public BoolToggle(Transform _transform, Func<bool> _OnToggle, bool showState):base(_transform,_OnToggle,showState,values)
        {

        }
    }
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
        void OnButtonClick()
        {
            SetShowState(OnToggle());
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

    Transform tf_Basic, tf_Control;
    Transform tf_ReturnToCamp;

    protected override void Init()
    {
        base.Init();
        tf_Basic = tf_Container.Find("Basic");
        new BoolToggle(tf_Basic.Find("FrameRate/BtnToggle"), OnFrequencyClicked, GetFrameRateOption());
        new ValueToggle<enum_Option_ScreenEffect>(tf_Basic.Find("ScreenEffect/BtnToggle"), OnScreenEffectClicked, GetScreenEffectOption(),new List<enum_Option_ScreenEffect> {  enum_Option_ScreenEffect.Normal, enum_Option_ScreenEffect.High, enum_Option_ScreenEffect.Epic});
        new BoolToggle(tf_Basic.Find("Region/BtnToggle"), OnRegionClicked, GetRegionOption());
        new SliderStatus(tf_Basic.Find("MusicVolume/Slider"), OnMusicVolumeChanged, OptionsManager.m_OptionsData.m_MusicVolumeTap);
         new SliderStatus(tf_Basic.Find("VFXVolume/Slider"), OnVFXVolumeChanged, OptionsManager.m_OptionsData.m_VFXVolumeTap);

        tf_Control = tf_Container.Find("Control");
         new BoolToggle(tf_Control.Find("JoystickMode/BtnToggle"), OnJoystickClicked, GetJoystickOption());
         new SliderStatus(tf_Control.Find("Sensitive/Slider"), OnSensitiveChanged, OptionsManager.m_OptionsData.m_SensitiveTap);
        tf_ReturnToCamp = tf_Control.Find("ReturnToCamp");
        tf_ReturnToCamp.Find("BtnReturnToCamp").GetComponent<Button>().onClick.AddListener(OnMainmenuBtnClick);
    }

    public void SetInGame(bool inGame)
    {
        tf_ReturnToCamp.SetActivate(inGame);
    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        OptionsManager.Save();
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
        if (OptionsManager.m_OptionsData.m_ScreenEffect > enum_Option_ScreenEffect.Epic)
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
    void OnMainmenuBtnClick()=> UIManager.Instance.ShowMessageBox<UIM_Intro>().Play("UI_Title_ExitGame","UI_Intro_ExitGame","UI_Option_ExitGameConfirm",GameManager.Instance.OnExitGame);

}
