using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(EntityDetector))]
public class GamePlayerTrigger :CSimplePoolObjectMono<int> {
    public int m_DetectIndex { get; private set; } = -1;
    public bool m_Entered { get; private set; } = false;
    Action<int> OnChunkEnter;
    public override void OnPoolInit()
    {
        base.OnPoolInit();
        GetComponent<EntityDetector>().Init(OnEntityDetect);
    }

    public void Play(int chunkIndex,Action<int> OnChunkEnter)
    {
        this.OnChunkEnter = OnChunkEnter;
        m_DetectIndex = chunkIndex;
        m_Entered = false;
    }

    void OnEntityDetect(HitCheckEntity entity,bool enter)
    {
        if (m_Entered)
            return;

        if (entity.m_Attacher.m_ControllType != GameSetting.enum_EntityController.Player)
            return;
        m_Entered = true;
        OnChunkEnter(m_DetectIndex);
    }
}
