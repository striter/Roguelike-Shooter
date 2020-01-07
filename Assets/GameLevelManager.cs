using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        m_ChunkPool.ClearPool();
        LevelChunkData[] datas = TResources.GetLevelData();
        m_ChunkPool.AddItem(0).Init(datas[0]);
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