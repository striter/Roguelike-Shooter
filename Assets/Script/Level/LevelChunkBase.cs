using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using TTiles;
using System;

public class LevelChunkBase : MonoBehaviour
{
    public enum_LevelType m_ChunkType { get; private set; }
    public int m_Width { get; private set; }
    public int m_Height { get; private set; }
    protected ObjectPoolListComponent<int, LevelTileBase> m_TilePool { get; private set; }
    public Texture2D m_TerrainMapping { get; private set; }
    public Vector4 m_TerrainMapping_Origin_Scale { get; private set; }
    public virtual void Init()
    {
        m_TilePool = new ObjectPoolListComponent<int, LevelTileBase>(transform.Find("TilePool"), "TileItem");
    }

    protected void InitData(LevelChunkData _data,System.Random _random,Func<TileAxis,ChunkTileData,ChunkTileData> DataObjectCheck=null)
    {
        m_TilePool.ClearPool();

        m_ChunkType = _data.Type;
        m_Width = _data.Width;
        m_Height = _data.Height;
        ChunkTileData[] tileData= _data.GetData();
        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                TileAxis axis = new TileAxis(i, j);
                int index = TileTools.Get1DAxisIndex(axis, m_Width);
                ChunkTileData data = tileData[index];
                if (DataObjectCheck!=null)
                    data = DataObjectCheck(axis, data);

                OnTileInit(m_TilePool.AddItem(index), axis, data,_random);

            }


        int mapTileSize = 8;
        int mapTileBlend = 1;
        int mapWidth = m_Width * mapTileSize;
        int mapHeight = m_Height * mapTileSize;
        m_TerrainMapping = new Texture2D(mapWidth, mapHeight, TextureFormat.ARGB32,false,true) { hideFlags = HideFlags.HideAndDontSave, wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
        m_TerrainMapping_Origin_Scale = new Vector4(0, 0, m_Width * LevelConst.I_TileSize, m_Height * LevelConst.I_TileSize);
        for(int i=0;i< mapWidth; i++)
            for(int j=0;j< mapHeight; j++)
                m_TerrainMapping.SetPixel(i,j,LevelConst.C_MapTexture_Ground);

        for (int i = 0; i < m_Width; i++)
            for (int j = 0; j < m_Height; j++)
            {
                bool isMapItem = false; 
                Color mapItemCenterColor=Color.clear;
                switch (tileData[TileTools.Get1DAxisIndex(new TileAxis(i,j), m_Width)].m_TerrainMap)
                {
                    case enum_TileTerrainMap.Plants:
                        isMapItem = true;
                        mapItemCenterColor = LevelConst.C_MapTexture_Plants;
                        break;
                }
                if (!isMapItem)
                    continue;
                TileAxis mapItemCenterStart = new TileAxis(i, j) * mapTileSize;
                TileAxis mapItemCenterEnd = mapItemCenterStart + TileAxis.One * (mapTileSize-1);
                TileAxis mapItemStart = mapItemCenterStart - TileAxis.One*mapTileBlend;
                TileAxis mapItemEnd = mapItemCenterEnd + TileAxis.One * mapTileBlend;


                TileTools.GetAxisRange(mapWidth, mapHeight, mapItemStart, mapItemEnd).Traversal((TileAxis mapAxis) => {
                    m_TerrainMapping.SetPixel(mapAxis.X, mapAxis.Y,  mapItemCenterColor);
                });
            }

        m_TerrainMapping.Apply();
        Shader.SetGlobalVector("_TerrainMapScale", m_TerrainMapping_Origin_Scale);
        Shader.SetGlobalTexture("_TerrainMapTex", m_TerrainMapping);
    }

    protected virtual void OnTileInit(LevelTileBase tile,TileAxis axis,ChunkTileData data,System.Random random)
    {
        tile.InitTile(axis, data,random);
    }
}
