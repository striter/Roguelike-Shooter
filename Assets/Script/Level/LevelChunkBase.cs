using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using TTiles;
using System;

public class LevelChunkBase : MonoBehaviour
{
    protected ObjectPoolListComponent<int, LevelTileBase> m_TilePool { get; private set; }

    public virtual void Init()
    {
        m_TilePool = new ObjectPoolListComponent<int, LevelTileBase>(transform.Find("TilePool"), "TileItem");
    }
    protected void InitData(int width,int height,ChunkTileData[] tileData ,System.Random _random,Func<TileAxis,ChunkTileData,ChunkTileData> DataObjectCheck=null)
    {
        m_TilePool.Clear();
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                TileAxis axis = new TileAxis(i, j);
                int index = TileTools.Get1DAxisIndex(axis, width);
                ChunkTileData data = tileData[index];
                if (DataObjectCheck!=null)
                    data = DataObjectCheck(axis, data);
                if (!data.HasValue)
                    continue;
                OnTileInit(m_TilePool.AddItem(index), axis, data,_random);
            }
    }

    protected virtual void OnTileInit(LevelTileBase tile,TileAxis axis,ChunkTileData data,System.Random random)
    {
        tile.InitTile(axis, data,random);
    }
}
