using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using UnityEngine;

public class TileObjectBlockLift : TileObjectBase
{
    public bool m_Lift { get; private set; }
    TSpecialClasses.AnimationClipControl m_Animation;
    Collider m_BlockCollider;
    protected override void Init()
    {
        base.Init();
        m_Animation = new TSpecialClasses.AnimationClipControl(GetComponent<Animation>(), true);
        m_BlockCollider = m_Model.Find("BlockCollider").GetComponent<Collider>();
    }
    public override void OnGenerateItem(ChunkTileData _data, System.Random random)
    {
        base.OnGenerateItem(_data, random);
        m_BlockCollider.enabled = false;
    }
    public void SetLift(bool lift)
    {
        m_Lift = lift;
        m_Animation.Play(lift);
        m_BlockCollider.enabled = lift;
    }
}
