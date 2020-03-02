using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TTiles;
using UnityEngine;
namespace LevelSetting
{
    public static class LevelConst
    {
        #region UI
        public const int I_UIMinimapSize = 8;
        public const int I_UIMapScale = 8;
        public const float F_UIMapIconBaseScale = 25;       //final scale= BaseScale/MapScale
        public const float F_UIMapDragSpeedMultiply = 2f;
        public const int I_UIMapPullbackCheckRange = 30;
        public const int I_UIMapPullbackSpeedMultiply = 8;

        public const int I_UIPlayerViewRevealRange = 10;
        public const int I_UIPlayerViewFadeRange = 12;
        public static readonly int I_UIPlayerViewRevealSqrRange = I_UIPlayerViewRevealRange * I_UIPlayerViewRevealRange;
        public static readonly int I_UIPlayerViewFadeSqrRange = I_UIPlayerViewFadeRange* I_UIPlayerViewFadeRange; 

        public static readonly Color C_MapFogRevealFogColor = TCommon.GetHexColor("191919FF");
        public static readonly Color C_MapFogRevealFadeColor= TCommon.GetHexColor("191919D0");
        public static readonly Color C_MapFogRevealClearColor = TCommon.GetHexColor("19191900");
        public static readonly Color C_MapTextureGroundColor = TCommon.GetHexColor("808080FF");
        #endregion
        public const int I_TileSize = 2;
        public const int I_QuadranteTileSize = 15;

        public static readonly Vector3 V3_TileUnitCenterOffset = new TileAxis(1, 1).ToPosition() / 2;
    }

    public enum enum_ChunkType {
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
        Ground=2,
    }

    public enum enum_TileGroundType
    {
        Invalid=-1,
        Main,
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
        
        RestrictStart=50,
        REntrance2x2=51,
        RExport4x1=53,
        REventArea3x3 = 55,
        REnermySpawn1x1=56,
        RestrictEnd,
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
        public static Bounds GetWorldBounds(TileAxis origin, TileAxis size)
        {
            Vector3 boundsCenter = origin.ToPosition();
            Vector3 boundsSize = size.ToPosition();
            return new Bounds(boundsCenter + boundsSize / 2f + Vector3.up * LevelConst.I_TileSize, boundsSize+Vector3.up* LevelConst.I_TileSize);
        } 
        public static Vector3 ToPosition(this TileAxis axis) => new Vector3(axis.X,0,axis.Y)*LevelConst.I_TileSize;

        public static bool TileObjectEditable(this enum_TileGroundType type)
        {
            switch(type)
            {
                case enum_TileGroundType.Main:
                    return true;
            }
            return false;
        }

        public static bool IsEditorTileObject(this enum_TileObjectType type) => type >= enum_TileObjectType.RestrictStart && type <= enum_TileObjectType.RestrictEnd;

        public static TileAxis GetSizeAxis(this enum_TileObjectType type,enum_TileDirection direction)
        {
            TileAxis size = TileAxis.One;
            switch (type)
            {
                case enum_TileObjectType.Object2x2A:
                case enum_TileObjectType.Object2x2B:
                case enum_TileObjectType.RExport4x1:
                    size = TileAxis.One * 2;
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
        
        public static Dictionary<enum_TileObjectType,int> GetChunkRestriction(enum_ChunkType type)
        {
            Dictionary<enum_TileObjectType, int> restrictionDic = new Dictionary<enum_TileObjectType, int>();
            switch(type)
            {
                case enum_ChunkType.Start:
                    restrictionDic.Add(enum_TileObjectType.REntrance2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    break;
                case enum_ChunkType.Event:
                    restrictionDic.Add(enum_TileObjectType.REntrance2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    restrictionDic.Add(enum_TileObjectType.REventArea3x3, 1);
                    break;
                case enum_ChunkType.Battle:
                    restrictionDic.Add(enum_TileObjectType.REntrance2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    restrictionDic.Add( enum_TileObjectType.REnermySpawn1x1,-1);
                    break;
                case enum_ChunkType.Final:
                    restrictionDic.Add(enum_TileObjectType.REntrance2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    restrictionDic.Add(enum_TileObjectType.RExport4x1, 1);
                    restrictionDic.Add(enum_TileObjectType.REnermySpawn1x1, -1);
                    break;
            }
            return restrictionDic;
        }
    }
    
    public struct ChunkPreGenerateData
    {
        public enum_ChunkType m_ChunkType;
        public enum_ChunkEventType m_EventType;
        public ChunkPreGenerateData(enum_ChunkType _chunkType,enum_ChunkEventType _eventType= enum_ChunkEventType.Invalid)
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
        public TileBounds m_GenerateCheckBounds { get; private set; } 
        public LevelChunkData m_Data { get; private set; }
        public enum_ChunkEventType m_EventType { get; private set; }
        public ChunkGenerateData( LevelChunkData _data, enum_ChunkEventType eventType)
        {
            m_Data = _data;
            m_EventType = eventType;
            m_MapBounds = new TileBounds(m_Origin,m_Data.m_Size);
            m_GenerateCheckBounds = new TileBounds(m_Origin+TileAxis.One, m_Data.m_Size-TileAxis.One*2);
        }
    }
    
}