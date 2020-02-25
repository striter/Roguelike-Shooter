using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;

public class TileGroundBase : TileItemBase,IObjectpool<enum_TileGroundType> {
    public enum_TileGroundType m_GroundType = enum_TileGroundType.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Ground;
    Action<enum_TileGroundType, MonoBehaviour> OnRecycle;
    protected override int GetTexSelection(System.Random random) => m_GroundType.GetTexSelection(random);
    public void OnPoolItemInit(enum_TileGroundType identity, Action<enum_TileGroundType, MonoBehaviour> OnRecycle)
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
