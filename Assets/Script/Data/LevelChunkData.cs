using UnityEngine;
using TTiles;
using LevelSetting;
using System.Collections.Generic;

public class LevelChunkData : ScriptableObject {
    [SerializeField]
    protected enum_ChunkType m_Type;
    [SerializeField]
    protected int m_Width, m_Height;
    [SerializeField]
    protected ChunkTileData[] m_TileData;
    [SerializeField]
    protected ChunkConnectionData[] m_ConnectionIndex;
    public enum_ChunkType Type => m_Type;
    public int Width => m_Width;
    public int Height => m_Height;
    public ChunkConnectionData[] Connections => m_ConnectionIndex;
    public TileAxis m_Size => new TileAxis(Width, Height);
    public ChunkTileData[] GetData()
    {
        ChunkTileData[] data = new ChunkTileData[m_TileData.Length];
        m_TileData.CopyTo(data, 0);
        return data;
    } 

    public static LevelChunkData NewData(int width,int height, enum_ChunkType type, LevelTileEditor[,] transferData=null)
    {
        LevelChunkData data = CreateInstance<LevelChunkData>();
        data.m_Type = type;
        data.m_Width = width;
        data.m_Height = height;
        data.m_TileData = new ChunkTileData[data.m_Width * data.m_Height];
        for (int i = 0; i < data.Width; i++)
            for (int j = 0; j < data.Height; j++)
            {
                int index = TileTools.Get1DAxisIndex(new TileAxis( i, j), data.Width);
                ChunkTileData tileData = ChunkTileData.Default();
                if(transferData!=null&&index<data.m_TileData.Length&&new TileAxis(i,j).InRange(transferData))
                    tileData = transferData.Get(new TileAxis(i,j)).m_Data;
                data.m_TileData[index] =tileData;
            }
        return data;
    }
    public void SaveData(LevelChunkEditor chunk)
    {
        m_Width = chunk.m_Width;
        m_Height = chunk.m_Height;
        m_Type = chunk.m_ChunkType;
        List<ChunkConnectionData> m_Connections = new List<ChunkConnectionData>();
         m_TileData = new ChunkTileData[m_Width*m_Height];
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                int index = TileTools.Get1DAxisIndex(new TileAxis( i, j), m_Width);
                if (chunk.m_TilesData[i, j].m_Data.m_ObjectType == enum_TileObjectType.RConnection1x5)
                    m_Connections.Add(new ChunkConnectionData(new TileAxis(i,j), chunk.m_TilesData[i, j].m_Data.m_Direction, m_Width,m_Height));
                m_TileData[index] = chunk.m_TilesData[i, j].m_Data;
            }
        m_ConnectionIndex = m_Connections.ToArray();
    }

    public Texture2D CalculateMapTexture()
    {
        Texture2D texture = new Texture2D(Width,Height, TextureFormat.RGBA32,false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(CalculateMapTextureColors());
        texture.Apply();
        return texture;
    }

    public Color[] CalculateMapTextureColors()
    {
        Color[] colors = new Color[m_Width*m_Height];
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
                colors[TileTools.Get1DAxisIndex(new TileAxis(i, j),m_Width)]= m_TileData[TileTools.Get1DAxisIndex(new TileAxis( i, j), m_Width)].m_GroundType == enum_TileGroundType.Invalid ? Color.black : Color.white;
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                TileAxis axis = new TileAxis(i, j);
                ChunkTileData tileData = m_TileData[TileTools.Get1DAxisIndex(axis, m_Width)];
                if (!tileData.m_ObjectType.IsEditorTileObject())
                    continue;
                TileAxis size = tileData.m_ObjectType.GetSizeAxis(tileData.m_Direction);
                List<TileAxis> axies = TileTools.GetAxisRange(m_Height, m_Width, axis, size);
                Color tileColor = Color.clear;
                switch (tileData.m_ObjectType)
                {
                    case enum_TileObjectType.RConnection1x5:
                        tileColor = Color.green;
                        break;
                    case enum_TileObjectType.RChunkPortal2x2:
                    case enum_TileObjectType.RStagePortal2x2:
                        tileColor = Color.blue;
                        break;
                    case enum_TileObjectType.RPlayerSpawn1x1:
                        tileColor = Color.grey;
                        break;
                    case enum_TileObjectType.REventArea3x3:
                        tileColor = Color.yellow;
                        break;
                    case enum_TileObjectType.RBattleTrigger1x1:
                        tileColor = Color.magenta;
                        break;
                    case enum_TileObjectType.REliteEnermySpawn1x1:
                    case enum_TileObjectType.REnermySpawn1x1:
                        tileColor = Color.red;
                        break;
                }
                axies.Traversal((TileAxis objectAxis) => { colors[TileTools.Get1DAxisIndex(objectAxis, m_Width)] = tileColor; });
            }
        return colors;
    }

}

[System.Serializable]
public struct ChunkTileData
{
    public enum_TilePillarType m_PillarType;
    public enum_TileGroundType m_GroundType;
    public enum_TileObjectType m_ObjectType;
    public enum_TileDirection m_Direction;

    public ChunkTileData ChangeGroundType(enum_TileGroundType groundType)
    {
        m_GroundType = groundType;
        return this;
    }
    public ChunkTileData ChangePillarType(enum_TilePillarType pillarType)
    {
        m_PillarType = pillarType;
        return this;
    }
    public ChunkTileData ChangeObjectType(enum_TileObjectType objectType)
    {
        m_ObjectType = objectType;
        return this;
    }
    public ChunkTileData ChangeDirection(enum_TileDirection direction)
    {
        m_Direction = direction;
        return this;
    }
    public static ChunkTileData Default() => new ChunkTileData() { m_PillarType =  enum_TilePillarType.Invalid, m_GroundType =  enum_TileGroundType.Invalid, m_ObjectType =  enum_TileObjectType.Invalid,m_Direction= enum_TileDirection.Top };
    public static ChunkTileData Create(enum_TilePillarType pillarType,enum_TileGroundType groundType , enum_TileObjectType objectType, enum_TileDirection direction) => new ChunkTileData() { m_PillarType = pillarType, m_GroundType = groundType, m_ObjectType = objectType, m_Direction = direction };

    public static bool operator ==(ChunkTileData a, ChunkTileData b) => a.m_Direction==b.m_Direction&&a.m_GroundType==b.m_GroundType&&a.m_ObjectType==b.m_ObjectType&&a.m_PillarType==b.m_PillarType;
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
        if ((int)directionData % 2 == 1)
        {
            if (m_Axis.X == 0)
                m_Direction = enum_TileDirection.Left;
            else if (m_Axis.X == width - 1)
                m_Direction = enum_TileDirection.Right;
        }
        else
        {
            if (m_Axis.Y == 0)
                m_Direction = enum_TileDirection.Bottom;
            else if (m_Axis.Y == height-1)
                m_Direction = enum_TileDirection.Top;
        }

        if (m_Direction == enum_TileDirection.Invalid)
            Debug.LogError("Invalid Direction Found");
    }
}
