using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LevelSetting;
public class UIGI_MapLocations : UIT_GridItem {
    Transform m_Event, m_Teleport;
    TSpecialClasses.AnimationControlBase m_Animation;
    Image m_EventIcon;
    public override void Init()
    {
        base.Init();
        m_Teleport = tf_Container.Find("Teleport");
        m_Event = tf_Container.Find("Event");
        m_EventIcon = m_Event.Find("Icon").GetComponent<Image>();
        m_Animation = new TSpecialClasses.AnimationControlBase(tf_Container.Find("Teleport").GetComponent<Animation>());
    }

    public void Play(GameChunk gameChunk)
    {
        m_Teleport.SetActivate(gameChunk.m_ChunkType == enum_ChunkType.Teleport);
        m_Event.SetActivate(gameChunk.m_ChunkType == enum_ChunkType.Event);
        switch (gameChunk.m_ChunkType)
        {
            case enum_ChunkType.Teleport:
                m_Animation.Play(true);
                break;
            case enum_ChunkType.Event:
                m_EventIcon.sprite = GameUIManager.Instance.m_InGameSprites[gameChunk.GetChunkMapLocationSprite];
                break;
        }
    }

    public int MapCastCheck(Vector2 localPos)
    {
        if (Vector2.Distance(localPos, rectTransform.anchoredPosition) > 3.5f)
            return -1;
        return m_Index;
    }
}
