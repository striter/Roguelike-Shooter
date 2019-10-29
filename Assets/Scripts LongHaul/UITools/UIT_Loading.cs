using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIT_Loading : SimpleSingletonMono<UIT_Loading> {
    Image m_LoadingImage;
    float m_loadDuration;
    float m_loadCheck;
    bool m_loading => m_loadCheck > 0;
    Action OnLoadFinished;
    protected override void Awake()
    {
        base.Awake();
        m_LoadingImage = transform.Find("Loading").GetComponent<Image>();
        m_LoadingImage.SetActivate(false);
        m_loadCheck = -1;
    }
    public void Play(float duration, Action _OnLoadFinished)
    {
        m_loadDuration = duration;
        m_loadCheck = m_loadDuration;
        m_LoadingImage.SetActivate(true);
        m_LoadingImage.color = TCommon.ColorAlpha(m_LoadingImage.color, 0f);
        OnLoadFinished = _OnLoadFinished;
    }

    void LoadFinished()
    {
        m_LoadingImage.SetActivate(false);
        OnLoadFinished();
        OnLoadFinished = null;
    }
    private void Update()
    {
        if (!m_loading)
            return;

        m_LoadingImage.color = TCommon.ColorAlpha(m_LoadingImage.color, 1 - m_loadCheck / m_loadDuration);
        m_loadCheck -= Time.unscaledDeltaTime;

        if (!m_loading)  LoadFinished();
    }
}
