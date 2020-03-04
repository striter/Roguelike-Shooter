using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using LevelSetting;
using System;

public class LevelTileEditorData : LevelTileEditor {

    public override bool isDataTile => true;
    public bool m_ContainsInfo => m_Data.m_GroundType != enum_TileGroundType.Invalid || m_Data.m_ObjectType != enum_TileObjectType.Invalid ;
    
    public void SetData(enum_TileObjectType type,enum_TileDirection direction, System.Random random)
    {
        if (m_Data.m_ObjectType == type && m_Data.m_Direction == direction)
            return;

        m_Data = m_Data.ChangeDirection(direction);
        m_Data = m_Data.ChangeObjectType(type);
        Init(m_Axis, m_Data,random);
    }
    public void SetData(enum_TileGroundType type, enum_TileDirection direction, System.Random random)
    {
        if (m_Data.m_GroundType == type&&m_Data.m_Direction==direction)
            return;

        m_Data = m_Data.ChangeDirection(direction);
        m_Data = m_Data.ChangeGroundType(type);
        Init(m_Axis, m_Data,random);
    }


    public override void Init(TileAxis axis, ChunkTileData data, System.Random random)
    {
        base.Init(axis, data, random);
        bool showEditorModel = false;
        if (LevelChunkEditor.Instance.m_GameViewMode)
            showEditorModel = false;
        else
        {
            switch (LevelChunkEditor.Instance.m_EditMode)
            {
                case enum_TileSubType.Ground:
                    showEditorModel = m_Ground == null;
                    break;
                case enum_TileSubType.Object:
                    showEditorModel = false;
                    break;
            }
        }

        m_EditorModel.SetActivate(showEditorModel);
    }
}
