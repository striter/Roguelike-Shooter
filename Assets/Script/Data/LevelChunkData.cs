﻿using UnityEngine;
using TTiles;
using LevelSetting;
using System.Collections.Generic;

public class LevelChunkData : ScriptableObject {
    [SerializeField]
    protected int m_Width, m_Height;
    [SerializeField]
    protected ChunkTileData[] m_TileData;
    [SerializeField]
    protected ChunkConnectionData[] m_Connections;
    public int Width => m_Width;
    public int Height => m_Height;
    public TileAxis m_Size => new TileAxis(Width, Height);
    public ChunkConnectionData[] Connections => m_Connections;
    public ChunkTileData[] GetData()
    {
        ChunkTileData[] data = new ChunkTileData[m_TileData.Length];
        m_TileData.CopyTo(data, 0);
        return data;
    } 

    public static LevelChunkData NewData(int width,int height)
    {
        LevelChunkData data = CreateInstance<LevelChunkData>();
        data.m_Width = width;
        data.m_Height = height;
        data.m_TileData = new ChunkTileData[data.m_Width * data.m_Height];
        for (int i = 0; i < data.Width; i++)
            for (int j = 0; j < data.Height; j++)
                data.m_TileData[TileTools.Get1DAxisIndex(new TileAxis(i, j), data.Width)] = ChunkTileData.EditorDefault();
        return data;
    }
    public static LevelChunkData NewData( int count,enum_TileDirection direction, LevelTileEditor[,] transferData)
    {
        LevelChunkData data = NewData(transferData.GetLength(0), transferData.GetLength(1));
        int xResize=0;
        int yResize=0;
        int xOffset=0;
        int yOffset=0;

        switch(direction)
        {
            case enum_TileDirection.Top:
                yResize = count;
                break;
            case enum_TileDirection.Bottom:
                yResize = count;
                yOffset = count;
                break;
            case enum_TileDirection.Left:
                xResize = count;
                xOffset = count;
                break;
            case enum_TileDirection.Right:
                xResize = count;
                break;
            case enum_TileDirection.TopRight:
                yResize = count;
                xResize = count;
                break;
            case enum_TileDirection.TopLeft:
                xResize = count;
                xOffset = count;
                yResize = count;
                break;
            case enum_TileDirection.BottomLeft:
                xResize = count;
                xOffset = count;
                yResize = count;
                yOffset = count;
                break;
            case enum_TileDirection.BottomRight:
                yResize = count;
                yOffset = count;
                xResize = count;
                break;
        }
        data.m_Width += xResize;
        data.m_Height += yResize;
        data.m_TileData = new ChunkTileData[data.m_Width * data.m_Height];
        for (int i = 0; i < data.Width; i++)
            for (int j = 0; j < data.Height; j++)
            {
                ChunkTileData tileData = ChunkTileData.EditorDefault();
                int index = TileTools.Get1DAxisIndex(new TileAxis(i, j), data.Width);
                TileAxis preTileAxis = new TileAxis(i-xOffset,j-yOffset);
                if ( index < data.m_TileData.Length && preTileAxis.InRange(transferData))
                    tileData = transferData.Get(preTileAxis).m_Data;
                data.m_TileData[index] = tileData;
            }
        return data;
    }

    public void SaveData(LevelChunkEditor chunk)
    {
        m_Width = chunk.m_Width;
        m_Height = chunk.m_Height;
        m_TileData = new ChunkTileData[m_Width*m_Height];
        List<ChunkConnectionData> m_Connections = new List<ChunkConnectionData>();
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                int index = TileTools.Get1DAxisIndex(new TileAxis( i, j), m_Width);
                m_TileData[index] = chunk.m_TilesData[i, j].m_Data;
                if (chunk.m_TilesData[i, j].m_Data.m_ObjectType == enum_TileObjectType.EConnection)
                {
                    ChunkConnectionData connectionData = new ChunkConnectionData(new TileAxis(i, j), chunk.m_TilesData[i, j].m_Data.m_Direction, m_Width, m_Height);
                    m_Connections.Add(connectionData);
                    m_TileData[index] = chunk.m_TilesData[i, j].m_Data.ChangeDirection(connectionData.m_Direction);
                    continue;
                }
            }
        this.m_Connections = m_Connections.ToArray();
    }

    public Texture2D CalculateEditorChunkTexture()
    {
        Texture2D texture = new Texture2D(Width,Height, TextureFormat.RGBA32,false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(CalculateMapTextureColors(true));
        texture.Apply();
        return texture;
    }

    public Color[] CalculateMapTextureColors(bool editor)
    {
        Color[] colors = new Color[m_Width*m_Height];
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                enum_TileTerrainType terrainType = m_TileData[TileTools.Get1DAxisIndex(new TileAxis(i, j), m_Width)].m_TerrainType;
                enum_EditorTerrainType editorTerrainType = terrainType.GetEditorGroundType();
                switch(editorTerrainType)
                {
                    case enum_EditorTerrainType.Invalid:
                        colors[TileTools.Get1DAxisIndex(new TileAxis(i, j), m_Width)] = LevelConst.C_MapTerrainInvalidColor;
                        break;
                    case enum_EditorTerrainType.Highland:
                        colors[TileTools.Get1DAxisIndex(new TileAxis(i, j), m_Width)] = LevelConst.C_MapTerrainHighlandColor;
                        break;
                    case enum_EditorTerrainType.River:
                        colors[TileTools.Get1DAxisIndex(new TileAxis(i, j), m_Width)] = LevelConst.C_MapTerrainRiverColor;
                        break;
                    case enum_EditorTerrainType.Plane:
                        colors[TileTools.Get1DAxisIndex(new TileAxis(i, j), m_Width)] = LevelConst.C_MapTerrainPlaneColor;
                        break;
                }
            }
        if (editor)
            for (int i = 0; i < m_Width; i++)
                for (int j = 0; j < m_Height; j++)
                {
                    TileAxis axis = new TileAxis(i, j);
                    ChunkTileData tileData = m_TileData[TileTools.Get1DAxisIndex(axis, m_Width)];
                    TileAxis size = tileData.m_ObjectType.GetSizeAxis(tileData.m_Direction);
                    List<TileAxis> axies = TileTools.GetAxisRange(m_Width, m_Height, axis, axis + size - TileAxis.One);
                    Color tileColor = Color.clear;
                    switch (tileData.m_ObjectType)
                    {
                        case enum_TileObjectType.Invalid:
                            break;
                        default:
                            tileColor = Color.grey;
                            break;
                        case enum_TileObjectType.EConnection:
                            tileColor = Color.magenta;
                            break;
                        case enum_TileObjectType.EMainEvent3x3:
                            tileColor = Color.green;
                            break;
                        case enum_TileObjectType.ERandomEvent3x3:
                            tileColor = Color.white;
                            break;
                        case enum_TileObjectType.EEnermySpawn:
                            tileColor = Color.red;
                            break;
                        case enum_TileObjectType.Dangerzone:
                            tileColor = Color.yellow;
                            break;
                    }
                    if (tileColor != Color.clear)
                        axies.Traversal((TileAxis objectAxis) => { colors[TileTools.Get1DAxisIndex(objectAxis, m_Width)] = tileColor; });
                }
        return colors;
    }

}

