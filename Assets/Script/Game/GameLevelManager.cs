using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TTiles;
public class GameLevelManager : SimpleSingletonMono<GameLevelManager> {
    ObjectPoolSimpleComponent<int, LevelChunk> m_ChunkPool;
    public string m_Seed { get; private set; }
    public Texture2D m_MapTexture { get; private set; }
    TileAxis m_MapOrigin, m_MapSize;
    protected override void Awake()
    {
        base.Awake();
        m_ChunkPool = new ObjectPoolSimpleComponent<int, LevelChunk>(transform.Find("Level"),"ChunkItem");
        ObjectPoolManager.Init();       //Test
        LevelObjectManager.Register(enum_LevelStyle.Forest);
    }

    public void Generate(string seed)
    {
        m_Seed = seed == "" ? DateTime.Now.ToLongTimeString() : seed;
        System.Random random = new System.Random(m_Seed.GetHashCode());

        Dictionary<enum_ChunkType,List<LevelChunkData>> datas=TResources.GetChunkDatas();
        List<ChunkGenerateData> gameChunkGenerate = new List<ChunkGenerateData>();

        //Generate First Chunk
        gameChunkGenerate.Add(new ChunkGenerateData(TileAxis.Zero, datas[ enum_ChunkType.Start].RandomItem(random)));
        List<enum_ChunkType> mainChunkType = new List<enum_ChunkType>() { enum_ChunkType.Battle, enum_ChunkType.Event, enum_ChunkType.Battle, enum_ChunkType.Event, enum_ChunkType.Battle, enum_ChunkType.Battle, enum_ChunkType.Event };

        //Gemerate Main Chunks
        List<ChunkGenerateData> mainChunkGenerate = TryGenerateChunkDatas(gameChunkGenerate[0], gameChunkGenerate, datas, mainChunkType, random);
        gameChunkGenerate.AddRange(mainChunkGenerate);

        //Generate Final Chunk
        List<ChunkGenerateData> finalChunkGenerate = TryGenerateChunkDatas(gameChunkGenerate[gameChunkGenerate.Count - 1], gameChunkGenerate,datas,new List<enum_ChunkType>() { enum_ChunkType.Final },random);
        gameChunkGenerate.AddRange(finalChunkGenerate);
        
        //Generate Sub Chunks
        List<enum_ChunkType> subChunkType = new List<enum_ChunkType>() { enum_ChunkType.Battle, enum_ChunkType.Event };
        mainChunkGenerate.TraversalRandomBreak((ChunkGenerateData mainChunkData) =>
        {
            List<ChunkGenerateData> subGenerateData = null;
            if (mainChunkData.CheckEmptyConnections(random))
                subGenerateData = TryGenerateChunkDatas(mainChunkData, gameChunkGenerate, datas, subChunkType, random);
            if(subGenerateData!=null)
                gameChunkGenerate.AddRange(subGenerateData);
            return subGenerateData != null;
        },random);

        mainChunkGenerate.TraversalRandomBreak((ChunkGenerateData mainChunkData) =>
        {
            List<ChunkGenerateData> subGenerateData = null;
            if (mainChunkData.CheckEmptyConnections(random))
                subGenerateData = TryGenerateChunkDatas(mainChunkData, gameChunkGenerate, datas, subChunkType, random);
            if (subGenerateData != null)
                gameChunkGenerate.AddRange(subGenerateData);
            return subGenerateData != null;
        }, random);

        //Set Map Data(Origin,Size,Texture)
        int originX = 0, originY = 0, oppositeX = 0, oppositeY = 0;
        gameChunkGenerate.Traversal((ChunkGenerateData chunkData) =>        //Check MapOrigin/MapSize
        {
            TileAxis chunkOrigin = chunkData.m_Axis;
            TileAxis chunkOpposite = chunkOrigin + chunkData.m_Data.m_Size;
            if (oppositeX < chunkOpposite.X)
                oppositeX = chunkOpposite.X;
            if (oppositeY < chunkOpposite.Y)
                oppositeY = chunkOpposite.Y;
            if (originX > chunkOrigin.X)
                originX = chunkOrigin.X;
            if (originY > chunkOrigin.Y)
                originY = chunkOrigin.Y;
        });
        m_MapOrigin = new TileAxis(originX, originY);
        m_MapSize = new TileAxis(oppositeX-originX,oppositeY-originY);


        m_ChunkPool.ClearPool();
        int chunkIndex=0;
        gameChunkGenerate.Traversal((ChunkGenerateData data) => {
            LevelChunk chunk = m_ChunkPool.AddItem(chunkIndex++);
            chunk.Init(data.m_Data);
            chunk.transform.localPosition =data.m_Axis.ToWorldPosition();
        });

        m_MapTexture = new Texture2D(m_MapSize.X, m_MapSize.Y, TextureFormat.RGBA32, false);
        m_MapTexture.filterMode = FilterMode.Point;
        gameChunkGenerate.Traversal((ChunkGenerateData chunkdata) =>
        {
            Color[] chunkColors = chunkdata.m_Data.CalculateMapTextureColors();
            int length = chunkColors.Length;
            for(int index=0;index<length;index++)
            {
                TileAxis tileAxis = ( chunkdata.m_Axis- m_MapOrigin) +TileTools.GetAxisByIndex(index,chunkdata.m_Data.Width);
                m_MapTexture.SetPixel(tileAxis.X,tileAxis.Y,chunkColors[index]);
            }
        });
        m_MapTexture.Apply();
    }

