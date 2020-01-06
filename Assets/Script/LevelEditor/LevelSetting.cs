using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
namespace LevelSetting
{
    public enum enum_ChunkType { Invalid = -1, Begin, Battle, Event, CrossRoad, Final }
    
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
    }

    public enum enum_TileObjectType
    {
        Invalid=-1,
        Main=1,
        Sub1=2,
        Sub2=3,
        Sub3=4,
        Sub4=5,
        Manmade=6,
        Object1x1 =11,
        Object1x2=12,
        Object2x1=13,
        Object2x2=14,
        Object2x3=15,
        Object3x2 = 16,
        Object3x3 =17,
        Connection1x5 = 21,
        ChunkPortal2x2 = 22,
        StagePortal2x2 =23,
        EventArea3x3 = 24,
        DangerZone1x1 = 25,
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
        public static TileAxis GetSizeAxis(this enum_TileObjectType type,enum_TileDirection direction)
        {
            TileAxis size = TileAxis.One;
            switch (type)
            {
                case enum_TileObjectType.StagePortal2x2:
                case enum_TileObjectType.Object2x2:
                case enum_TileObjectType.ChunkPortal2x2: size= TileAxis.One * 2;
                    break;
                case enum_TileObjectType.Object1x2: size= new TileAxis(1, 2);
                    break;
                case enum_TileObjectType.Object2x1: size = new TileAxis(2, 1);
                    break;
                case enum_TileObjectType.Object2x3: size = new TileAxis(2, 3);
                    break;
                case enum_TileObjectType.Object3x2: size = new TileAxis(3, 2);
                    break;

                case enum_TileObjectType.EventArea3x3:
                case enum_TileObjectType.Object3x3: size = TileAxis.One * 3;
                    break;
                case enum_TileObjectType.Connection1x5: size = new TileAxis(5, 1); break;

            }
            return TileTools.GetDirectionedSize(size, direction);
        }
    }
}