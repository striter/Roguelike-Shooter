using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using UnityEngine;

public class TileEdgeObjectBase : TileItemBase,IObjectpool<enum_TileEdgeObject> {
    public override enum_TileSubType m_Type => enum_TileSubType.EdgeObject;
    public enum_TileEdgeObject m_EdgeObjectType = enum_TileEdgeObject.Invalid;
    Action<enum_TileEdgeObject, MonoBehaviour> OnRecycle;

    public override void DoRecycle() => OnRecycle(m_EdgeObjectType, this);

    public void OnPoolItemInit(enum_TileEdgeObject identity, Action<enum_TileEdgeObject, MonoBehaviour> OnRecycle)
    {
        Init();
        this.OnRecycle = OnRecycle;
    }
}
