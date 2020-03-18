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
        Object=1,
        Terrain=2,
        EdgeObject=3,

        EditorGround=10,
    }

    public enum enum_EditorTerrainType
    {
        Invalid = -1,
        Plane,
        River,
        Highland,
    }
    
    public enum enum_TileTerrainType 
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

    public enum enum_TileObjectType
    {
        Invalid=-1,
        Dangerzone = 1,
        Block=2,

        Object1x1A =11,
        Object1x1B=12,
        Object1x1C=13,
        Object1x1D=14,
        Object2x1A=15,
        Object2x1B=16,
        Object2x2A=17,
        Object2x2B=18,
        Object3x2A=19,
        Object3x2B=20,
        Object3x3A=21,
        Object3x3B=22,
        
        EditorStart =50,       //Available During 
        REntrance2x2=51,
        RExport4x1=53,
        REventArea3x3 = 55,
        REnermySpawn1x1=56,
        EditorEnd,
    }

    public enum enum_TileEdgeObjectType
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
        Object=2,
        EdgeObject=3,
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
        
        public static float GetTerrainHeight(this enum_TileTerrainType type)
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

        public static enum_EditorTerrainType GetEditorGroundType(this enum_TileTerrainType type)
        {
            switch(type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return enum_EditorTerrainType.Invalid;
                case enum_TileTerrainType.Invalid:
                    return enum_EditorTerrainType.Invalid;
                case enum_TileTerrainType.Plane:
                    return enum_EditorTerrainType.Plane;
                case enum_TileTerrainType.River_0P:
                case enum_TileTerrainType.River_1RT:
                case enum_TileTerrainType.River_2RTB:
                case enum_TileTerrainType.River_2RTR_CP:
                case enum_TileTerrainType.River_2RRT_CR:
                case enum_TileTerrainType.River_3RLTR_LP_RR:
                case enum_TileTerrainType.River_3RLTR_LRP:
                case enum_TileTerrainType.River_3RLTR_LRR:
                case enum_TileTerrainType.River_3RLTR_LR_RP:
                case enum_TileTerrainType.River_4R_1PTR:
                case enum_TileTerrainType.River_4R_2PTRBL:
                case enum_TileTerrainType.River_4R_2PTRTL:
                case enum_TileTerrainType.River_4R_3P_1RTR:
                case enum_TileTerrainType.River_4R_4P:
                case enum_TileTerrainType.River_4R_0P:
                    return enum_EditorTerrainType.River;
                case enum_TileTerrainType.Highland:
                case enum_TileTerrainType.Slope_2STR_CS:
                case enum_TileTerrainType.Slope_3SLTR_LRS:
                case enum_TileTerrainType.Slope_4S_1GTR:
                    return enum_EditorTerrainType.Highland;
            }
        }

        public static enum_TileTerrainType GetDefaultTerrainType(this enum_EditorTerrainType type)
        {
            switch(type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return enum_TileTerrainType.Invalid;
                case enum_EditorTerrainType.Plane:
                    return enum_TileTerrainType.Plane;
                case enum_EditorTerrainType.River:
                    return enum_TileTerrainType.River_0P;
                case enum_EditorTerrainType.Highland:
                    return enum_TileTerrainType.Highland;
            }
        }


        public static bool IsEditorTileObject(this enum_TileObjectType type) => type >= enum_TileObjectType.EditorStart && type <= enum_TileObjectType.EditorEnd;

        public static TileAxis GetSizeAxis(this enum_TileObjectType type,enum_TileDirection direction)
        {
            TileAxis size = TileAxis.One;
            switch (type)
            {
                case enum_TileObjectType.Object2x2A:
                case enum_TileObjectType.Object2x2B:
                case enum_TileObjectType.REntrance2x2:
                    size = TileAxis.One * 2;
                    break;
                case enum_TileObjectType.RExport4x1:
                    size = new TileAxis(4, 1);
                    break;
                case enum_TileObjectType.Object2x1B:
                case enum_TileObjectType.Object2x1A:
                    size = new TileAxis(2, 1);
                    break;
                case enum_TileObjectType.Object3x2A: 
                case enum_TileObjectType.Object3x2B:
                    size = new TileAxis(3, 2);
                    break;
                case enum_TileObjectType.REventArea3x3:
                case enum_TileObjectType.Object3x3A:
                case enum_TileObjectType.Object3x3B:
                    size = TileAxis.One * 3;
                    break;
            }
            return TileTools.GetDirectionedSize(size, direction);
        }
        
        public static Dictionary<enum_TileObjectType,int> GetChunkRestriction(enum_LevelType type)
        {
            Dictionary<enum_TileObjectType, int> restrictionDic = new Dictionary<enum_TileObjectType, int>();
            switch(type)
            {
                case enum_LevelType.Start:
                    restrictionDic.Add(enum_TileObjectType.REntrance2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    restrictionDic.Add(enum_TileObjectType.REventArea3x3, 1);
                    break;
                case enum_LevelType.Event:
                    restrictionDic.Add(enum_TileObjectType.REntrance2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    restrictionDic.Add(enum_TileObjectType.REventArea3x3, 1);
                    break;
                case enum_LevelType.Battle:
                    restrictionDic.Add(enum_TileObjectType.REntrance2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    restrictionDic.Add( enum_TileObjectType.REnermySpawn1x1,-1);
                    break;
                case enum_LevelType.Final:
                    restrictionDic.Add(enum_TileObjectType.REntrance2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    restrictionDic.Add(enum_TileObjectType.REnermySpawn1x1, -1);
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