using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using TTiles;
using System;

public class LevelChunk : MonoBehaviour
{
    public enum_ChunkType m_ChunkType { get; private set; }
    public int m_Width { get; private set; }
    public int m_Height { get; private set; }
    ObjectPoolSimpleComponent<int, LevelTileNew> m_TilePool;
    protected void InitData(LevelChunkData _data,System.Random _random,Func<TileAxis,ChunkTileData,ChunkTileData> DoTileDataCheck=null)
    {
        if (m_TilePool == null)
            m_TilePool = new ObjectPoolSimpleComponent<int, LevelTileNew>(transform.Find("TilePool"), "TileItem");
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
                if (DoTileDataCheck!=null)
                    data = DoTileDataCheck(axis, data);
                if (!WillGenerateTile(data))
                    continue;
                OnTileInit(m_TilePool.AddItem(index), axis, data,_random);
            }
    }

    public ChunkGameData InitGameChunk(ChunkGenerateData data,System.Random random)
    {
        Dictionary<enum_TileObjectType, List<ChunkTileGameData>> m_ChunkObjectPos=new Dictionary<enum_TileObjectType, List<ChunkTileGameData>>();
        InitData(data.m_Data,random,(TileAxis axis, ChunkTileData tileData)=> {
            if (tileData.m_ObjectType.IsEditorTileObject())
            {
                if (!m_ChunkObjectPos.ContainsKey(tileData.m_ObjectType))
                    m_ChunkObjectPos.Add(tileData.m_ObjectType, new List<ChunkTileGameData>());
                m_ChunkObjectPos[tileData.m_ObjectType].Add(new ChunkTileGameData( axis.ToWorldPosition() + tileData.m_ObjectType.GetSizeAxis(tileData.m_Direction).ToWorldPosition() / 2f,tileData.m_Direction.GetWorldRotation()));
                return tileData.ChangeObjectType(enum_TileObjectType.Invalid);
            }
            return tileData;
        });
        
        //Generate Connections
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

        return new ChunkGameData(data.m_Data.Type,m_ChunkObjectPos);
    }

    protected virtual bool WillGenerateTile( ChunkTileData tileData)
    {
        if (tileData.m_GroundType != enum_TileGroundType.Invalid)
            return true;
        if (tileData.m_ObjectType != enum_TileObjectType.Invalid)
            return true;
        if (tileData.m_PillarType != enum_TilePillarType.Invalid)
            return true;
        return false;
    }
    protected virtual void OnTileInit(LevelTileNew tile,TileAxis axis,ChunkTileData data,System.Random random)
    {
        tile.Init(axis, data,random);
    }
}
