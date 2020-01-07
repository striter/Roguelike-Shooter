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
    protected LevelTileData[] m_TileData;
    [SerializeField]
    protected int[] m_ConnectionIndex;
    public enum_ChunkType Type => m_Type;
    public int Width => m_Width;
    public int Height => m_Height;
    public int[] Connections => m_ConnectionIndex;
    public LevelTileData[] Data
    {
        get
        {
            LevelTileData[] data = new LevelTileData[m_TileData.Length];
            m_TileData.CopyTo(data,0);
            return data;
        }
    } 

    public static LevelChunkData NewData(int width,int height, enum_ChunkType type, LevelTileEditor[,] transferData=null)
    {
        LevelChunkData data = CreateInstance<LevelChunkData>();
        data.m_Type = type;
        data.m_Width = width;
        data.m_Height = height;
        data.m_TileData = new LevelTileData[data.m_Width * data.m_Height];
        for (int i = 0; i < data.Width; i++)
            for (int j = 0; j < data.Height; j++)
            {
                int index = TileTools.GetAxisIndex(i, j, data.Width);
                LevelTileData tileData = LevelTileData.Default();
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
        List<int> m_Connections = new List<int>();
         m_TileData = new LevelTileData[m_Width*m_Height];
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                int index = TileTools.GetAxisIndex(i, j, m_Width);
                if (chunk.m_TilesData[i, j].m_Data.m_ObjectType == enum_TileObjectType.Connection1x5)
                    m_Connections.Add(index);
                m_TileData[index] = chunk.m_TilesData[i, j].m_Data;
            }
        m_ConnectionIndex = m_Connections.ToArray();
    }
}

[System.Serializable]
public struct LevelTileData
{
    public enum_TilePillarType m_PillarType;
    public enum_TileGroundType m_GroundType;
    public enum_TileObjectType m_ObjectType;
    public enum_TileDirection m_Direction;

    public LevelTileData ChangeGroundType(enum_TileGroundType groundType, enum_TileDirection direction)
    {
        m_GroundType = groundType;
        m_Direction = direction;
        return this;
    }
    public LevelTileData ChangePillarType(enum_TilePillarType pillarType, enum_TileDirection direction)
    {
        m_PillarType = pillarType;
        m_Direction = direction;
        return this;
    }
    public LevelTileData ChangeObjectType(enum_TileObjectType objectType, enum_TileDirection direction)
    {
        m_ObjectType = objectType;
        m_Direction = direction;
        return this;
    }
    public LevelTileData ChangeDirection(enum_TileDirection direction)
    {
        m_Direction = direction;
        return this;
    }
    public static LevelTileData Default() => new LevelTileData() { m_PillarType =  enum_TilePillarType.Invalid, m_GroundType =  enum_TileGroundType.Invalid, m_ObjectType =  enum_TileObjectType.Invalid,m_Direction= enum_TileDirection.Top };
    public static LevelTileData Create(enum_TilePillarType pillarType,enum_TileGroundType groundType , enum_TileObjectType objectType, enum_TileDirection direction) => new LevelTileData() { m_PillarType = pillarType, m_GroundType = groundType, m_ObjectType = objectType, m_Direction = direction };
}
