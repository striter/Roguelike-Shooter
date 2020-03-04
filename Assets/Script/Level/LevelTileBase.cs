using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTiles;
using LevelSetting;
using System;

public class LevelTileBase : MonoBehaviour, ITileAxis
{
    public TileAxis m_Axis { get; private set; }
    public ChunkTileData m_Data { get; protected set; }
    protected Transform tf_Models { get; private set; }
    public TileGroundBase m_Ground { get; protected set; }
    public TileObjectBase m_Object { get; protected set; }
    public virtual void Init(TileAxis axis,ChunkTileData data,System.Random random)
    {
        m_Axis = axis;
        m_Data = data;
        transform.localPosition = new Vector3(axis.X * LevelConst.I_TileSize, 0, axis.Y * LevelConst.I_TileSize);
        transform.localRotation = Quaternion.identity;
        tf_Models = transform.Find("Models");
        Clear();
        if (WillGenerateObject(m_Data.m_ObjectType))
        {
            m_Object = LevelObjectManager.GetObjectItem(m_Data.m_ObjectType, tf_Models);
            m_Object.OnGenerateItem(m_Data, random);
            m_Object.transform.localPosition = Vector3.zero;
        }

        if (m_Data.m_GroundType != enum_TileGroundType.Invalid)
        {
            m_Ground = LevelObjectManager.GetGroundItem(m_Data.m_GroundType, tf_Models);
            m_Ground.OnGenerateItem(m_Data, random);
            m_Ground.transform.localPosition = Vector3.zero;
        }
    }
    public void Clear()
    {
        if (m_Object)
            m_Object.DoRecycle();
        if (m_Ground)
            m_Ground.DoRecycle();
        m_Object = null;
        m_Ground = null;
    }
    protected virtual bool WillGenerateObject(enum_TileObjectType type) => type != enum_TileObjectType.Invalid && !type.IsEditorTileObject();
}