[System.Serializable]
public struct ChunkTileData
{
    public enum_TileTerrainType m_TerrainType;
    public enum_TileObjectType m_ObjectType;
    public enum_TileEdgeObjectType m_EdgeObjectType;
    public enum_TileDirection m_Direction;

    public bool HasValue => m_TerrainType != enum_TileTerrainType.Invalid || m_ObjectType != enum_TileObjectType.Invalid || m_EdgeObjectType != enum_TileEdgeObjectType.Invalid;
    public ChunkTileData ChangeTerrainType(enum_TileTerrainType groundType)
    {
        m_TerrainType = groundType;
        return this;
    }
    public ChunkTileData ChangeObjectType(enum_TileObjectType objectType)
    {
        m_ObjectType = objectType;
        return this;
    }
    public ChunkTileData ChangeEdgeObjectType(enum_TileEdgeObjectType edgeObjectType)
    {
        m_EdgeObjectType = edgeObjectType;
        return this;
    }
    public ChunkTileData ChangeDirection(enum_TileDirection direction)
    {
        m_Direction = direction;
        return this;
    }

    public static ChunkTileData EditorDefault() => new ChunkTileData() { m_TerrainType =  enum_TileTerrainType.Plane, m_ObjectType =  enum_TileObjectType.Invalid,m_EdgeObjectType= enum_TileEdgeObjectType.Invalid,m_Direction= enum_TileDirection.Top };
    public static ChunkTileData Empty() => new ChunkTileData() { m_TerrainType = enum_TileTerrainType.Invalid, m_ObjectType = enum_TileObjectType.Invalid, m_EdgeObjectType = enum_TileEdgeObjectType.Invalid, m_Direction = enum_TileDirection.Invalid };
    public static ChunkTileData Create(enum_TileTerrainType groundType ,  enum_TileObjectType objectType,enum_TileEdgeObjectType edgeObjectType, enum_TileDirection direction) => new ChunkTileData() { m_TerrainType = groundType, m_ObjectType = objectType,m_EdgeObjectType=edgeObjectType, m_Direction = direction };


    public static bool operator ==(ChunkTileData a, ChunkTileData b) => a.m_Direction==b.m_Direction&&a.m_TerrainType==b.m_TerrainType&&a.m_ObjectType==b.m_ObjectType;
    public static bool operator !=(ChunkTileData a, ChunkTileData b) => !(a==b);
    public override bool Equals(object obj)=> base.Equals(obj);
    public override int GetHashCode()=> base.GetHashCode();
}
[System.Serializable]
public struct ChunkConnectionData
{
    public TileAxis m_Axis;
    public enum_TileDirection m_Direction;
    public ChunkConnectionData(TileAxis axis,enum_TileDirection directionData, int width, int height)
    {
        m_Axis = axis;
        m_Direction = enum_TileDirection.Invalid;

        int connectionIndex = 0;

        if (m_Axis.X == 0)
        {
            m_Direction = enum_TileDirection.Left;
            connectionIndex++;
        }
        if (m_Axis.X == width - 1)
        {
            m_Direction = enum_TileDirection.Right;
            connectionIndex++;
        }
        if (m_Axis.Y == 0)
        {
            m_Direction = enum_TileDirection.Bottom;
            connectionIndex++;

        }
        if (m_Axis.Y == height - 1)
        {
            m_Direction = enum_TileDirection.Top;
            connectionIndex++;
        }

        if (m_Direction == enum_TileDirection.Invalid)
            Debug.LogError("Invalid Direction Found!");
        if (connectionIndex != 1)
            Debug.LogError("Cornor Direction Found!");
    }
}
