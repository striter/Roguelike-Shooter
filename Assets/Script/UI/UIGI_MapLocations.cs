using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_MapLocations : UIT_GridItem {
    Image m_Icon;
    int m_ChunkIndex;
    public override void Init()
    {
        base.Init();
        m_Icon = tf_Container.Find("Icon").GetComponent<Image>();
    }

    public void Play(int chunkIndex, string icon)
    {
        m_ChunkIndex = chunkIndex;
        m_Icon.sprite = GameUIManager.Instance.m_InGameSprites[icon];
    }

    public int MapCastCheck(Vector2 localPos)
    {
        if (Vector2.Distance(localPos, rectTransform.anchoredPosition) > 3.5f)
            return -1;
        return m_ChunkIndex;
    }
}
