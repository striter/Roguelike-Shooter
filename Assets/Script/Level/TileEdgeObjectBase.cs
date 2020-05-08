using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using UnityEngine;

public class TileEdgeObjectBase : TileItemBase,IObjectPoolStaticBase<enum_TileEdgeObjectType> {
    public override enum_TileSubType m_Type => enum_TileSubType.EdgeObject;
    public enum_TileEdgeObjectType m_EdgeObjectType = enum_TileEdgeObjectType.Invalid;
    Action<enum_TileEdgeObjectType, MonoBehaviour> OnRecycle;

    public override void DoRecycle() => OnRecycle(m_EdgeObjectType, this);

    public void OnPoolItemInit(enum_TileEdgeObjectType identity, Action<enum_TileEdgeObjectType, MonoBehaviour> OnRecycle)
    {
        Init();
        this.OnRecycle = OnRecycle;
    }
}
