using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(EntityDetector))]
public class ChunkPlayerTrigger :CSimplePoolObjectMono<int> {
    public int m_DetectIndex { get; private set; } = -1;
    public bool m_Entered { get; private set; } = false;
    Action<int> OnChunkEnter;
    Bounds m_ChunkBounds;
    public override void OnPoolInit()
    {
        base.OnPoolInit();
        GetComponent<EntityDetector>().Init(OnEntityDetect);
    }

    public void Play(int chunkIndex,Bounds chunkBounds,Action<int> OnChunkEnter)
    {
        this.OnChunkEnter = OnChunkEnter;
        m_DetectIndex = chunkIndex;
        m_Entered = false;
        m_ChunkBounds = chunkBounds;
    }

    void OnEntityDetect(HitCheckEntity entity,bool triggerEntering)
    {
        if (m_Entered)
            return;

        if (triggerEntering)
            return;

        if (entity.m_Attacher.m_ControllType != enum_EntityController.Player)
            return;

        if (!m_ChunkBounds.Contains(entity.m_Attacher.transform.position))
            return;

        m_Entered = true;
        OnChunkEnter(m_DetectIndex);
    }
}
