using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using System;

public class LevelTileEditorGround : TileItemBase, IObjectpool<enum_EditorGroundType>
{
    public override enum_TileSubType m_Type => enum_TileSubType.EditorGround;

    public enum_EditorGroundType m_EditorGroundType = enum_EditorGroundType.Invalid;
    Action<enum_EditorGroundType, MonoBehaviour> OnRecycle;
    public void OnPoolItemInit(enum_EditorGroundType identity, Action<enum_EditorGroundType, MonoBehaviour> OnRecycle)
    {
        Init();
        this.OnRecycle = OnRecycle;
    }
    public override void DoRecycle() => OnRecycle(m_EditorGroundType, this);
}
