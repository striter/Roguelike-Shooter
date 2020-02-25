using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroundBlockLift : TileGroundBase
{
    public bool m_Lift { get; private set; }
    TSpecialClasses.AnimationControlBase m_Animation;
    Collider m_BlockCollider;
    public override void Init(ChunkTileData _data, System.Random random)
    {
        base.Init(_data, random);
        m_Animation = new TSpecialClasses.AnimationControlBase(GetComponent<Animation>(), true);
        m_BlockCollider = m_Model.Find("BlockCollider").GetComponent<Collider>();
        m_BlockCollider.enabled = false;
    }
    public void SetLift(bool lift)
    {
        m_Lift = lift;
        m_Animation.Play(lift);
        m_BlockCollider.enabled = lift;
    }
}
