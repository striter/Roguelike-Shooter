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

    public void SetTerrain(enum_EditorTerrainType groundType, System.Random random)
    {
        m_EditorTerrainType = groundType;
        m_Data = m_Data.ChangeTerrainType(m_EditorTerrainType .GetDefaultTerrainType());
        InitTile(m_Axis, m_Data, random);
    }
    public void ClearData()
    {
        m_Data = ChunkTileData.Default();
        InitTile(m_Axis, m_Data, null);
    }

    public void UpdateTerrain(Dictionary<enum_TileDirection,enum_EditorTerrainType> _edgeTerrains, Dictionary<enum_TileDirection, enum_EditorTerrainType> _angleTerrains, System.Random random)
    {
        enum_TileTerrainType terrainType = enum_TileTerrainType.Plane;
        enum_TileDirection terrainDirection = enum_TileDirection.Top;
        CheckTerrain(_edgeTerrains,_angleTerrains,out terrainType,out terrainDirection);
        m_Data = m_Data.ChangeTerrainType(terrainType);
        if (terrainDirection != enum_TileDirection.Invalid)
            m_Data = m_Data.ChangeDirection(terrainDirection);
        InitTile(m_Axis, m_Data, random);
    }


    void CheckTerrain(Dictionary<enum_TileDirection, enum_EditorTerrainType> _edgeTerrains, Dictionary<enum_TileDirection, enum_EditorTerrainType> _angleTerrains,out enum_TileTerrainType terrainType,out enum_TileDirection terrainDirection)
    {
        terrainType = enum_TileTerrainType.Invalid;
        terrainDirection = enum_TileDirection.Invalid;
        switch(m_EditorTerrainType)
        {
            case enum_EditorTerrainType.Plane:
                terrainType = enum_TileTerrainType.Plane;
                break;
            case enum_EditorTerrainType.Highland:    //Broken Logic Cause Only Provided 3 Available Model!
                TileTools.m_EdgeDirections.Traversal((enum_TileDirection direction) => { if (!_edgeTerrains.ContainsKey(direction)) _edgeTerrains.Add(direction, enum_EditorTerrainType.Highland); });
                TileTools.m_AngleDirections.Traversal((enum_TileDirection direction) => { if (!_angleTerrains.ContainsKey(direction)) _angleTerrains.Add(direction, enum_EditorTerrainType.Highland); });
                terrainType = enum_TileTerrainType.Highland;
                List<enum_TileDirection> edgeSlope = _edgeTerrains.Keys.ToList().FindAll(p => _edgeTerrains[p] == enum_EditorTerrainType.Highland);

                if (edgeSlope.Count==2)
                {
                    bool straight = edgeSlope[0].Inverse() == edgeSlope[1];
                    if (straight)
                        return;

                    terrainDirection = edgeSlope[0] == enum_TileDirection.Top && edgeSlope[1] == enum_TileDirection.Left ? enum_TileDirection.Left : edgeSlope[0];
                    if (!edgeSlope.Contains(terrainDirection.EdgeNextCornor(true)))
                        terrainType = enum_TileTerrainType.Slope_2STR_CS;
                    return;
                }

                if (edgeSlope.Count == 3)
                {
                    List<enum_TileDirection> angleSlope = _angleTerrains.Keys.ToList().FindAll(p => _angleTerrains[p] == enum_EditorTerrainType.Highland);
                    terrainDirection = TileTools.m_EdgeDirections.Find(p => !edgeSlope.Contains(p)).Inverse();
                    bool cornorSlopeClockwise = angleSlope.Contains(terrainDirection.EdgeNextCornor(true));
                    bool cornorSlopeCounterClockwise = angleSlope.Contains(terrainDirection.EdgeNextCornor(false));
                    if (cornorSlopeClockwise && cornorSlopeCounterClockwise)
                        terrainType = enum_TileTerrainType.Slope_3SLTR_LRS;
                }

                if (edgeSlope.Count==4)
                {
                    List<enum_TileDirection> angleElse = _angleTerrains.Keys.ToList().FindAll(p => _angleTerrains[p] != enum_EditorTerrainType.Highland);
                    if (angleElse.Count != 1)
                        return;
                    terrainType = enum_TileTerrainType.Slope_4S_1GTR;
                    terrainDirection = angleElse[0].AngleNextEdge(false);
                }
                break;
            case enum_EditorTerrainType.River:
                TileTools.m_EdgeDirections.Traversal((enum_TileDirection direction) => { if (!_edgeTerrains.ContainsKey(direction)) _edgeTerrains.Add(direction, enum_EditorTerrainType.River); });
                TileTools.m_AngleDirections.Traversal((enum_TileDirection direction) => { if (!_angleTerrains.ContainsKey(direction)) _angleTerrains.Add(direction, enum_EditorTerrainType.River); });
                terrainType = enum_TileTerrainType.River_0P;
                int waterEdgeCount = _edgeTerrains.Values.Count(p => p == enum_EditorTerrainType.River);
                if (waterEdgeCount == 0)
                {
                    terrainType = enum_TileTerrainType.River_0P;
                    return;
                }

                if (waterEdgeCount == 1)
                {
                    terrainType = enum_TileTerrainType.River_1RT;
                    terrainDirection = _edgeTerrains.Keys.First(p => _edgeTerrains[p] == enum_EditorTerrainType.River);
                    return;
                }

                List<enum_TileDirection> edgeWaters = _edgeTerrains.Keys.ToList().FindAll(p => _edgeTerrains[p] == enum_EditorTerrainType.River);
                List<enum_TileDirection> angleWaters = _angleTerrains.Keys.ToList().FindAll(p => _angleTerrains[p] == enum_EditorTerrainType.River);

                if (waterEdgeCount == 2)
                {
                    bool straight = edgeWaters[0].Inverse() == edgeWaters[1];
                    if (straight)
                    {
                        terrainType = enum_TileTerrainType.River_2RTB;
                        terrainDirection = edgeWaters[0];
                        return;
                    }

                    terrainDirection = edgeWaters[0] == enum_TileDirection.Top && edgeWaters[1] == enum_TileDirection.Left ? enum_TileDirection.Left : edgeWaters[0];
                    terrainType = angleWaters.Contains(terrainDirection.EdgeNextCornor(true)) ? enum_TileTerrainType.River_2RRT_CR : enum_TileTerrainType.River_2RTR_CP;
                    return;
                }


                if (waterEdgeCount == 3)
                {
                    terrainDirection = TileTools.m_EdgeDirections.Find(p => !edgeWaters.Contains(p)).Inverse();
                    bool cornorWaterClockWise = angleWaters.Contains(terrainDirection.EdgeNextCornor(true));
                    bool cornorWaterCounterClockWise = angleWaters.Contains(terrainDirection.EdgeNextCornor(false));
                    terrainType = cornorWaterClockWise ? (cornorWaterCounterClockWise ? enum_TileTerrainType.River_3RLTR_LRR : enum_TileTerrainType.River_3RLTR_LP_RR) : (cornorWaterCounterClockWise ? enum_TileTerrainType.River_3RLTR_LR_RP : enum_TileTerrainType.River_3RLTR_LRP);
                    return;
                }


                if (waterEdgeCount == 4)
                {
                    List<enum_TileDirection> angleElse = _angleTerrains.Keys.ToList().FindAll(p => _angleTerrains[p] != enum_EditorTerrainType.River);
                    switch (angleElse.Count)
                    {
                        case 0:
                            terrainType = enum_TileTerrainType.River_4R_0P;
                            break;
                        case 1:
                            terrainType = enum_TileTerrainType.River_4R_1PTR;
                            terrainDirection = angleElse[0].AngleNextEdge(false);
                            break;
                        case 2:

                            bool inverse = angleElse[0].Inverse() == angleElse[1];
                            if (inverse)
                            {
                                terrainType = enum_TileTerrainType.River_4R_2PTRBL;
                                terrainDirection = angleElse[0].AngleNextEdge(false);
                            }
                            else
                            {
                                terrainType = enum_TileTerrainType.River_4R_2PTRTL;
                                terrainDirection = angleElse[0] == enum_TileDirection.TopRight && angleElse[1] == enum_TileDirection.TopLeft ? enum_TileDirection.Top : angleElse[1].AngleNextEdge(false);
                            }
                            break;
                        case 3:
                            terrainType = enum_TileTerrainType.River_4R_3P_1RTR;
                            terrainDirection = angleWaters[0].AngleNextEdge(false);
                            break;
                        case 4:
                            terrainType = enum_TileTerrainType.River_4R_4P;
                            break;
                    }
                    return;
                }
                break;
        }

    }
}
