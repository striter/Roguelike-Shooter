using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using TTiles;
using System;

public class LevelChunkBase : MonoBehaviour
{
    public enum_ChunkType m_ChunkType { get; private set; }
    public int m_Width { get; private set; }
    public int m_Height { get; private set; }
    protected ObjectPoolListComponent<int, LevelTileBase> m_TilePool { get; private set; }

    public virtual void Init()
    {
        m_TilePool = new ObjectPoolListComponent<int, LevelTileBase>(transform.Find("TilePool"), "TileItem");
    }
    protected void InitData(LevelChunkData _data,System.Random _random,Func<TileAxis,ChunkTileData,ChunkTileData> DataObjectCheck=null)
    {
        m_TilePool.ClearPool();
        m_ChunkType = _data.Type;
        m_Width = _data.Width;
        m_Height = _data.Height;

        ChunkTileData[] tileData= _data.GetData();
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                TileAxis axis = new TileAxis(i, j);
                int index = TileTools.Get1DAxisIndex(axis, m_Width);
                ChunkTileData data = tileData[index];
                if (DataObjectCheck!=null)
                    data = DataObjectCheck(axis, data);
                if (!WillGenerateTile(data))
                    continue;
                OnTileInit(m_TilePool.AddItem(index), axis, data,_random);
            }
    }

    protected virtual bool WillGenerateTile( ChunkTileData tileData)
    {
        if (tileData.m_TerrainType != enum_TileTerrainType.Invalid)
            return true;
        if (tileData.m_ObjectType != enum_TileObjectType.Invalid)
            return true;
        return false;
    }
    protected virtual void OnTileInit(LevelTileBase tile,TileAxis axis,ChunkTileData data,System.Random random)
    {
        tile.InitTile(axis, data,random);
    }
}
