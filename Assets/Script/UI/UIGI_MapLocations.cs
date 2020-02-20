using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_MapLocations : UIT_GridItem {
    Image m_Icon;
    Action<int> OnItemClick;
    int m_ChunkIndex;
    public override void Init()
    {
        base.Init();
        m_Icon = tf_Container.Find("Icon").GetComponent<Image>();
    }

    public void Play(int chunkIndex, string icon,Action<int> OnItemClick)
    {
        m_ChunkIndex = chunkIndex;
        m_Icon.sprite = GameUIManager.Instance.m_InGameSprites[icon];
        this.OnItemClick = OnItemClick;
    }

    public void MapCastCheck(Vector2 localPos)
    {
        if (Vector2.Distance(localPos, rectTransform.anchoredPosition) > 5f)
            return;
        OnItemClick(m_ChunkIndex);
    }
}
