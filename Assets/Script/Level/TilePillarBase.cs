using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using UnityEngine;

public class TilePillarBase : TileItemBase,IObjectpool<enum_TilePillarType> {
    public enum_TilePillarType m_PillarType = enum_TilePillarType.Default;
    public override enum_TileSubType m_Type => enum_TileSubType.Pillar;
    Action<enum_TilePillarType, MonoBehaviour> OnRecycle;
    public void OnPoolItemInit(enum_TilePillarType identity, Action<enum_TilePillarType, MonoBehaviour> _OnRecycle)
    {
        Init();
        m_PillarType = identity;
        this.OnRecycle = _OnRecycle;
    }

    public override void DoRecycle()
    {
        OnRecycle(m_PillarType, this);
    }
}
