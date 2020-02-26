using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;

[RequireComponent(typeof(EntityDetector))]
public class GamePlayerChunkTrigger :CSimplePoolObjectMono<int> {
    public int m_DetectIndex { get; private set; } = -1;
    Action<int> OnChunkEntering;
    TileBounds m_ChunkBounds;
    public override void OnPoolInit()
    {
        base.OnPoolInit();
        GetComponent<EntityDetector>().Init(OnEntityDetect);
    }

    public void Play(int chunkIndex, TileBounds chunkBounds,Action<int> OnChunkEntering)
    {
        this.OnChunkEntering = OnChunkEntering;
        m_DetectIndex = chunkIndex;
        m_ChunkBounds = chunkBounds;
    }

    void OnEntityDetect(HitCheckEntity entity,bool triggerEntering)
    {
        if (triggerEntering)
            return;

        if (entity.m_Attacher.m_ControllType != enum_EntityController.Player)
            return;

        if (!m_ChunkBounds.Contains(GameLevelManager.Instance.GetMapAxis(entity.m_Attacher.transform.position) ))
            return;

        OnChunkEntering(m_DetectIndex);
    }
}
