using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTiles;
using LevelSetting;
using System;

public class LevelTileBase : MonoBehaviour, ITileAxis
{
    public TileAxis m_Axis { get; private set; }
    protected Transform tf_Models { get; private set; }
    public TileTerrainBase m_Terrain { get; protected set; }
    public TileObjectBase m_Object { get; protected set; }
    public TileEdgeObjectBase m_EdgeObject { get; private set; }
    public virtual void InitTile(TileAxis axis,ChunkTileData data,System.Random random)
    {
        Clear();
        m_Axis = axis;
        transform.localPosition = new Vector3(axis.X * LevelConst.I_TileSize, 0, axis.Y * LevelConst.I_TileSize);
        transform.localRotation = Quaternion.identity;
        tf_Models = transform.Find("Models");
        if (WillGenerateObject(data.m_ObjectType))
        {
            m_Object = LevelObjectManager.GetObjectItem(data.m_ObjectType, tf_Models);
            m_Object.OnGenerateItem(data, random);
            m_Object.transform.localPosition = Vector3.zero;
        }

        if (data.m_TerrainType != enum_TileTerrainType.Invalid)
        {
            m_Terrain = LevelObjectManager.GetTerrainItem(data.m_TerrainType, tf_Models);
            m_Terrain.OnGenerateItem(data, random);
            m_Terrain.transform.localPosition = Vector3.zero;
        }

        if(data.m_EdgeObjectType!= enum_TileEdgeObjectType.Invalid)
        {
            m_EdgeObject = LevelObjectManager.GetEdgeObjectItem(data.m_EdgeObjectType, tf_Models);
            m_EdgeObject.OnGenerateItem(data, random);
            m_EdgeObject.transform.localPosition = Vector3.zero;
        }
    }
    public virtual void Clear()
    {
        if (m_Object)
            m_Object.DoRecycle();
        if (m_Terrain)
            m_Terrain.DoRecycle();
        if (m_EdgeObject)
            m_EdgeObject.DoRecycle();
        m_Object = null;
        m_Terrain = null;
        m_EdgeObject = null;
    }
    protected virtual bool WillGenerateObject(enum_TileObjectType type) => type != enum_TileObjectType.Invalid && !type.IsEditorTileObject();
}
