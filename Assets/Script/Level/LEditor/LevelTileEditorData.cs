using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using LevelSetting;
using System;
using System.Linq;

public class LevelTileEditorData : LevelTileEditor {

    public override bool isDataTile => true;
    public bool m_ContainsInfo => m_Data.m_TerrainType != enum_TileTerrainType.Invalid || m_Data.m_ObjectType != enum_TileObjectType.Invalid ;
    
    public void SetData(enum_TileObjectType type,enum_TileDirection direction, System.Random random)
    {
        if (m_Data.m_ObjectType == type && m_Data.m_Direction == direction)
            return;

        m_Data = m_Data.ChangeDirection(direction).ChangeObjectType(type);
        InitTile(m_Axis, m_Data,random);
    }

    public void SetData(enum_TileEdgeObjectType objectType, enum_TileDirection direction, System.Random random)
    {
        m_Data = m_Data.ChangeDirection(direction).ChangeEdgeObjectType(objectType);
        InitTile(m_Axis, m_Data, random);
    }

    public void SetEditorGround(enum_EditorGroundType groundType, System.Random random)
    {
        m_EditorGroundType = groundType;
        m_Data = m_Data.ChangeTerrainType(m_EditorGroundType == enum_EditorGroundType.Main ? enum_TileTerrainType.Ground : enum_TileTerrainType.River_4W_0G);
        InitTile(m_Axis, m_Data, random);
    }

    public void UpdateWaterTerrain(Dictionary<enum_TileDirection,enum_EditorGroundType> _edgeTerrains, Dictionary<enum_TileDirection, enum_EditorGroundType> _angleTerrains, System.Random random)
    {
        enum_TileTerrainType terrainType = enum_TileTerrainType.Ground;
        enum_TileDirection waterDirection = enum_TileDirection.Top;
        GetWaterTerrainType(_edgeTerrains,_angleTerrains,out terrainType,out waterDirection);
        m_Data = m_Data.ChangeTerrainType(terrainType).ChangeDirection(waterDirection);
        InitTile(m_Axis, m_Data, random);
    }

    void GetWaterTerrainType(Dictionary<enum_TileDirection, enum_EditorGroundType> _edgeTerrains, Dictionary<enum_TileDirection, enum_EditorGroundType> _angleTerrains,out enum_TileTerrainType terrainType,out enum_TileDirection terrainDirection)
    {
        terrainType = enum_TileTerrainType.River_0W;
        terrainDirection = enum_TileDirection.Top;
        int waterEdgeCount = _edgeTerrains.Values.Count(p => p == enum_EditorGroundType.Water);
        if (waterEdgeCount == 0)
        {
            terrainType = enum_TileTerrainType.River_0W;
            return;
        }
        
        if(waterEdgeCount==1)
        {
            terrainType = enum_TileTerrainType.River_1WT;
            terrainDirection = _edgeTerrains.Keys.First(p=>_edgeTerrains[p]== enum_EditorGroundType.Water);
            return;
        }

        List<enum_TileDirection> edgeWaters = _edgeTerrains.Keys.ToList().FindAll(p => _edgeTerrains[p] == enum_EditorGroundType.Water);
        List<enum_TileDirection> angleWaters = _angleTerrains.Keys.ToList().FindAll(p => _angleTerrains[p] == enum_EditorGroundType.Water);
        
        if(waterEdgeCount==2)
        {
            bool straight = edgeWaters[0].Inverse() == edgeWaters[1];
            if(straight)
            {
                terrainType = enum_TileTerrainType.River_2WTB;
                terrainDirection = edgeWaters[0];
                return;
            }

            terrainDirection = edgeWaters[0]== enum_TileDirection.Top&&edgeWaters[1]== enum_TileDirection.Left?  enum_TileDirection.Left: edgeWaters[0];
            terrainType = angleWaters.Contains(terrainDirection.EdgeNextCornor(true)) ? enum_TileTerrainType.River_2WRT_CW : enum_TileTerrainType.River_2WTR_CG;
            return;
        }


        if (waterEdgeCount==3)
        {
            terrainDirection = TileTools.m_EdgeDirections.Find(p => !edgeWaters.Contains(p)).Inverse();
            bool cornorWaterClockWise = angleWaters.Contains(terrainDirection.EdgeNextCornor(true));
            bool cornorWaterCounterClockWise = angleWaters.Contains(terrainDirection.EdgeNextCornor(false));
            terrainType = cornorWaterClockWise ? (cornorWaterCounterClockWise ? enum_TileTerrainType.River_3WLTR_LRW: enum_TileTerrainType.River_3WLTR_LG_RW) : (cornorWaterCounterClockWise? enum_TileTerrainType.River_3WLTR_LW_RG: enum_TileTerrainType.River_3WLTR_LRG);
            return;
        }


        if(waterEdgeCount==4)
        {
            List<enum_TileDirection> angleGrounds = _angleTerrains.Keys.ToList().FindAll(p => _angleTerrains[p] == enum_EditorGroundType.Main);
            switch (angleGrounds.Count)
            {
                case 0:
                    terrainType = enum_TileTerrainType.River_4W_0G;
                    break;
                case 1:
                    terrainType = enum_TileTerrainType.River_4W_1GTR;
                    terrainDirection = angleGrounds[0].AngleNextEdge(false);
                    break;
                case 2:

                    bool inverse = angleGrounds[0].Inverse() == angleGrounds[1];
                    if (inverse)
                    {
                        terrainType = enum_TileTerrainType.River_4W_2GTRBL;
                        terrainDirection = angleGrounds[0].AngleNextEdge(false);
                    }
                    else
                    {
                        terrainType = enum_TileTerrainType.River_4W_2GTRTL;
                        terrainDirection = angleGrounds[0] == enum_TileDirection.TopRight&& angleGrounds[1] == enum_TileDirection.TopLeft ? enum_TileDirection.Top : angleGrounds[1].AngleNextEdge(false);
                    }
                    break;
                case 3:
                    terrainType = enum_TileTerrainType.River_4W_3G_1WTR;
                    terrainDirection = angleWaters[0].AngleNextEdge(false);
                    break;
                case 4:
                    terrainType = enum_TileTerrainType.River_4W_4G;
                    break;
            }
            return;
        }

    }
}
