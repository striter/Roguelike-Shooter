using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIM_Intro : UIMessageBoxBase{

    UIT_TextExtend m_title, m_Intro;
    UIT_TextExtend txt_Confirm;
    protected override void Init()
    {
        base.Init();
        m_title = tf_Container.Find("Title").GetComponent<UIT_TextExtend>();
        m_Intro = tf_Container.Find("Intro").GetComponent<UIT_TextExtend>();
        txt_Confirm = tf_Container.Find("Confirm/Text").GetComponent<UIT_TextExtend>();
    }
    public void Play(string titleKey, string introKey, string confirmKey, Action _OnConfirmClick)
    {
        base.Play(_OnConfirmClick);
        m_title.localizeText = titleKey;
        m_Intro.localizeText = introKey;
        txt_Confirm.localizeText = confirmKey;
    }
}
