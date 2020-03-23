using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using System;

public class LevelTileItemEditorTerrainMap : TileItemBase, IObjectpool<enum_TileTerrainMap>
{
    public override enum_TileSubType m_Type => enum_TileSubType.TerrainMap;

    public enum_TileTerrainMap m_EditorTerrainMap = enum_TileTerrainMap.Invalid;
    Action<enum_TileTerrainMap, MonoBehaviour> OnRecycle;
    public void OnPoolItemInit(enum_TileTerrainMap identity, Action<enum_TileTerrainMap, MonoBehaviour> OnRecycle)
    {
        Init();
        this.OnRecycle = OnRecycle;
    }

    public override void DoRecycle() => OnRecycle(m_EditorTerrainMap, this);
}
