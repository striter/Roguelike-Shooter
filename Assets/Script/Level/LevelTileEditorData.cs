using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using LevelSetting;
public class LevelTileEditorData : LevelTileEditor {

    public override bool isDataTile => true;
    public bool m_ContainsInfo => m_Data.m_GroundType != enum_TileGroundType.Invalid || m_Data.m_ObjectType != enum_TileObjectType.Invalid || m_Data.m_PillarType != enum_TilePillarType.Invalid;


    public void SetData(enum_TilePillarType type, enum_TileDirection direction)
    {
        if (m_Data.m_PillarType == type && m_Data.m_Direction == direction)
            return;

        m_Data = m_Data.ChangePillarType(type, direction);
        Init(m_Axis, m_Data);
    }

    public void SetData(enum_TileObjectType type,enum_TileDirection direction )
    {
        if (m_Data.m_ObjectType == type && m_Data.m_Direction == direction)
            return;

        m_Data = m_Data.ChangeObjectType(type, direction);
        Init(m_Axis, m_Data);
    }
    public void SetData(enum_TileGroundType type, enum_TileDirection direction )
    {
        if (m_Data.m_GroundType == type&&m_Data.m_Direction==direction)
            return;

        m_Data = m_Data.ChangeGroundType(type, direction);
        Init(m_Axis, m_Data);
    }
    public override void OnEditSelectionChange()
    {
        base.OnEditSelectionChange();
        bool showEditorModel = false;
        if (LevelChunkEditor.Instance.m_ShowAllModel)
            showEditorModel = false;
        else
        {
            switch (LevelChunkEditor.Instance.m_EditMode)
            {
                case enum_TileSubType.Ground:
                        showEditorModel = m_Ground == null;
                    break;
                case enum_TileSubType.Object:
                        showEditorModel = m_Ground == null;
                    break;
                case enum_TileSubType.Pillar:
                        showEditorModel = m_Pillar == null;
                    break;
            }
        }

        m_EditorModel.SetActivate(showEditorModel);
    }
}
