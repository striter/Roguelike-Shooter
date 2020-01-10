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
        PlayerSpawn1x1=51,
        Connection1x5 =52,
        StagePortal2x2=53,
        ChunkPortal2x2=54,
        EventArea3x3 = 55,
        EnermySpawn1x1=56,
        EliteEnermySpawn1x1=57,
        EliteTrigger1x1=58,
        RestrictEnd,
    }

    public enum enum_TilePillarType
    {
        Invalid=-1,
        Default=1,
    }
    
    public static class LevelConst
    {
        public const int I_TileSize = 2;
    }

    public static class LevelExpressions
    {
        public static Vector3 ToWorldPosition(this TileAxis axis) => new Vector3(axis.X,0,axis.Y)*LevelConst.I_TileSize;

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

        public static TileAxis GetSizeAxis(this enum_TileObjectType type,enum_TileDirection direction)
        {
            TileAxis size = TileAxis.One;
            switch (type)
            {
                case enum_TileObjectType.Connection1x5:
                    size = new TileAxis(5, 1);
                    break;
                case enum_TileObjectType.Object2x2A:
                case enum_TileObjectType.Object2x2B:
                case enum_TileObjectType.StagePortal2x2:
                case enum_TileObjectType.ChunkPortal2x2:
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
                case enum_TileObjectType.EventArea3x3:
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
                    restrictionDic.Add(enum_TileObjectType.Connection1x5, 1);
                    restrictionDic.Add(enum_TileObjectType.PlayerSpawn1x1, 1);
                    break;
                case enum_ChunkType.Event:
                    restrictionDic.Add(enum_TileObjectType.ChunkPortal2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.EventArea3x3, 1);
                    restrictionDic.Add(enum_TileObjectType.Connection1x5, 3);
                    break;
                case enum_ChunkType.Battle:
                    restrictionDic.Add(enum_TileObjectType.Connection1x5, 3);
                    restrictionDic.Add( enum_TileObjectType.EnermySpawn1x1,-1);
                    break;
                case enum_ChunkType.Final:
                    restrictionDic.Add(enum_TileObjectType.Connection1x5, 1);
                    restrictionDic.Add(enum_TileObjectType.StagePortal2x2, 1);
                    restrictionDic.Add(enum_TileObjectType.EliteEnermySpawn1x1,1);
                    restrictionDic.Add(enum_TileObjectType.EliteTrigger1x1, -1);
                    restrictionDic.Add(enum_TileObjectType.EnermySpawn1x1, -1);
                    break;
            }
            return restrictionDic;
        }
    }
}