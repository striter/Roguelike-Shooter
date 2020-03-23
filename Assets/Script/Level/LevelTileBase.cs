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
    public List<TilePlantsBase> m_Plants { get; private set; } = new List<TilePlantsBase>();
    protected virtual float GetTerrainHeight(enum_TileTerrain terrain) => terrain == enum_TileTerrain.Invalid ?0 : terrain.GetTerrainHeight();
    public virtual void InitTile(TileAxis axis,ChunkTileData data,System.Random random)
    {
        m_Axis = axis;
        transform.localPosition = new Vector3(axis.X * LevelConst.I_TileSize, 0, axis.Y * LevelConst.I_TileSize);
        transform.localRotation = Quaternion.identity;
        tf_Models = transform.Find("Models");
        Vector3 objectHight = Vector3.up * GetTerrainHeight(data.m_TerrainType);
        if (data.m_TerrainType != enum_TileTerrain.Invalid)
        {
            m_Terrain = LevelObjectManager.GetTerrainItem(data.m_TerrainType, tf_Models);
            m_Terrain.OnGenerateItem(data, random);
            m_Terrain.transform.localPosition = Vector3.zero;
        }

        if (WillGenerateObject(data.m_ObjectType))
        {
            m_Object = LevelObjectManager.GetObjectItem(data.m_ObjectType, tf_Models);
            m_Object.OnGenerateItem(data, random);
            m_Object.transform.localPosition = objectHight; 
        }

        if(WillGeneratePlants(data))
        {
            int plantsCount = random.Next(1,4);
            for (int i = 0; i < plantsCount; i++)
            {
                TilePlantsBase plant = LevelObjectManager.GetPlantsItem( TCommon.RandomEnumValues<enum_TilePlantsType>(random),tf_Models);
                plant.OnGenerateItem(data,random);
                plant.transform.localPosition = objectHight+ new Vector3(TCommon.RandomUnitValue(random)*LevelConst.I_TileSize/2f,0, TCommon.RandomUnitValue(random) * LevelConst.I_TileSize / 2f);
                m_Plants.Add(plant);
            }
        }

        if(data.m_EdgeObjectType!= enum_TileEdgeObject.Invalid)
        {
            m_EdgeObject = LevelObjectManager.GetEdgeObjectItem(data.m_EdgeObjectType, tf_Models);
            m_EdgeObject.OnGenerateItem(data, random);
            m_EdgeObject.transform.localPosition = objectHight;
        }
    }

    public virtual void Clear()
    {
        if (m_Object)
            m_Object.DoRecycle();
        m_Object = null;
        if (m_Terrain)
            m_Terrain.DoRecycle();
        m_Terrain = null;
        if (m_EdgeObject)
            m_EdgeObject.DoRecycle();
        m_EdgeObject = null;

        m_Plants.Traversal((TilePlantsBase plant) => { plant.DoRecycle(); });
        m_Plants.Clear();
    }

    protected virtual bool WillGenerateObject(enum_TileObject type)
    {
        if (type.IsEditorTileObject()||type== enum_TileObject.Invalid)
            return false;
        return true;
    }
    protected virtual bool WillGeneratePlants(ChunkTileData data)
    {
        if (data.m_TerrainMap != enum_TileTerrainMap.Plants|| data.m_ObjectType != enum_TileObject.Invalid)
            return false;
        return true;
    }
}