    List<ChunkGenerateData> TryGenerateChunkDatas(ChunkGenerateData generateStartChunk,List<ChunkGenerateData> intersectsCheckChunks, Dictionary<enum_ChunkType, List<LevelChunkData>> datas,List<enum_ChunkType> generateTypes,System.Random random)
    {
        List<ChunkGenerateData> chunkIntersectsCheckData = new List<ChunkGenerateData>(intersectsCheckChunks);
        List<ChunkGenerateData> chunkGenerateData = new List<ChunkGenerateData>();
        ChunkGenerateData previousChunkGenerate = generateStartChunk;
        for (int i = 0; i < generateTypes.Count; i++)
        {
            ChunkGenerateData nextChunkGenerate = null;
            previousChunkGenerate.m_Data.Connections.TraversalRandomBreak((int previousConnectionIndex) =>
            {
                if (previousChunkGenerate.m_Connection[previousConnectionIndex])
                    return false;

                ChunkConnectionData m_previousConnectionData = previousChunkGenerate.m_Data.Connections[previousConnectionIndex];
                enum_TileDirection connectDirection = m_previousConnectionData.m_Direction.Inverse();
                datas[generateTypes[i]].TraversalRandomBreak((LevelChunkData nextChunkData) =>
                {
                    ChunkConnectionData? nextConnectionData = null;
                    nextChunkData.Connections.TraversalRandomBreak((int nextConnectionIndex) => {
                        ChunkConnectionData _connectionData = nextChunkData.Connections[nextConnectionIndex];
                        if (_connectionData.m_Direction == connectDirection)
                            nextConnectionData = _connectionData;

                        if (nextConnectionData.HasValue)
                        {
                            TileAxis nextChunkAxis = previousChunkGenerate.m_Axis + m_previousConnectionData.m_Axis - nextConnectionData.GetValueOrDefault().m_Axis;
                            bool _anyGeneratedChunkIntersects = false;
                            chunkIntersectsCheckData.TraversalBreak((ChunkGenerateData data) =>
                            {
                                _anyGeneratedChunkIntersects = CheckChunkIntersects(nextChunkAxis, nextChunkData.m_Size, data.m_Axis, data.m_Data.m_Size);
                                return _anyGeneratedChunkIntersects;
                            });
                            if (!_anyGeneratedChunkIntersects)
                            {
                                nextChunkGenerate = new ChunkGenerateData(nextChunkAxis, nextChunkData);
                                previousChunkGenerate.OnConnectionSet(previousConnectionIndex);
                                nextChunkGenerate.OnConnectionSet(nextConnectionIndex);
                            }
                        }
                        return nextConnectionData != null;
                    }, random);
                    return nextChunkGenerate != null;
                }, random);
                return nextChunkGenerate != null;
            }, random);

            if (nextChunkGenerate == null)
            {
                chunkGenerateData = null;
                break;
            }
            else
            {
                chunkGenerateData.Add(nextChunkGenerate);
                chunkIntersectsCheckData.Add(nextChunkGenerate);
                previousChunkGenerate = nextChunkGenerate;
            }
        }
        return chunkGenerateData;
    }

