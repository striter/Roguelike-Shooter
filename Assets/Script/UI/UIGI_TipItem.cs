﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UIGI_TipItem : UIT_GridItem {
    UIT_TextExtend m_TipText;
    Action<int> OnPlayFinished;
    RectTransform m_Rect;
    float f_timeCheck;
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_TipText = rtf_Container.Find("TipText").GetComponent<UIT_TextExtend>();
        m_Rect = GetComponent<RectTransform>();
    }

    public UIT_TextExtend Play(enum_UITipsType type,Action<int> _OnPlayFinished)
    {
        m_TipText.color = type.TipsColor();
        OnPlayFinished = _OnPlayFinished;
        f_timeCheck = 2f;
        return m_TipText;
    }

    private void Update()
    {
        if (f_timeCheck <= 0)
            return;
        f_timeCheck -= Time.unscaledDeltaTime;

        float value = f_timeCheck / 2f;
        m_Rect.SetAnchor(Vector2.Lerp(new Vector2(.5f, .75f), new Vector2(.5f, .8f), 1-value));
        m_TipText.color = TCommon.ColorAlpha(m_TipText.color, value);

        if (f_timeCheck <= 0)
            OnPlayFinished(m_Identity);
    }
}
