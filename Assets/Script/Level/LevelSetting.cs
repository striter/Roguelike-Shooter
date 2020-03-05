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

        EditorGround=10,
    }

    public enum enum_EditorGroundType
    {
        Invalid = -1,
        Main,
        Water,
    }
    
    public enum enum_TileTerrainType 
    {
        Invalid=-1,
        Ground,

        River_0W,
        River_1WT,
        River_2WTB,
        River_2WTR_CG,
        River_2WRT_CW,
        River_3WLTR_LRW,
        River_3WLTR_LRG,
        River_3WLTR_LW_RG,
        River_3WLTR_LG_RW,
        River_4W_0G,
        River_4W_1GTR,
        River_4W_2GTRBL,
        River_4W_2GTRTL,
        River_4W_3G_1WTR,
        River_4W_4G,
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
        
        EditorStart=50,       //Available During 
        REntrance2x2=51,
        RExport4x1=53,
        REventArea3x3 = 55,
        REnermySpawn1x1=56,
        EditorEnd,
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

    public enum enum_ChunkRevealType
    {
        Invalid=-1,
        PreFog=1,
        PreFaded=2,
        PreRevealed = 3,
        PrepareEnd=10,

        Empty = 11,
        Revealed = 12,
    }
    
    public static class LevelExpressions
    {
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

        public static bool EditorObjectEditable(this enum_EditorGroundType type)
        {
            switch(type)
            {
                default:return false;
                case enum_EditorGroundType.Main:
                    return true;
            }
        }
        public static enum_EditorGroundType GetEditorGroundType(this enum_TileTerrainType type)
        {
            switch(type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return enum_EditorGroundType.Invalid;
                case enum_TileTerrainType.Invalid:
                    return enum_EditorGroundType.Invalid;
                case enum_TileTerrainType.Ground:
                    return enum_EditorGroundType.Main;
                case enum_TileTerrainType.River_0W:
                case enum_TileTerrainType.River_1WT:
                case enum_TileTerrainType.River_2WTB:
                case enum_TileTerrainType.River_2WTR_CG:
                case enum_TileTerrainType.River_2WRT_CW:
                case enum_TileTerrainType.River_3WLTR_LG_RW:
                case enum_TileTerrainType.River_3WLTR_LRG:
                case enum_TileTerrainType.River_3WLTR_LRW:
                case enum_TileTerrainType.River_3WLTR_LW_RG:
                case enum_TileTerrainType.River_4W_1GTR:
                case enum_TileTerrainType.River_4W_2GTRBL:
                case enum_TileTerrainType.River_4W_2GTRTL:
                case enum_TileTerrainType.River_4W_3G_1WTR:
                case enum_TileTerrainType.River_4W_4G:
                case enum_TileTerrainType.River_4W_0G:
                    return enum_EditorGroundType.Water;
            }
        }

        public static enum_TileTerrainType GetEditorTerrainType(this enum_EditorGroundType type)
        {
            switch(type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return enum_TileTerrainType.Invalid;
                case enum_EditorGroundType.Main:
                    return enum_TileTerrainType.Ground;
                    case enum_EditorGroundType.Water:
                    return enum_TileTerrainType.River_0W;
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
        public TileAxis m_Origin { get; private set; }
        public TileBounds m_MapBounds { get; private set; }
        public LevelChunkData m_Data { get; private set; }
        public enum_ChunkEventType m_EventType { get; private set; }
        public ChunkGenerateData( LevelChunkData _data, enum_ChunkEventType eventType)
        {
            m_Data = _data;
            m_EventType = eventType;
            m_MapBounds = new TileBounds(m_Origin,m_Data.m_Size);
        }
    }
    
}