using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UIGI_TipItem : UIT_GridItem {
    UIT_TextExtend m_TipText;
    Action<int> OnPlayFinished;
    Animation m_Animation;
    public override void Init()
    {
        base.Init();
        m_TipText = tf_Container.Find("TipText").GetComponent<UIT_TextExtend>();
        m_Animation = GetComponent<Animation>();
    }

    public void ShowTips(string key,enum_UITipsType type,Action<int> _OnPlayFinished)
    {
        m_TipText.color = type.TipsColor();
        m_TipText.localizeKey = key;
        OnPlayFinished = _OnPlayFinished;
        m_Animation.Play();
    }

    void OnAnimFinished()=>OnPlayFinished(I_Index);
}
