using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using LevelSetting;
[RequireComponent(typeof(HitCheckInteract))]
public class LevelTileEditor : LevelTileNew {

    public virtual bool isDataTile => false;
    BoxCollider m_Collider;
    protected Transform m_EditorModel { get; private set; }
    public override void Init(TileAxis axis, ChunkTileData data,System.Random random)
    {
        if (m_Pillar)
        {
            m_Pillar.DoRecycle();
            m_Pillar = null;
        }
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

        base.Init(axis, data,random);
        m_EditorModel = transform.Find("EditorModel");
        m_Collider = GetComponent<BoxCollider>();
        OnEditSelectionChange();
    }
    
    public virtual void OnEditSelectionChange()
    {
        bool showGround = false;
        bool showPillar = false;
        bool showObject = false;
        TileAxis tileSize = TileAxis.One;
        if (LevelChunkEditor.Instance.m_ShowAllModel)
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
                    if (m_Ground) tileSize=m_Ground.GetDirectionedSize(m_Data.m_Direction);
                    break;
                case enum_TileSubType.Object:
                    showGround = true;
                    showObject = true;
                    if (m_Object) tileSize=m_Object.GetDirectionedSize(m_Data.m_Direction);
                    break;
                case enum_TileSubType.Pillar:
                    showPillar = true;
                    if (m_Pillar) tileSize=m_Pillar.GetDirectionedSize(m_Data.m_Direction);
                    break;
            }
        }
        if (m_Ground) m_Ground.SetActivate(showGround);
        if (m_Object) m_Object.SetActivate(showObject);
        if (m_Pillar) m_Pillar.SetActivate(showPillar);
        
        m_Collider.center = TileTools.GetLocalPosBySizeAxis(tileSize);
        Vector3 colliderSize = TileTools.GetUnitScaleBySizeAxis(tileSize, LevelConst.I_TileSize);
        if (LevelChunkEditor.Instance.m_EditMode == enum_TileSubType.Object&&m_Object!=null)
            colliderSize.y = LevelConst.I_TileSize*3;
        m_Collider.size = colliderSize;
    }

    public void RotateDirection(enum_TileDirection direction,System.Random random)
    {
        m_Data = m_Data.ChangeDirection(direction);
        Init(m_Axis, m_Data,random);
    }

}
