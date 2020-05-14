using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using System;

public class LevelTileItemEditorTerrain : TileItemBase, IObjectPoolStaticBase<enum_EditorTerrainType>
{
    public override enum_TileSubType m_Type => enum_TileSubType.EditorGround;

    public enum_EditorTerrainType m_EditorTerrainType = enum_EditorTerrainType.Invalid;
    Action<enum_EditorTerrainType, MonoBehaviour> OnRecycle;
    public void OnPoolItemInit(enum_EditorTerrainType identity, Action<enum_EditorTerrainType, MonoBehaviour> OnRecycle)
    {
        Init();
        this.OnRecycle = OnRecycle;
    }
    public void OnPoolItemRecycle()
    {
    }
    public override void DoRecycle() => OnRecycle(m_EditorTerrainType, this);

}
