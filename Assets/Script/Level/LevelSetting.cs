using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
namespace LevelSetting
{
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
        REliteTrigger1x1=58,
        RestrictEnd,
    }

    public enum enum_TilePillarType
    {
        Invalid=-1,
        Default=1,
    }

    public enum enum_ChunkConnectionType
    {
        Invalid=-1,
        Empty=0,
        Export=1,
        Entrance=2,
    }
    
    public static class LevelConst
    {
        public const int I_TileSize = 2;
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
                    restrictionDic.Add(enum_TileObjectType.REliteTrigger1x1, -1);
                    restrictionDic.Add(enum_TileObjectType.REnermySpawn1x1, -1);
                    break;
            }
            return restrictionDic;
        }
    }

    public struct ChunkGameData
    {
        public Vector3 m_Origin { get; private set; }
        public Vector3 GetObjectWorldPosition(Vector3 localPosition) => m_Origin + localPosition+Vector3.up*LevelConst.I_TileSize;
        public enum_ChunkType m_ChunkType { get; private set; }
        public Dictionary<enum_TileObjectType,  List<ChunkGameObjectData>> m_LocalChunkObjects { get; private set; }
        public Dictionary<ChunkGameObjectData,enum_ChunkConnectionType> m_LocalChunkConnections { get; private set; }
        public ChunkGameData(enum_ChunkType _chunkType,Vector3 _origin, Dictionary<enum_TileObjectType, List<ChunkGameObjectData>> _chunkObjects, Dictionary<ChunkGameObjectData, enum_ChunkConnectionType> _connections)
        {
            m_Origin = _origin;
            m_ChunkType = _chunkType;
            m_LocalChunkObjects = _chunkObjects;
            m_LocalChunkConnections = _connections;
        }
    }

    public class ChunkGameObjectData
    {
        public Vector3 m_LocalPosition { get; private set; }
        public Quaternion Rotation { get; private set; }
        public ChunkGameObjectData(Vector3 positon,Quaternion rotation)
        {
            m_LocalPosition = positon;
            Rotation = rotation;
        }
    }

    public class ChunkGenerateData
    {
        public TileAxis m_Axis { get; private set; }
        public LevelChunkData m_Data { get; private set; }
        public Dictionary<int, enum_ChunkConnectionType> m_Connection { get; private set; }
        public ChunkGenerateData(TileAxis _offset, LevelChunkData _data)
        {
            m_Axis = _offset;
            m_Data = _data;
            m_Connection = new Dictionary<int, enum_ChunkConnectionType>();
            for (int i = 0; i < _data.Connections.Length; i++)
                m_Connection.Add(i, enum_ChunkConnectionType.Empty);
        }

        public bool CheckEmptyConnections(System.Random random)
        {
            bool haveEmptyConnections = false;
            m_Connection.TraversalRandomBreak((int index, enum_ChunkConnectionType connectType) =>
            {
                haveEmptyConnections = connectType == enum_ChunkConnectionType.Empty;
                return haveEmptyConnections;
            }, random);
            return haveEmptyConnections;
        }

        public void OnConnectionSet(int connectionIndex, enum_ChunkConnectionType type) => m_Connection[connectionIndex] = type;
    }
}