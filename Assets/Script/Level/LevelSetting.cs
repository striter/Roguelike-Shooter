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
        #region UI
        public const int I_UIMinimapSize = 7;
        public const int I_UIPlayerViewClearRangeSecondPow = 100;       //10
        public const int I_UIPlayerViewFadeRangeSecondPow = 169;        //13

        public static readonly Color C_MapTextureFogColor = TCommon.GetHexColor("191919FF");
        public static readonly Color C_MapTextureFogFade = TCommon.GetHexColor("19191980");
        public static readonly Color C_MapTextureFogReveal = TCommon.GetHexColor("19191900");
        public static readonly Color C_MapTextureGroundColor = TCommon.GetHexColor("808080FF");
        public static readonly Color C_MapTextureGroundBlockColor = TCommon.GetHexColor("2FC02FFF");
        #endregion
    }

    public enum enum_ChunkType {
        Invalid = -1,
        Start,
        Event,
        Battle,
        Final,
        Connection,
    }
    
    public enum enum_TileSubType
    {
        Invalid = -1,
        Object,
        Ground,
        Pillar
    }

    public enum enum_TileGroundType
    {
        Invalid=-1,
        Main=1,
        Sub1=2,
        Sub2=3,
        Road1 = 4,
        Road2 = 5,
        Road3 = 6,
        Road4 = 7,
        Bridge =8,
        Dangerzone = 9,
        Block,
    }

    public enum enum_TileObjectType
    {
        Invalid=-1,
        Main=1,
        Sub1=2,
        Sub2=3,
        SubUnbreakable = 4,
        SubBreakable = 5,
        
        Object1x1A=11,
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
        RPlayerSpawn1x1=51,
        RConnection1x5 =52,
        RStagePortal2x2=53,
        RChunkPortal2x2=54,
        REventArea3x3 = 55,
        REnermySpawn1x1=56,
        REliteEnermySpawn1x1=57,
        RestrictEnd,
    }

    public enum enum_TilePillarType
    {
        Invalid=-1,
        Default=1,
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
    
    public static class LevelExpressions
    {
        public static Vector3 ToPosition(this TileAxis axis) => new Vector3(axis.X,0,axis.Y)*LevelConst.I_TileSize;

        public static bool TileObjectEditable(this enum_TileGroundType type)
        {
            switch(type)
            {
                case enum_TileGroundType.Main:
                case enum_TileGroundType.Sub1:
                case enum_TileGroundType.Sub2:
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
                case enum_TileObjectType.RConnection1x5:
                    size = new TileAxis(5, 1);
                    break;
                case enum_TileObjectType.Object2x2A:
                case enum_TileObjectType.Object2x2B:
                case enum_TileObjectType.RStagePortal2x2:
                case enum_TileObjectType.RChunkPortal2x2:
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

        public static int GetTexSelection(this enum_TileObjectType type,System.Random random)
        {
            int texSelection = -1;
            if (type == enum_TileObjectType.Main)
                texSelection = random.Next(0, 4);
            else if (type == enum_TileObjectType.Sub1)
                texSelection = random.Next(0, 2);
            return texSelection;
        }
        public static int GetTexSelection(this enum_TileGroundType type,System.Random random)
        {
            int texSelection = -1;
            switch (type)
            {
                case enum_TileGroundType.Main:
                    texSelection = random.Next(0, 4);
                    break;
                case enum_TileGroundType.Sub1:
                    texSelection = random.Next(0, 2);
                    break;
                case enum_TileGroundType.Road1:
                    texSelection = 0;
                    break;
                case enum_TileGroundType.Road2:
                    texSelection = 1;
                    break;
                case enum_TileGroundType.Road3:
                    texSelection = 2;
                    break;
                case enum_TileGroundType.Road4:
                    texSelection = 3;
                    break;
            }
            return texSelection;
        }

        public static Dictionary<enum_TileObjectType,int> GetChunkRestriction(enum_ChunkType type)
        {
            Dictionary<enum_TileObjectType, int> restrictionDic = new Dictionary<enum_TileObjectType, int>();
            switch(type)
            {
                case enum_ChunkType.Start:
                    restrictionDic.Add(enum_TileObjectType.RConnection1x5, 1);
                    restrictionDic.Add(enum_TileObjectType.RPlayerSpawn1x1, 1);
                    break;
                case enum_ChunkType.Event:
                    restrictionDic.Add(enum_TileObjectType.RChunkPortal2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.REventArea3x3, 1);
                    restrictionDic.Add(enum_TileObjectType.RConnection1x5, -1);
                    break;
                case enum_ChunkType.Battle:
                    restrictionDic.Add(enum_TileObjectType.RConnection1x5, -1);
                    restrictionDic.Add( enum_TileObjectType.REnermySpawn1x1,-1);
                    break;
                case enum_ChunkType.Final:
                    restrictionDic.Add(enum_TileObjectType.RConnection1x5, 1);
                    restrictionDic.Add(enum_TileObjectType.RStagePortal2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.REliteEnermySpawn1x1,1);
                    restrictionDic.Add(enum_TileObjectType.REnermySpawn1x1, -1);
                    break;
                case enum_ChunkType.Connection:
                    restrictionDic.Add(enum_TileObjectType.RConnection1x5, 2);
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
        public int m_ChunkIndex { get; private set; }
        public TileAxis m_Axis { get; private set; }
        public LevelChunkData m_Data { get; private set; }
        public enum_ChunkEventType m_EventType { get; private set; }
        public Dictionary<int, bool> m_ConnectPoint { get; private set; }
        public ChunkGenerateData(int chunkIndex,TileAxis _offset, LevelChunkData _data, enum_ChunkEventType eventType)
        {
            m_ChunkIndex = chunkIndex;
             m_Axis = _offset;
            m_Data = _data;
            m_EventType = eventType;
            m_ConnectPoint = new Dictionary<int, bool>();
            for (int i = 0; i < _data.Connections.Length; i++)
                m_ConnectPoint.Add(i, false);
        }

        public int m_PreChunkIndex { get; private set; }
        public int m_ChunkConnectPoint { get; private set; }
        public int m_PreChunkConnectPoint { get; private set; }
        public void SetPreConnectData(int connnectChunkIndex, int previousPointIndex, int currentPointIndex)
        {
            m_PreChunkIndex = connnectChunkIndex;
            m_PreChunkConnectPoint = previousPointIndex;
            m_ChunkConnectPoint = currentPointIndex;
        }

        public bool HaveEmptyConnection() => m_ConnectPoint.Values.Any(p => !p);
        public void OnConnectionSet(int connectionIndex) => m_ConnectPoint[connectionIndex] = true;
    }
}