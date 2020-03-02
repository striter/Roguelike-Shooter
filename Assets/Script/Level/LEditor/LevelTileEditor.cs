using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using LevelSetting;
[RequireComponent(typeof(HitCheckInteract))]
public class LevelTileEditor : LevelTileBase {

    public virtual bool isDataTile => false;
    BoxCollider m_Collider;
    protected Transform m_EditorModel { get; private set; }


    public void Clear()
    {
        if (m_Object)
        {
            m_Object.DoRecycle();
            m_Object = null;
        }

        if (m_Ground)
        {
            m_Ground.DoRecycle();
            m_Ground = null;
        }
    }

    public override void Init(TileAxis axis, ChunkTileData data,System.Random random)
    {
        m_EditorModel = transform.Find("EditorModel");
        m_Collider = GetComponent<BoxCollider>();
        Clear();
        base.Init(axis, data, random);

        bool showGround = false;
        bool showPillar = false;
        bool showObject = false;
        TileAxis tileSize = TileAxis.One;
        if (LevelChunkEditor.Instance.m_GameViewMode)
        {
            showGround = true;
            showPillar = true;
            showObject = true;
        }
        else
        {
            switch (LevelChunkEditor.Instance.m_EditMode)
            {
                case enum_TileSubType.Ground:
                    showGround = true;
                    showObject = true;
                    if (m_Ground) tileSize = m_Ground.GetDirectionedSize(m_Data.m_Direction);
                    break;
                case enum_TileSubType.Object:
                    showGround = true;
                    showObject = true;
                    if (m_Object) tileSize = m_Object.GetDirectionedSize(m_Data.m_Direction);
                    break;
            }
        }
        if (m_Ground) m_Ground.SetActivate(showGround);
        if (m_Object) m_Object.SetActivate(showObject);

        m_Collider.center = TileTools.GetLocalPosBySizeAxis(tileSize);
        Vector3 colliderSize = TileTools.GetUnitScaleBySizeAxis(tileSize, LevelConst.I_TileSize);
        if (LevelChunkEditor.Instance.m_EditMode == enum_TileSubType.Object && m_Object != null)
            colliderSize.y = LevelConst.I_TileSize * 3;
        m_Collider.size = colliderSize;
    }

    protected override bool WillGenerateObject(enum_TileObjectType type) => LevelChunkEditor.Instance.m_GameViewMode ?   base.WillGenerateObject(type): type != enum_TileObjectType.Invalid;

    public void RotateDirection(enum_TileDirection direction,System.Random random)
    {
        m_Data = m_Data.ChangeDirection(direction);
        Init(m_Axis, m_Data,random);
    }

}
