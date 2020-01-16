using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;

public class TileObjectBase : TileItemBase,IPoolObject<enum_TileObjectType> {
    public enum_TileObjectType m_ObjectType= enum_TileObjectType.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Object;
    Action<enum_TileObjectType, MonoBehaviour> OnRecycle;
    public override TileAxis GetDirectionedSize(enum_TileDirection direction) => m_ObjectType.GetSizeAxis(direction);
    protected override int GetTexSelection(System.Random random) => m_ObjectType.GetTexSelection(random);
    public void OnPoolItemInit(enum_TileObjectType identity, Action<enum_TileObjectType, MonoBehaviour> OnRecycle)
    {
        m_ObjectType = identity;
        this.OnRecycle = OnRecycle;
    }

    public override void DoRecycle()
    {
        OnRecycle(m_ObjectType, this);
    }
    
}
