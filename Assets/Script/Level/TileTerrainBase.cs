using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;

public class TileTerrainBase : TileItemBase,IObjectpool<enum_TileTerrain> {
    public enum_TileTerrain m_GroundType = enum_TileTerrain.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Terrain;
    Action<enum_TileTerrain, MonoBehaviour> OnRecycle;
    public void OnPoolItemInit(enum_TileTerrain identity, Action<enum_TileTerrain, MonoBehaviour> OnRecycle)
    {
        Init();
        m_GroundType = identity;
        this.OnRecycle = OnRecycle;
    }

    public override void DoRecycle()
    {
        OnRecycle(m_GroundType,this);
    }
}
