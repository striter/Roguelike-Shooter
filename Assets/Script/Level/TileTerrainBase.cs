using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;

public class TileTerrainBase : TileItemBase,IObjectPoolStaticBase<enum_TileTerrainType> {
    public enum_TileTerrainType m_GroundType = enum_TileTerrainType.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Terrain;
    Action<enum_TileTerrainType, MonoBehaviour> OnRecycle;
    public void OnPoolItemInit(enum_TileTerrainType identity, Action<enum_TileTerrainType, MonoBehaviour> OnRecycle)
    {
        Init();
        m_GroundType = identity;
        this.OnRecycle = OnRecycle;
    }

    public void OnPoolItemRecycle()
    {
    }

    public override void DoRecycle()
    {
        OnRecycle(m_GroundType,this);
    }

}
