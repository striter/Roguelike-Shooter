﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIM_Intro : UIMessageBoxBase{

    UIT_TextExtend m_title, m_Intro;
    protected override void Init()
    {
        base.Init();
        m_title = tf_Container.Find("Title").GetComponent<UIT_TextExtend>();
        m_Intro = tf_Container.Find("Intro").GetComponent<UIT_TextExtend>();
    }
    public void Play(string titleKey, string introKey, Action _OnConfirmClick)
    {
        base.Play(_OnConfirmClick);
        m_title.localizeKey = titleKey;
        m_Intro.localizeKey = introKey;
    }
}
