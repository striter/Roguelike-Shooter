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

        public static readonly Dictionary<enum_TileObjectType, int> m_ChunkRestriction = new Dictionary<enum_TileObjectType, int>() { {enum_TileObjectType.EConnection,2 },{enum_TileObjectType.EEntrance,-1 },{enum_TileObjectType.EEventArea,-1 },{enum_TileObjectType.EPortal,-1 }, { enum_TileObjectType.EEnermySpawn, -1 } };

    }

    public enum enum_ChunkPortalType
    {
        Invalid=-1,
        Battle,
        Event,
        Reward,
    }

    public enum enum_TileSubType
    {
        Invalid = -1,
        Object=1,
        Terrain=2,
        EdgeObject=3,
        Plants=4,

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

        PlantsCombine,

        EditorStart =50,       //Available During Editor
        EEntrance=51,
        EConnection=52,
        EPortal=53,
        EEventArea = 54,
        EEnermySpawn=55,
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

    public enum enum_TileEdgeObjectType
    {
        Invalid=-1,
        EdgeObjectFenceL_BL = 1,
        EdgeObjectFenceL_BLBR = 2,
        EdgeObjectFenceBL_BLTR = 3,
    }
    public enum enum_LevelEditorEditType
    {
        Invalid = -1,
        Terrain = 1,
        Object = 2,
        EdgeObject = 3,
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
                case enum_TileObjectType.Static2x2A:
                case enum_TileObjectType.Static2x2B:
                    size = TileAxis.One * 2;
                    break;
                case enum_TileObjectType.Static2x1B:
                case enum_TileObjectType.Static2x1A:
                    size = new TileAxis(2, 1);
                    break;
                case enum_TileObjectType.Static3x2A: 
                case enum_TileObjectType.Static3x2B:
                    size = new TileAxis(3, 2);
                    break;
                case enum_TileObjectType.Static3x3A:
                case enum_TileObjectType.Static3x3B:
                    size = TileAxis.One * 3;
                    break;
            }
            return TileTools.GetDirectionedSize(size, direction);
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
    
    
}