using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using TTiles;

public class LevelChunk : MonoBehaviour
{
    public enum_ChunkType m_ChunkType { get; private set; }
    public int m_Width { get; private set; }
    public int m_Height { get; private set; }
    ObjectPoolSimpleComponent<int, LevelTileNew> m_TilePool;
    void Init()
    {
        if (m_TilePool != null)
            return;
        m_TilePool = new ObjectPoolSimpleComponent<int, LevelTileNew>(transform.Find("TilePool"), "TileItem");
    }


    public virtual void Init(LevelChunkData _data,System.Random random)
    {
        Init();
        m_ChunkType = _data.Type;
        m_Width = _data.Width;
        m_Height = _data.Height;
        m_TilePool.ClearPool();
        ChunkTileData[] tileData= _data.Data;
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                TileAxis axis = new TileAxis(i, j);
                ChunkTileData data = tileData[TileTools.Get1DAxisIndex(axis, m_Width)];
                if (WillGenerateTile(ref data))
                    OnTileInit(m_TilePool.AddItem(i+j*m_Width), axis, data,random);
            }
    }
    protected virtual bool WillGenerateTile(ref ChunkTileData data)
    {
        if (data.m_ObjectType.IsEditorTileObject())
            data = data.ChangeObjectType(enum_TileObjectType.Invalid);

        if (data.m_GroundType != enum_TileGroundType.Invalid)
            return true;
        if (data.m_ObjectType != enum_TileObjectType.Invalid)
            return true;
        if (data.m_PillarType != enum_TilePillarType.Invalid)
            return true;
        return false;
    }
    protected virtual void OnTileInit(LevelTileNew tile,TileAxis axis,ChunkTileData data,System.Random random)
    {

        tile.Init(axis, data,random);
    }
}
