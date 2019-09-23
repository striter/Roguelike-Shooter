using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UI_Options : UIPageBase {
    Button btn_Mainmenu;
    Slider sld_Sensitive, sld_Music, sld_VFX;
    UIT_GridDefaultSingle<UIT_GridDefaultItem> m_FrameRateGrid, m_JoyStickModeGrid,m_RegionGrid;

    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        btn_Mainmenu = tf_Container.Find("BtnMainmenu").GetComponent<Button>();
        btn_Mainmenu.onClick.AddListener(OnMainmenuBtnClick);
        sld_Music = tf_Container.Find("Music").GetComponent<Slider>();
        sld_Music.onValueChanged.AddListener(OnMusicVolumeChanged);
        sld_VFX = tf_Container.Find("VFX").GetComponent<Slider>();
        sld_VFX.onValueChanged.AddListener(OnVFXVolumeChanged);
        sld_Sensitive = tf_Container.Find("Sensitive").GetComponent<Slider>();
        sld_Sensitive.onValueChanged.AddListener(OnSensitiveChanged);
        m_JoyStickModeGrid = new UIT_GridDefaultSingle<UIT_GridDefaultItem>(tf_Container.Find("JoystickMode"),OnJoyStickModeClicked);
        m_FrameRateGrid = new UIT_GridDefaultSingle<UIT_GridDefaultItem>(tf_Container.Find("FrameRate"),OnFrameRateModeClicked);
        m_RegionGrid = new UIT_GridDefaultSingle<UIT_GridDefaultItem>(tf_Container.Find("Region"),OnRegionClicked);
    }

    public void SetInGame(bool inGame)
    {
        btn_Mainmenu.SetActivate(inGame);
        m_JoyStickModeGrid.ClearGrid();
        sld_Music.value = OptionsManager.m_OptionsData.m_MusicVolume;
        sld_Sensitive.value = OptionsManager.m_OptionsData.m_Sensitive;
        sld_VFX.value = OptionsManager.m_OptionsData.m_VFXVolume;
        TCommon.TraversalEnum((enum_Option_JoyStickMode joyStick) => { m_JoyStickModeGrid.AddItem((int)joyStick).SetItemInfo(TLocalization.GetKeyLocalized( joyStick.GetLocalizeKey())); });
        m_JoyStickModeGrid.OnItemClick((int)OptionsManager.m_OptionsData.m_JoyStickMode);
        TCommon.TraversalEnum((enum_Option_FrameRate frameRate) => { m_FrameRateGrid.AddItem((int)frameRate).SetItemInfo(TLocalization.GetKeyLocalized(frameRate.GetLocalizeKey())); });
        m_FrameRateGrid.OnItemClick((int)OptionsManager.m_OptionsData.m_FrameRate);
        TCommon.TraversalEnum((enum_Option_LanguageRegion region) => { m_RegionGrid.AddItem((int)region).SetItemInfo(TLocalization.GetKeyLocalized(region.GetLocalizeKey())); });
        m_RegionGrid.OnItemClick((int)OptionsManager.m_OptionsData.m_Region);
    }

    void OnJoyStickModeClicked(int joyStick)=> OptionsManager.m_OptionsData.m_JoyStickMode = (enum_Option_JoyStickMode)joyStick;
    void OnFrameRateModeClicked(int frameRate) => OptionsManager.m_OptionsData.m_FrameRate = (enum_Option_FrameRate)frameRate;
    void OnRegionClicked(int region) => OptionsManager.m_OptionsData.m_Region = (enum_Option_LanguageRegion)region;
    void OnMusicVolumeChanged(float value) => OptionsManager.m_OptionsData.m_MusicVolume = value;
    void OnVFXVolumeChanged(float value) => OptionsManager.m_OptionsData.m_VFXVolume = value;
    void OnSensitiveChanged(float value) => OptionsManager.m_OptionsData.m_Sensitive = value;
    void OnMainmenuBtnClick()=> GameManager.Instance.OnExitGame();

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        OptionsManager.Save();
    }

}
