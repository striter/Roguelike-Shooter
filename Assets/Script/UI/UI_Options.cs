using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UI_Options : UIPageBase {
    class ButtonToggle
    {
        Func<bool> OnToggle;
        Image m_On, m_Off;
        public ButtonToggle(Transform _transform, Func<bool> _OnToggle, bool showState)
        {
            _transform.GetComponent<Button>().onClick.AddListener(OnButtonClick);
            m_On = _transform.Find("On").GetComponent<Image>();
            m_Off = _transform.Find("Off").GetComponent<Image>();
            OnToggle = _OnToggle;
            SetShowState(showState);
        }
        void SetShowState(bool showState)
        {
            m_On.SetActivate(showState);
            m_Off.SetActivate(!showState);
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
    ButtonToggle btn_FrameRate, btn_region, btn_joyStickMode;
    SliderStatus sld_Sensitive, sld_MusicVolume, sld_VFXVolume;
    Button btn_ReturnToCamp;

    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        tf_Basic = tf_Container.Find("Basic");
        btn_FrameRate = new ButtonToggle(tf_Basic.Find("FrameRate/BtnToggle"), OnFrequencyClicked, frameRateOn());
        btn_region = new ButtonToggle(tf_Basic.Find("Region/BtnToggle"), OnRegionClicked, regionOn());
        sld_MusicVolume = new SliderStatus(tf_Basic.Find("MusicVolume/Slider"), OnMusicVolumeChanged, OptionsManager.m_OptionsData.m_MusicVolumeTap);
        sld_VFXVolume = new SliderStatus(tf_Basic.Find("VFXVolume/Slider"), OnVFXVolumeChanged, OptionsManager.m_OptionsData.m_VFXVolumeTap);

        tf_Control = tf_Container.Find("Control");
        btn_joyStickMode = new ButtonToggle(tf_Control.Find("JoystickMode/BtnToggle"), OnJoystickClicked, joystickOn());
        sld_Sensitive = new SliderStatus(tf_Control.Find("Sensitive/Slider"), OnSensitiveChanged, OptionsManager.m_OptionsData.m_SensitiveTap);
        btn_ReturnToCamp = tf_Control.Find("ReturnToCamp/BtnReturnToCamp").GetComponent<Button>();
        btn_ReturnToCamp.onClick.AddListener(OnMainmenuBtnClick);
    }

    public void SetInGame(bool inGame)
    {
        btn_ReturnToCamp.SetActivate(inGame);
    }

    bool frameRateOn() => OptionsManager.m_OptionsData.m_FrameRate == enum_Option_FrameRate.Normal;
    bool OnFrequencyClicked()
    {
        OptionsManager.m_OptionsData.m_FrameRate = frameRateOn() ? enum_Option_FrameRate.High : enum_Option_FrameRate.Normal;
        return frameRateOn();
    }

    bool regionOn() => OptionsManager.m_OptionsData.m_Region == enum_Option_LanguageRegion.EN;
    bool OnRegionClicked()
    {
        OptionsManager.m_OptionsData.m_Region = regionOn() ? enum_Option_LanguageRegion.CN : enum_Option_LanguageRegion.EN;
        return regionOn();
    }
    bool joystickOn() => OptionsManager.m_OptionsData.m_JoyStickMode == enum_Option_JoyStickMode.Retarget;
    bool OnJoystickClicked()
    {
        OptionsManager.m_OptionsData.m_JoyStickMode = joystickOn() ? enum_Option_JoyStickMode.Stational : enum_Option_JoyStickMode.Retarget;
        return joystickOn();
    }
    
    void OnMusicVolumeChanged(float value) => OptionsManager.m_OptionsData.m_MusicVolumeTap = (int)value;
    void OnVFXVolumeChanged(float value) => OptionsManager.m_OptionsData.m_VFXVolumeTap = (int)value;
    void OnSensitiveChanged(float value) => OptionsManager.m_OptionsData.m_SensitiveTap = (int)value;
    void OnMainmenuBtnClick()=> UIT_MessageBox.Instance.Begin("UI_Title_ExitGame","UI_Intro_ExitGame","UI_Option_ExitGameConfirm",GameManager.Instance.OnExitGame);

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        OptionsManager.Save();
    }

}
