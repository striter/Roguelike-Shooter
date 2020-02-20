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
        m_Icon.GetComponent<Button>().onClick.AddListener(()=> { OnItemClick(m_ChunkIndex); });
    }

    public void Play(int chunkIndex, string icon,Action<int> OnItemClick)
    {
        m_ChunkIndex = chunkIndex;
        m_Icon.sprite = GameUIManager.Instance.m_InGameSprites[icon];
        this.OnItemClick = OnItemClick;
    }
}
