using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TTiles;
using UnityEngine;
namespace LevelSetting
{
    public static class LevelConst
    {
        public const int I_TileSize = 2;

        public static readonly Vector3 V3_TileUnitCenterOffset = new TileAxis(1, 1).ToPosition() / 2;

        public static readonly Color C_MapTexture_Ground =  new Color(0, 0, 0, 1);
        public static readonly Color C_MapTexture_Plants = new Color(1, 0, 0, 1);
    }

    public enum enum_LevelType {
        Invalid = -1,
        Start,
        Event,
        Battle,
        Final,
    }
    
    public enum enum_TileSubType
    {
        Invalid = -1,
        Object = 1,
        Terrain = 2,
        TerrainMap = 3,
        EdgeObject = 4,
        Plants = 5,

        EditorTerrain = 10,
    }

    public enum enum_EditorTerrainType
    {
        Invalid = -1,
        Plane,
        River,
        Highland,
    }
    
    public enum enum_TileTerrain 
    {
        Invalid=-1,
        Plane,

        River_0P,
        River_1RT,
        River_2RTB,
        River_2RTR_CP,
        River_2RRT_CR,
        River_3RLTR_LRR,
        River_3RLTR_LRP,
        River_3RLTR_LR_RP,
        River_3RLTR_LP_RR,
        River_4R_0P,
        River_4R_1PTR,
        River_4R_2PTRBL,
        River_4R_2PTRTL,
        River_4R_3P_1RTR,
        River_4R_4P,

        Highland,
        Slope_2STR_CS,
        Slope_3SLTR_LRS,
        Slope_4S_1GTR,
    }

    public enum enum_TileTerrainMap
    {
        Invalid=-1,
        Plants=1,
        Ground=2,
    }

    public enum enum_TileObject
    {
        Invalid=-1,
        Dangerzone = 1,
        Block=2,

        Static1x1A =11,
        Static1x1B=12,
        Static1x1C=13,
        Static1x1D=14,
        Static2x1A=15,
        Static2x1B=16,
        Static2x2A=17,
        Static2x2B=18,
        Static3x2A=19,
        Static3x2B=20,
        Static3x3A=21,
        Static3x3B=22,

        EditorStart =50,       //Available During Editor
        EEntrance2x2=51,
        EExport4x1=53,
        EEventArea3x3 = 55,
        EEnermySpawn1x1=56,
        EditorEnd,
    }

    public enum enum_TilePlantsType
    {
        Invalid=-1,
        Plants1=1,
        Plants2,
        Plants3,
        Plants4,
        Plants5,
        Plants6,
    }

    public enum enum_TileEdgeObject
    {
        Invalid=-1,
        EdgeObjectFenceL_BL = 1,
        EdgeObjectFenceL_BLBR = 2,
        EdgeObjectFenceBL_BLTR = 3,
    }
    public enum enum_ChunkEventType
    {
        Invalid=-1,
        Trader,
        Bonefire ,
        RewardChest,
        PerkUpgrade,
        PerkAcquire,
        WeaponReforge,
    }

    public enum enum_LevelEditorEditType
    {
        Invalid=-1,
        Terrain=1,
        TerrainMap=2,
        Object=3,
        EdgeObject=4,
    }

    public static class LevelExpressions
    {
        public static enum_LevelEditorEditType Next(this enum_LevelEditorEditType type)
        {
            type++;
            if (type > enum_LevelEditorEditType.EdgeObject)
                type = enum_LevelEditorEditType.Terrain;
            return type;
        }

        public static bool IsBattleLevel(this enum_LevelType type)
        {
            switch(type)
            {
                default:
                    return false;
                case enum_LevelType.Battle:
                case enum_LevelType.Final:
                    return true;
            }
        }
        public static Bounds GetWorldBounds(TileAxis origin, TileAxis size)
        {
            Vector3 boundsCenter = origin.ToPosition();
            Vector3 boundsSize = size.ToPosition();
            return new Bounds(boundsCenter + boundsSize / 2f + Vector3.up * LevelConst.I_TileSize, boundsSize+Vector3.up* LevelConst.I_TileSize);
        } 

        public static Vector3 ToPosition(this TileAxis axis) => new Vector3(axis.X,0,axis.Y)*LevelConst.I_TileSize;
        
        public static float GetTerrainHeight(this enum_TileTerrain type)
        {
            switch(type.GetEditorGroundType())
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return 0;
                case enum_EditorTerrainType.Highland:
                    return 2f;
                case enum_EditorTerrainType.Plane:
                    return 0;
                case enum_EditorTerrainType.River:
                    return -.5f;
            }
        }

