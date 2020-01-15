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


    public void InitChunk(LevelChunkData _data,System.Random _random)
    {
        Init();
        m_ChunkType = _data.Type;
        m_Width = _data.Width;
        m_Height = _data.Height;
        m_TilePool.ClearPool();

        ChunkTileData[] tileData= _data.GetData();
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                TileAxis axis = new TileAxis(i, j);
                int index = TileTools.Get1DAxisIndex(axis, m_Width);
                ChunkTileData data = tileData[index];
                if (WillGenerateTile(ref data))
                    OnTileInit(m_TilePool.AddItem(index), axis, data,_random);
            }
    }

    public void InitGameData(ChunkGenerateData data,System.Random random)
    {
        List<ChunkConnectionData> entranceGenerate = new List<ChunkConnectionData>();
        data.m_Connection.Traversal((int index, enum_ChunkConnectionType type) => {
            if (type == enum_ChunkConnectionType.Entrance)
                entranceGenerate.Add(data.m_Data.Connections[index]);
        });

        entranceGenerate.Traversal((ChunkConnectionData connection)=> {
            List<TileAxis> axies = TileTools.GetAxisRange(connection.m_Axis,enum_TileObjectType.Connection1x5.GetSizeAxis(connection.m_Direction));
            axies.Traversal((TileAxis axis) => {
                ChunkTileData connectionTile = ChunkTileData.Create(enum_TilePillarType.Invalid, enum_TileGroundType.Main, enum_TileObjectType.Invalid, enum_TileDirection.Top);
                OnTileInit(m_TilePool.AddItem(TileTools.Get1DAxisIndex(axis, data.m_Data.Width)),axis,connectionTile,random);
            });
        });
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
