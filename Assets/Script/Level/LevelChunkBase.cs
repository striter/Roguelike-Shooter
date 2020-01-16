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
    ObjectPoolSimpleComponent<int, LevelTileBase> m_TilePool;

    public virtual void Init()
    {
        m_TilePool = new ObjectPoolSimpleComponent<int, LevelTileBase>(transform.Find("TilePool"), "TileItem");
    }

    protected void InitData(LevelChunkData _data,System.Random _random,Func<TileAxis,ChunkTileData,ChunkTileData> DoTileDataCheck=null)
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
                if (DoTileDataCheck!=null)
                    data = DoTileDataCheck(axis, data);
                if (!WillGenerateTile(data))
                    continue;
                OnTileInit(m_TilePool.AddItem(index), axis, data,_random);
            }
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
    protected virtual void OnTileInit(LevelTileBase tile,TileAxis axis,ChunkTileData data,System.Random random)
    {
        tile.Init(axis, data,random);
    }


    public virtual ChunkGameData InitGameChunk(ChunkGenerateData data, System.Random random)
    {
        transform.localPosition = data.m_Axis.ToPosition();
        Dictionary<enum_TileObjectType, List<ChunkGameObjectData>> m_ChunkObjectPos = new Dictionary<enum_TileObjectType, List<ChunkGameObjectData>>();
        Dictionary<ChunkGameObjectData, enum_ChunkConnectionType> m_ChunkConnections = new Dictionary<ChunkGameObjectData, enum_ChunkConnectionType>();
        InitData(data.m_Data, random, (TileAxis axis, ChunkTileData tileData) => {
            if (tileData.m_ObjectType.IsEditorTileObject())
            {
                if (!m_ChunkObjectPos.ContainsKey(tileData.m_ObjectType))
                    m_ChunkObjectPos.Add(tileData.m_ObjectType, new List<ChunkGameObjectData>());
                m_ChunkObjectPos[tileData.m_ObjectType].Add(new ChunkGameObjectData(axis.ToPosition() + tileData.m_ObjectType.GetSizeAxis(tileData.m_Direction).ToPosition() / 2f, tileData.m_Direction.ToRotation()));
                return tileData.ChangeObjectType(enum_TileObjectType.Invalid);
            }
            return tileData;
        });

        //Generate Connections
        List<ChunkConnectionData> entranceGenerate = new List<ChunkConnectionData>();
        data.m_Connection.Traversal((int index, enum_ChunkConnectionType type) => {
            ChunkConnectionData connection = data.m_Data.Connections[index];
            m_ChunkConnections.Add(new ChunkGameObjectData(connection.m_Axis.ToPosition() + enum_TileObjectType.RConnection1x5.GetSizeAxis(connection.m_Direction).ToPosition()/2f, connection.m_Direction.ToRotation()), type);
            if (type == enum_ChunkConnectionType.Entrance)
                entranceGenerate.Add(connection);
        });

        entranceGenerate.Traversal((ChunkConnectionData connection)=>OnEntranceConnectionGenerate(connection,data.m_Data.Width,random));

        return new ChunkGameData(data.m_Data.Type, transform.localPosition, m_ChunkObjectPos, m_ChunkConnections);
    }


    protected virtual void OnEntranceConnectionGenerate(ChunkConnectionData connection,int width,System.Random random)
    {
        List<TileAxis> axies = TileTools.GetAxisRange(connection.m_Axis, enum_TileObjectType.RConnection1x5.GetSizeAxis(connection.m_Direction));
        axies.Traversal((TileAxis axis) => {
            ChunkTileData connectionTile = ChunkTileData.Create(enum_TilePillarType.Invalid, enum_TileGroundType.Main, enum_TileObjectType.Invalid, enum_TileDirection.Top);
            OnTileInit(m_TilePool.AddItem(TileTools.Get1DAxisIndex(axis, width)), axis, connectionTile, random);
        });
    }
}