    bool CheckChunkIntersects(TileAxis s1origin,TileAxis s1size,TileAxis s2origin,TileAxis s2size)
    {
        //Edge Clip(All Edge Used For Connection)
        s1origin += TileAxis.One;
        s1size -= TileAxis.One*2;
        s2origin += TileAxis.One;
        s2size -= TileAxis.One*2;

        TileAxis[] square1Axises = new TileAxis[] { s1origin, s1origin + new TileAxis(s1size.X, 0), s1origin + new TileAxis(s1size.Y, 0),s1origin+s1size,s1origin+s1size/2 };
        TileAxis[] square2Axises = new TileAxis[] { s2origin, s2origin + new TileAxis(s2size.X, 0), s2origin + new TileAxis(s2size.Y, 0), s2origin + s2size,s2origin+s2size/2 };
        bool matched = false;
        square1Axises.TraversalRandomBreak((TileAxis s1Axis)=> {
            matched = TileTools.AxisInSquare(s1Axis, s2origin,s2size);
            return matched;
        });
        if (!matched)
        {
            square2Axises.TraversalRandomBreak((TileAxis s2Axis) => {
                matched = TileTools.AxisInSquare(s2Axis, s1origin, s1size);
                return matched;
            });
        }
        return matched;
    }

    class ChunkGenerateData
    {
        public LevelChunkData m_Data { get; private set; }
        public TileAxis m_Axis { get; private set; }
        public Dictionary<int, bool> m_Connection { get; private set; } = new Dictionary<int, bool>();
        public bool CheckEmptyConnections(System.Random random)
        {
            bool haveEmptyConnections = false;
            m_Connection.TraversalRandomBreak((int index, bool connected) =>
            {
                haveEmptyConnections = !connected;
                return haveEmptyConnections;
            }, random);
            return haveEmptyConnections;
        }
        public ChunkGenerateData(TileAxis _offset,LevelChunkData _data)
        {
            m_Axis = _offset;
            m_Data = _data;
            for (int i = 0; i < _data.Connections.Length; i++)
                m_Connection.Add(i,false);
        }

        public void OnConnectionSet(int connectionIndex) => m_Connection[connectionIndex]=true;
    }
}


public static class LevelObjectManager
{
    public static bool m_Registed = false;
    public static void Register(enum_LevelStyle style)
    {
        m_Registed = true;
        TResources.GetLevelItemsNew(style).Traversal((enum_TileSubType type, List<LevelTileItemBase> items) => {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!"); break;
                case enum_TileSubType.Ground:
                    items.Traversal((LevelTileItemBase item) =>
                    {
                        TileGroundBase groundItem = item as TileGroundBase;
                        ObjectPoolManager<enum_TileGroundType, TileGroundBase>.Register(groundItem.m_GroundType, groundItem, 1);
                    });
                    break;
                case enum_TileSubType.Object:
                    items.Traversal((LevelTileItemBase item) =>
                    {
                        TileObjectBase objectItem = item as TileObjectBase;
                        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Register(objectItem.m_ObjectType, objectItem, 1);
                    });
                    break;
                case enum_TileSubType.Pillar:
                    items.Traversal((LevelTileItemBase item) =>
                    {
                        TilePillarBase pillarItem = item as TilePillarBase;
                        ObjectPoolManager<enum_TilePillarType, TilePillarBase>.Register(pillarItem.m_PillarType, pillarItem, 1);
                    });
                    break;
            }
        });
    }

    public static void Clear()
    {
        m_Registed = false;
        ObjectPoolManager<enum_TileGroundType, TileGroundBase>.DestroyAll();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.DestroyAll();
        ObjectPoolManager<enum_TilePillarType, TilePillarBase>.DestroyAll();
    }

    public static TilePillarBase GetPillar(enum_TilePillarType type, Transform trans) => ObjectPoolManager<enum_TilePillarType, TilePillarBase>.Spawn(type, trans);
    public static TileObjectBase GetObject(enum_TileObjectType type, Transform trans) => ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Spawn(type, trans);
    public static TileGroundBase GetGround(enum_TileGroundType type, Transform trans) => ObjectPoolManager<enum_TileGroundType, TileGroundBase>.Spawn(type, trans);
}