        public static enum_EditorTerrainType GetEditorGroundType(this enum_TileTerrain type)
        {
            switch(type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return enum_EditorTerrainType.Invalid;
                case enum_TileTerrain.Invalid:
                    return enum_EditorTerrainType.Invalid;
                case enum_TileTerrain.Plane:
                    return enum_EditorTerrainType.Plane;
                case enum_TileTerrain.River_0P:
                case enum_TileTerrain.River_1RT:
                case enum_TileTerrain.River_2RTB:
                case enum_TileTerrain.River_2RTR_CP:
                case enum_TileTerrain.River_2RRT_CR:
                case enum_TileTerrain.River_3RLTR_LP_RR:
                case enum_TileTerrain.River_3RLTR_LRP:
                case enum_TileTerrain.River_3RLTR_LRR:
                case enum_TileTerrain.River_3RLTR_LR_RP:
                case enum_TileTerrain.River_4R_1PTR:
                case enum_TileTerrain.River_4R_2PTRBL:
                case enum_TileTerrain.River_4R_2PTRTL:
                case enum_TileTerrain.River_4R_3P_1RTR:
                case enum_TileTerrain.River_4R_4P:
                case enum_TileTerrain.River_4R_0P:
                    return enum_EditorTerrainType.River;
                case enum_TileTerrain.Highland:
                case enum_TileTerrain.Slope_2STR_CS:
                case enum_TileTerrain.Slope_3SLTR_LRS:
                case enum_TileTerrain.Slope_4S_1GTR:
                    return enum_EditorTerrainType.Highland;
            }
        }

        public static enum_TileTerrain GetDefaultTerrainType(this enum_EditorTerrainType type)
        {
            switch(type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return enum_TileTerrain.Invalid;
                case enum_EditorTerrainType.Plane:
                    return enum_TileTerrain.Plane;
                case enum_EditorTerrainType.River:
                    return enum_TileTerrain.River_0P;
                case enum_EditorTerrainType.Highland:
                    return enum_TileTerrain.Highland;
            }
        }


        public static bool IsEditorTileObject(this enum_TileObject type) => type >= enum_TileObject.EditorStart && type <= enum_TileObject.EditorEnd;

        public static TileAxis GetSizeAxis(this enum_TileObject type,enum_TileDirection direction)
        {
            TileAxis size = TileAxis.One;
            switch (type)
            {
                case enum_TileObject.Static2x2A:
                case enum_TileObject.Static2x2B:
                case enum_TileObject.EEntrance2x2:
                    size = TileAxis.One * 2;
                    break;
                case enum_TileObject.EExport4x1:
                    size = new TileAxis(4, 1);
                    break;
                case enum_TileObject.Static2x1B:
                case enum_TileObject.Static2x1A:
                    size = new TileAxis(2, 1);
                    break;
                case enum_TileObject.Static3x2A: 
                case enum_TileObject.Static3x2B:
                    size = new TileAxis(3, 2);
                    break;
                case enum_TileObject.EEventArea3x3:
                case enum_TileObject.Static3x3A:
                case enum_TileObject.Static3x3B:
                    size = TileAxis.One * 3;
                    break;
            }
            return TileTools.GetDirectionedSize(size, direction);
        }
        
        public static Dictionary<enum_TileObject,int> GetChunkRestriction(enum_LevelType type)
        {
            Dictionary<enum_TileObject, int> restrictionDic = new Dictionary<enum_TileObject, int>();
            switch(type)
            {
                case enum_LevelType.Start:
                    restrictionDic.Add(enum_TileObject.EEntrance2x2, 1);
                    restrictionDic.Add(enum_TileObject.EExport4x1, 1);
                    restrictionDic.Add(enum_TileObject.EEventArea3x3, 1);
                    break;
                case enum_LevelType.Event:
                    restrictionDic.Add(enum_TileObject.EEntrance2x2, 1);
                    restrictionDic.Add(enum_TileObject.EExport4x1, 1);
                    restrictionDic.Add(enum_TileObject.EEventArea3x3, 1);
                    break;
                case enum_LevelType.Battle:
                    restrictionDic.Add(enum_TileObject.EEntrance2x2, 1);
                    restrictionDic.Add(enum_TileObject.EExport4x1, 1);
                    restrictionDic.Add( enum_TileObject.EEnermySpawn1x1,-1);
                    break;
                case enum_LevelType.Final:
                    restrictionDic.Add(enum_TileObject.EEntrance2x2, 1);
                    restrictionDic.Add(enum_TileObject.EExport4x1, 1);
                    restrictionDic.Add(enum_TileObject.EEnermySpawn1x1, -1);
                    break;
            }
            return restrictionDic;
        }
    }
    
    public struct GameLevelData
    {
        public enum_LevelType m_ChunkType;
        public enum_ChunkEventType m_EventType;
        public GameLevelData(enum_LevelType _chunkType,enum_ChunkEventType _eventType= enum_ChunkEventType.Invalid)
        {
            m_ChunkType = _chunkType;
            m_EventType = _eventType;
        }
    }

    public struct ChunkGameObjectData
    {
        public Vector3 pos { get; private set; }
        public Quaternion rot { get; private set; }
        public ChunkGameObjectData(Vector3 _pos,Quaternion _rot)
        {
            pos = _pos;
            rot = _rot;
        }
    }

    public class ChunkGenerateData
    {
        public LevelChunkData m_Data { get; private set; }
        public enum_ChunkEventType m_EventType { get; private set; }
        public ChunkGenerateData( LevelChunkData _data, enum_ChunkEventType eventType)
        {
            m_Data = _data;
            m_EventType = eventType;
        }
    }
    
}