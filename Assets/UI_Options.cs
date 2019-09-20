using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Options : UIPageBase {
    Button btn_Mainmenu;
    Slider sld_Sensitive;
    ToggleGroup tgg_FrameRate, tgg_JoystickMode;
    Toggle tg_Music, tg_VFX;

    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        tg_Music = tf_Container.Find("Music").GetComponent<Toggle>();
        tg_VFX = tf_Container.Find("VFX").GetComponent<Toggle>();
        tgg_JoystickMode = tf_Container.Find("JoyttickMode").GetComponent<ToggleGroup>();
        tgg_FrameRate = tf_Container.Find("FrameRate").GetComponent<ToggleGroup>();
        sld_Sensitive = tf_Container.Find("Sensitive").GetComponent<Slider>();
        btn_Mainmenu = tf_Container.Find("Mainmenu").GetComponent<Button>();
    }

    public void SetInGame(bool inGame)
    {
        btn_Mainmenu.SetActivate(inGame);

    }
    
}
