using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTiles;
public class GameLevelManager : SimpleSingletonMono<GameLevelManager> {
    ObjectPoolSimpleComponent<int, LevelChunk> m_ChunkPool;
    protected override void Awake()
    {
        base.Awake();
        m_ChunkPool = new ObjectPoolSimpleComponent<int, LevelChunk>(transform.Find("Level"),"ChunkItem");
        ObjectPoolManager.Init();       //Test
        LevelObjectManager.Register(enum_LevelStyle.Forest);
    }

    public void Generate()
    {
        LevelChunkData[] datas = TResources.GetLevelData();
        List<ChunkGenerateData> generateData = new List<ChunkGenerateData>();

        generateData.Add(new ChunkGenerateData(TileAxis.Zero, datas[0]));
        for(int i=0;i<2;i++)
        {
            ChunkGenerateData previousChunk = generateData[i];
            Dictionary<TileAxis, enum_TileDirection> connectionDirection = new Dictionary<TileAxis, enum_TileDirection>();
            previousChunk.m_Connection.Traversal((int index) =>
            {
            });



            LevelChunkData currentData = datas.RandomItem();

            TileAxis offsetAxis = new TileAxis(previousChunk.m_Data.Width, 0);

            generateData.Add(new ChunkGenerateData(previousChunk.m_Axis+offsetAxis, currentData));
        }


        m_ChunkPool.ClearPool();
        int chunkIndex=0;
        generateData.Traversal((ChunkGenerateData data) => {
            LevelChunk chunk = m_ChunkPool.AddItem(chunkIndex++);
            chunk.Init(data.m_Data);
            chunk.transform.localPosition = new Vector3(data.m_Axis.X,0, data.m_Axis.Y)*LevelConst.I_TileSize;
        });
    }

    class ChunkGenerateData
    {
        public TileAxis m_Axis { get; private set; }
        public LevelChunkData m_Data { get; private set; }
        public Dictionary<int, ChunkGenerateConnectionData> m_Connection { get; private set; } = new Dictionary<int, ChunkGenerateConnectionData>();

        public ChunkGenerateData(TileAxis _offset,LevelChunkData _data)
        {
            m_Axis = _offset;
            m_Data = _data;
            for (int i = 0; i < _data.Connections.Length; i++)
                m_Connection.Add(_data.Connections[i],new ChunkGenerateConnectionData(_data.Connections[i],_data.Width,_data.Height));
        }

        public void OnConnectionSet(int connectionIndex) => m_Connection[connectionIndex].SetConnecting();

        public class ChunkGenerateConnectionData
        {
            public TileAxis axis { get; private set; }
            public  enum_TileDirection direction { get; private set; }
            public bool connecting { get; private set; }
            public ChunkGenerateConnectionData(int connectionIndex,int width,int height)
            {
                axis = TileTools.GetAxisByIndex(connectionIndex, width);
                direction = enum_TileDirection.Invalid;
                if (axis.Y == 0)
                    direction = enum_TileDirection.Bottom;
                else if (axis.Y == height - 1)
                    direction = enum_TileDirection.Top;
                else if (axis.X == 0)
                    direction = enum_TileDirection.Left;
                else if (axis.X == width - 1)
                    direction = enum_TileDirection.Right;
                connecting = false;
            }
            public void SetConnecting() => connecting = true;
        }
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