using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractGameQuadrant : InteractGameBase {

    public int m_RelativeChunkIndex { get; private set; } = -1;
    protected bool m_ChunkRelativeInteract => m_RelativeChunkIndex > 0;
    protected void PlayQuadrant(int chunkIndex)
    {
        base.Play();
        m_RelativeChunkIndex = chunkIndex;
        if (m_ChunkRelativeInteract)
            OnQuadrantPlay();
    }
    void CheckQuadrantShow(List<int> chunkIndexShow) => OnQuadrantShow(chunkIndexShow.Contains(m_RelativeChunkIndex));
    protected virtual void OnQuadrantShow(bool show)
    {
        transform.SetActivate(show);
    }

    protected override void OnRecycle()
    {
        base.OnRecycle();
        if (m_ChunkRelativeInteract)
            OnQuadrantStop();
    }

    protected virtual void OnQuadrantPlay()
    {
        TBroadCaster<enum_BC_GameStatus>.Add<List<int>>(enum_BC_GameStatus.OnChunkQuadrantCheck, CheckQuadrantShow);
    }
    protected virtual void OnQuadrantStop()
    {
        TBroadCaster<enum_BC_GameStatus>.Remove<List<int>>(enum_BC_GameStatus.OnChunkQuadrantCheck, CheckQuadrantShow);
        m_RelativeChunkIndex = -1;
    }

    private void OnDestroy()
    {
        if (m_ChunkRelativeInteract)
            OnQuadrantStop();
    }

}
