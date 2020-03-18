using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTerrainSlope3LTR : TileTerrainBase {
    Transform m_Model1,m_Model2;
    protected override void Init()
    {
        base.Init();
        m_Model1 = m_Model.Find("Model1");
        m_Model2 = m_Model.Find("Model2");

    }
    public override void OnGenerateItem(ChunkTileData _data, System.Random random)
    {
        base.OnGenerateItem(_data, random);
        bool modelShow = TCommon.RandomBool(random);
        m_Model1.SetActivate(!modelShow);
        m_Model2.SetActivate(modelShow);
    }
}
