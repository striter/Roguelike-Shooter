using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LevelSetting;
public class UIGI_MapLocations : UIT_GridItem {
    Image m_EventIcon;

    public override void Init()
    {
        base.Init();
        m_EventIcon = rtf_Container.Find("Icon").GetComponent<Image>();
    }

    public void Play(InteractGameBase gameChunk)
    {
        m_EventIcon.sprite = GameUIManager.Instance.m_InGameSprites[gameChunk.GetInteractMapIcon()];
    }

    public int MapCastCheck(Vector2 localPos)
    {
        if (Vector2.Distance(localPos, rectTransform.anchoredPosition) > 3.5f)
            return -1;
        return m_Index;
    }
}
