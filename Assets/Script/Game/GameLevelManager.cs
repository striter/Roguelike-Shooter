using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TTiles;
using UnityEngine.AI;
using System.Threading.Tasks;
using System.Linq;

public class GameLevelManager : SingletonMono<GameLevelManager>, ICoroutineHelperClass
{
    ObjectPoolListComponent<int, LevelChunkGame> m_ChunkPool;
    public Vector3 m_LevelCenter { get; private set; }
    public TileAxis m_MapSize { get; private set; }
    public Texture2D m_MapTexture { get; private set; }
    public Texture2D m_FogTexture { get; private set; }

    protected TileAxis m_QuadrantRange, m_QuadrantSize;
    public int m_PlayerAtQuadrant { get; private set; }

    #region Get
    public Vector2 GetOffsetPosition(Vector3 worldPosition)
    {
        Vector3 offset =  worldPosition / LevelConst.I_TileSize;
        return new Vector2(offset.x, offset.z);
    }

    public TileAxis GetMapAxis(Vector3 worldPosition)
    {
        Vector3 offset =  worldPosition / LevelConst.I_TileSize;
        return new TileAxis(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.z));
    }

    public float GetMapAngle(float cameraYAngle) => cameraYAngle;
    public LevelChunkGame GetQuadrantChunk(int quadrantIndex)
    {
        if (m_ChunkPool.ContainsItem(quadrantIndex))
            return m_ChunkPool.GetItem(quadrantIndex);
        Debug.LogError("Chun Index Not Found" + quadrantIndex);
        return null;
    }

    public LevelChunkGame GetPlayerAtQuadrant() => GetQuadrantChunk(m_PlayerAtQuadrant);
    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_ChunkPool = new ObjectPoolListComponent<int, LevelChunkGame>(transform.Find("GameChunk"),"PoolItem",(LevelChunkGame chunk)=>chunk.Init());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        NavigationManager.ClearNavMeshDatas();
    }

    #region Generate
    public IEnumerator Generate(enum_GameStyle style, System.Random random, Action<Dictionary<enum_ObjectEventType, List<ChunkGameObjectData>>> OnGenerateEditorObjects)
    {
        m_PlayerMinimapUpdateCheckAxis = -TileAxis.One;
        m_PlayerQuadrantActivateCheckAxis = -TileAxis.One;
        m_PlayerAtQuadrant = -1;
        m_ActiveCheckQuadrants.Clear();

        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        List<ChunkGenerateData> gameChunkGenerate = new List<ChunkGenerateData>();
        List<enum_ObjectEventType> mainChunkType = new List<enum_ObjectEventType>() { enum_ObjectEventType.Normal, enum_ObjectEventType.Final };
        List<enum_ObjectEventType> subChunkType = new List<enum_ObjectEventType>() { enum_ObjectEventType.Normal };

        LevelChunkData[] chunkDatas = TResources.GetChunkDatas();
        //Generate All Connected Chunks
        yield return Task.Run(() =>
        {
            short generateCount = 0;
            while (true)
            {
                generateCount++;
                if (generateCount > 10240)
                    throw new Exception("Generate Overtime!");

                List<LevelChunkData> availableChunkData = chunkDatas.ToList();
                gameChunkGenerate.Clear();
                Action<ChunkGenerateData, List<ChunkGenerateData>> ConnectGameData = (ChunkGenerateData startChunk, List<ChunkGenerateData> connectDatas) =>
                {
                    ChunkGenerateData preChunk = startChunk;
                    connectDatas.Traversal((ChunkGenerateData data) =>
                    {
                        availableChunkData.Remove(data.m_Data);
                        preChunk.OnConnectionSet(data.m_PreChunkConnectPoint);
                        data.OnConnectionSet(data.m_ChunkConnectPoint);
                        preChunk = data;
                        gameChunkGenerate.Add(data);
                    });
                };

                //Generate Main Chunks
                LevelChunkData startData = availableChunkData.RandomItem(random);
                gameChunkGenerate.Add(new ChunkGenerateData(gameChunkGenerate.Count, TileAxis.Zero, startData, enum_ObjectEventType.Start));
                availableChunkData.Remove(startData);

                //Gemerate Main Chunks
                List<ChunkGenerateData> mainConnectionChunks = null;
                if (!TryGenerateChunkDatas(gameChunkGenerate.Count, gameChunkGenerate[0], gameChunkGenerate, availableChunkData, mainChunkType, random, out mainConnectionChunks))
                    continue;
                ConnectGameData(gameChunkGenerate[0], mainConnectionChunks);

                bool subGenerateSuccessful = false;
                for (int i = 0; i < 2; i++)
                {
                    bool generateSuccessful = false;
                    mainConnectionChunks.TraversalRandomBreak((ChunkGenerateData mainChunk) =>
                    {
                        if (!mainChunk.HaveEmptyConnection())
                            return false;
                        List<ChunkGenerateData> subConnectionChunks = null;
                        if (TryGenerateChunkDatas(1000 * i, mainChunk, gameChunkGenerate, availableChunkData, subChunkType, random, out subConnectionChunks))
                        {
                            ConnectGameData(mainChunk, subConnectionChunks);
                            generateSuccessful = true;
                            return true;
                        }
                        return false;
                    }, random);
                    subGenerateSuccessful = generateSuccessful;
                    if (!subGenerateSuccessful)
                        break;
                }

                if (subGenerateSuccessful)
                    break;
            }
        }).TaskCoroutine();

        //Generate Chunk Datas Map/Texture/Quadrant
        int originX = 0, originY = 0, oppositeX = 0, oppositeY = 0;
        gameChunkGenerate.Traversal((ChunkGenerateData chunkData) =>        //Check MapOrigin/MapSize
        {
            TileAxis chunkOrigin = chunkData.m_Origin;
            TileAxis chunkOpposite = chunkOrigin + chunkData.m_Data.m_Size;
            if (oppositeX < chunkOpposite.X)
                oppositeX = chunkOpposite.X;
            if (oppositeY < chunkOpposite.Y)
                oppositeY = chunkOpposite.Y;
            if (originX > chunkOrigin.X)
                originX = chunkOrigin.X;
            if (originY > chunkOrigin.Y)
                originY = chunkOrigin.Y;
        });
        TileAxis mapDataFill = TileAxis.One * LevelConst.I_UIPlayerViewFadeRange;
        m_MapSize = new TileAxis(oppositeX - originX, oppositeY - originY) + mapDataFill * 2;
        //Total Game Map
        TileGenerateData?[,] mapTileDatas = new TileGenerateData?[m_MapSize.X, m_MapSize.Y];
        TileAxis _mapDataOrigin = new TileAxis(originX, originY) - mapDataFill;
        gameChunkGenerate.Traversal((ChunkGenerateData generateData) =>
        {
            ChunkTileData[] chunkData = generateData.m_Data.GetData();
            TileAxis _chunkWholeDataOrigin = generateData.m_Origin - _mapDataOrigin;
            for (int i = 0; i < generateData.m_Data.Width; i++)
            {
                for (int j = 0; j < generateData.m_Data.Height; j++)
                {
                    TileAxis chunkDataAxis = new TileAxis(i, j);
                    TileAxis wholeDataAxis = _chunkWholeDataOrigin + chunkDataAxis;
                    mapTileDatas[wholeDataAxis.X, wholeDataAxis.Y] = new TileGenerateData(generateData.m_EventType, chunkData[TileTools.Get1DAxisIndex(chunkDataAxis, generateData.m_Data.Width)]);
                }
            }
        });

        //Quadrant Generate
        List<ChunkQuadrantData> _quadrantDatas = new List<ChunkQuadrantData>();
        Dictionary<enum_ObjectEventType, List<ChunkGameObjectData>> editorObjectDatas = new Dictionary<enum_ObjectEventType, List<ChunkGameObjectData>>();
        m_QuadrantRange = (m_MapSize / LevelConst.I_QuadranteTileSize);
        m_QuadrantSize = TileAxis.One * LevelConst.I_QuadranteTileSize;

        TileAxis quadrantSizeEdge = new TileAxis(m_MapSize.X % m_QuadrantSize.X, m_MapSize.Y % m_QuadrantSize.Y);
        bool edgeQuadrantX = quadrantSizeEdge.X != 0;
        bool edgeQuadrantY = quadrantSizeEdge.Y != 0;
        m_QuadrantRange += new TileAxis(edgeQuadrantX ? 1 : 0, edgeQuadrantY ? 1 : 0);

        for (int i = 0; i < m_QuadrantRange.X; i++)
            for (int j = 0; j < m_QuadrantRange.Y; j++)
            {
                TileAxis quadrantAxis = new TileAxis(i, j);
                int quadrantIndex = TileTools.Get1DAxisIndex(quadrantAxis, m_QuadrantRange.X);
                TileAxis quadrantMapOrigin = quadrantAxis * m_QuadrantSize;
                TileAxis quadrantMapSize = m_QuadrantSize;
                if (edgeQuadrantX && i == m_QuadrantRange.X - 1)
                    quadrantMapSize.X = quadrantSizeEdge.X;
                if (edgeQuadrantY && j == m_QuadrantRange.Y - 1)
                    quadrantMapSize.Y = quadrantSizeEdge.Y;

                ChunkTileData[] quadrantDatas = new ChunkTileData[quadrantMapSize.X * quadrantMapSize.Y];
                bool validData = false;
                for (int x = 0; x < quadrantMapSize.X; x++)
                    for (int y = 0; y < quadrantMapSize.Y; y++)
                    {
                        TileAxis quadrantTileAxis = new TileAxis(x, y);
                        TileAxis mapDataAxis = quadrantMapOrigin + quadrantTileAxis;
                        TileGenerateData? mapTileData = mapTileDatas[mapDataAxis.X, mapDataAxis.Y];
                        ChunkTileData quadrantTileData = ChunkTileData.Empty();
                        if (mapTileData.HasValue)
                        {
                            quadrantTileData = mapTileData.Value.m_Data;
                            if (quadrantTileData.m_ObjectType.IsEditorTileObject())
                            {
                                enum_ObjectEventType eventType = mapTileData.Value.m_eventType;
                                Vector3 worldPosition = mapDataAxis.ToPosition() + quadrantTileData.m_ObjectType.GetSizeAxis(quadrantTileData.m_Direction).ToPosition() / 2f;
                                if (!editorObjectDatas.ContainsKey(eventType))
                                    editorObjectDatas.Add(eventType, new List<ChunkGameObjectData>());
                                editorObjectDatas[eventType].Add(new ChunkGameObjectData(quadrantIndex, quadrantTileData.m_ObjectType, worldPosition, quadrantTileData.m_Direction.ToRotation()));
                                quadrantTileData = quadrantTileData.ChangeObjectType(enum_TileObjectType.Invalid);
                            }
                            validData = true;
                        }

                        quadrantDatas[TileTools.Get1DAxisIndex(quadrantTileAxis, quadrantMapSize.X)] = quadrantTileData;
                    }

                if (!validData)
                    continue;
                _quadrantDatas.Add(new ChunkQuadrantData(quadrantIndex, quadrantAxis, new TileBounds(quadrantMapOrigin, quadrantMapSize), quadrantDatas));
            }

        m_ChunkPool.Clear();
        for (int i = 0; i < _quadrantDatas.Count; i++)
        {
            ChunkQuadrantData quadrantData = _quadrantDatas[i];
            LevelChunkGame chunk = m_ChunkPool.AddItem(quadrantData.m_QuadrantIndex);
            chunk.InitGameChunk(quadrantData, random);
            if (i % 5 == 0)
                yield return null;
        }
        _quadrantDatas = null;

        OnGenerateEditorObjects(editorObjectDatas);

        //GenerateNavigationData
        Bounds mapBounds = new Bounds();
        mapBounds.center = m_MapSize.ToPosition() / 2f;
        mapBounds.size = new Vector3(m_MapSize.X, .1f, m_MapSize.Y) * LevelConst.I_TileSize;
        NavigationManager.InitNavMeshData(transform, mapBounds);

        #region Generate Map Texture
        m_FogRevealation = new enum_MapFogType[m_MapSize.X, m_MapSize.Y];
        Color[,] mapColors = new Color[m_MapSize.X, m_MapSize.Y];
        Color[,] fogColors = new Color[m_MapSize.X, m_MapSize.Y];
        for (int i = 0; i < m_MapSize.X; i++)
        {
            for (int j = 0; j < m_MapSize.Y; j++)
            {
                m_FogRevealation[i, j] = enum_MapFogType.Heavy;
                mapColors[i, j] = LevelConst.C_MapTerrainInvalidColor;
                fogColors[i, j] = LevelConst.C_MapFogHeavyColor;
            }
        }

        gameChunkGenerate.Traversal((ChunkGenerateData chunkdata) =>
        {
            Color[] chunkColors = chunkdata.m_Data.CalculateMapTextureColors(false);
            int length = chunkColors.Length;
            for (int index = 0; index < length; index++)
            {
                TileAxis tileAxis = chunkdata.m_Origin - _mapDataOrigin + TileTools.GetAxisByIndex(index, chunkdata.m_Data.Width);
                mapColors[tileAxis.X, tileAxis.Y] = chunkColors[index];
            }
            chunkColors = null;
        });

        m_MapTexture = new Texture2D(m_MapSize.X, m_MapSize.Y, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
        m_FogTexture = new Texture2D(m_MapSize.X, m_MapSize.Y, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
        for (int i = 0; i < m_MapSize.X; i++)
        {
            for (int j = 0; j < m_MapSize.Y; j++)
            {
                m_MapTexture.SetPixel(i, j, mapColors[i, j]);
                m_FogTexture.SetPixel(i, j, fogColors[i, j]);
            }
        }
        m_MapTexture.Apply();
        m_FogTexture.Apply();
        mapColors = null;
        fogColors = null;
        #endregion
    }

    bool TryGenerateChunkDatas(int generateStartIndex, ChunkGenerateData generateStartChunk, List<ChunkGenerateData> intersectsCheckChunks, List<LevelChunkData> chunkDatas, List<enum_ObjectEventType> generateMainTypes, System.Random random, out List<ChunkGenerateData> chunkGenerateData)
    {
        chunkGenerateData = new List<ChunkGenerateData>();
        List<ChunkGenerateData> chunkIntersectsCheckData = new List<ChunkGenerateData>(intersectsCheckChunks);
        ChunkGenerateData previousChunkGenerate = generateStartChunk;
        for (int i = 0; i < generateMainTypes.Count; i++)
        {
            bool isConnectingFinalChunk = i == generateMainTypes.Count - 1;
            ChunkGenerateData nextChunkGenerate = null;
            previousChunkGenerate.m_Data.Connections.TraversalRandomBreak((int previousConnectionIndex) =>
            {
                if (previousChunkGenerate.m_ConnectPoint[previousConnectionIndex])
                    return false;

                ChunkConnectionData m_previousConnectionData = previousChunkGenerate.m_Data.Connections[previousConnectionIndex];
                enum_TileDirection nextConnectDirection = m_previousConnectionData.m_Direction.Inverse();
                chunkDatas.TraversalRandomBreak((LevelChunkData chunkData) =>
                {
                    if (!isConnectingFinalChunk && chunkData.Connections.Length <= 1)
                        return false;

                    chunkData.Connections.TraversalRandomBreak((int curConnectionIndex) =>
                    {
                        ChunkConnectionData _connectionData = chunkData.Connections[curConnectionIndex];
                        if (_connectionData.m_Direction != nextConnectDirection)
                            return false;

                        TileAxis nextChunkAxis = previousChunkGenerate.m_Origin + m_previousConnectionData.m_Axis - _connectionData.m_Axis;
                        nextChunkAxis = TileTools.DirectionAxis(nextChunkAxis, m_previousConnectionData.m_Direction);
                        TileBounds nextChunkSizeCheck = new TileBounds(nextChunkAxis + TileAxis.One, chunkData.m_Size - TileAxis.One * 2);
                        if (chunkIntersectsCheckData.Any(p => p.m_GenerateCheckBounds.Intersects(nextChunkSizeCheck)))
                            return false;

                        nextChunkGenerate = new ChunkGenerateData(generateStartIndex, nextChunkAxis, chunkData, generateMainTypes[i]);
                        nextChunkGenerate.SetPreConnectData(previousChunkGenerate.m_ChunkIndex, previousConnectionIndex, curConnectionIndex);
                        generateStartIndex++;
                        return true;
                    }, random);
                    return nextChunkGenerate != null;
                }, random);
                return nextChunkGenerate != null;
            }, random);

            if (nextChunkGenerate == null)
            {
                chunkGenerateData.Clear();
                return false;
            }
            chunkDatas.Remove(nextChunkGenerate.m_Data);
            chunkGenerateData.Add(nextChunkGenerate);
            chunkIntersectsCheckData.Add(nextChunkGenerate);
            previousChunkGenerate = nextChunkGenerate;
        }
        return true;
    }
    #endregion

    public void TickGameLevel(Vector3 playerPosition)
    {
        TileAxis playerMapAxis = GetMapAxis(playerPosition);
        if (m_PlayerMinimapUpdateCheckAxis != playerMapAxis)
        {
            m_PlayerMinimapUpdateCheckAxis = playerMapAxis;
            CheckMinimapUpdate(m_PlayerMinimapUpdateCheckAxis);
        }

        TileAxis playerAtQuadrant = GetQuadrantAxis(playerMapAxis);
        if (m_PlayerQuadrantActivateCheckAxis != playerAtQuadrant)
        {
            m_PlayerQuadrantActivateCheckAxis = playerAtQuadrant;
            CheckActiveQuadrants(m_PlayerQuadrantActivateCheckAxis);
        }

        CheckActiveCullQuadrants();
    }
    
    #region QuadrantActivate
    List<int> m_ActiveQuadrants = new List<int>();
    List<int> m_ActiveCheckQuadrants = new List<int>();
    TileAxis m_PlayerQuadrantActivateCheckAxis;

    TileAxis GetQuadrantAxis(TileAxis playerAxis)
    {
        TileAxis quadrantAxis = playerAxis / m_QuadrantSize;
        if (quadrantAxis.X == m_QuadrantRange.X)
            quadrantAxis.X = m_QuadrantRange.X - 1;
        if (quadrantAxis.Y == m_QuadrantRange.Y)
            quadrantAxis.Y = m_QuadrantRange.Y - 1;
        return quadrantAxis;
    }

    void CheckActiveQuadrants(TileAxis playerQuadrant)
    {
        m_ActiveCheckQuadrants.Clear();
        m_ChunkPool.m_ActiveItemDic.Traversal((LevelChunkGame chunk) =>
        {
            int sqrMagnitude = (playerQuadrant - chunk.m_QuadrantAxis).SqrMagnitude;
            if (sqrMagnitude == 0)
                m_PlayerAtQuadrant = chunk.m_QuadrantIndex;

            bool activeCheckQuadrant = sqrMagnitude <= 2;
            chunk.SetActivate(activeCheckQuadrant);
            if (!activeCheckQuadrant)
                return;
            m_ActiveCheckQuadrants.Add(chunk.m_QuadrantIndex);
        });
    }

    void CheckActiveCullQuadrants()
    {
        if (m_ActiveCheckQuadrants.Count == 0)
            return;

        m_ActiveQuadrants.Clear();
        Plane[] cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(CameraController.MainCamera);
        m_ActiveCheckQuadrants.Traversal((int quadrantIndex) =>
        {
            LevelChunkGame chunk = m_ChunkPool.GetItem(quadrantIndex);
            bool activeQuadrant = GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, chunk.m_ChunkCullBounds);
            chunk.SetActivate(activeQuadrant);
            if (!activeQuadrant)
                return;
            m_ActiveQuadrants.Add(quadrantIndex);
        });
    }

#if UNITY_EDITOR
    public bool m_DrawQuadrant = false;
    public bool m_DrawQuadrantActivateCull = false;
    private void OnDrawGizmos()
    {
        if (m_DrawQuadrant && m_ChunkPool != null && m_ChunkPool.Count > 0)
        {
            Gizmos.color = Color.white;
            Vector3 mapSize = m_MapSize.ToPosition();
            Gizmos.DrawWireCube(mapSize / 2, mapSize);

            m_ChunkPool.m_ActiveItemDic.Traversal((LevelChunkGame chunk) =>
            {
                bool playerAtQuadrant = m_PlayerQuadrantActivateCheckAxis == chunk.m_QuadrantAxis;
                bool activateCheckQuadrant = m_ActiveCheckQuadrants.Contains(chunk.m_QuadrantIndex);
                bool activateQuadrant = m_ActiveQuadrants.Contains(chunk.m_QuadrantIndex);

                Gizmos.color = playerAtQuadrant ? Color.red : (activateCheckQuadrant ? (activateQuadrant ? Color.green : Color.yellow) : Color.white);
                float height = (playerAtQuadrant ? 3f : (activateCheckQuadrant ? (activateQuadrant ? 2f : 1f) : 0f));

                if (m_DrawQuadrantActivateCull)
                {
                    if (activateQuadrant)
                        Gizmos.DrawWireCube(chunk.m_ChunkCullBounds.center, chunk.m_ChunkCullBounds.size);
                }
                else
                    Gizmos.DrawWireCube(chunk.m_ChunkBaseBounds.center, chunk.m_ChunkBaseBounds.size + Vector3.up * height);

            });
        }
    }
#endif
    #endregion

    #region Minimap Update
    TileAxis m_PlayerMinimapUpdateCheckAxis;
    bool m_MinimapUpdating = false;
    enum_MapFogType[,] m_FogRevealation;
    Dictionary<TileAxis, enum_MapFogType> m_FogRevealMarkup = new Dictionary<TileAxis, enum_MapFogType>();

    public bool CheckTileRevealed(int i, int j) => m_FogRevealation[i, j] == enum_MapFogType.Cleared;
    public void CheckMinimapUpdate(TileAxis playerMapPos)
    {
        if (m_MinimapUpdating)
            return;
        StartCoroutine(UpdateMinimap(playerMapPos));
    }

    IEnumerator UpdateMinimap(TileAxis updatePos)
    {
        m_MinimapUpdating = true;
        m_FogRevealMarkup.Clear();
        yield return Task.Run(() =>
        {
            for (int i = 0; i < m_MapSize.X; i++)
                for (int j = 0; j < m_MapSize.Y; j++)
                {
                    enum_MapFogType fogRevealation = m_FogRevealation[i, j];
                    if (fogRevealation == enum_MapFogType.Cleared)
                        continue;

                    float sqrMagnitude = (updatePos - new TileAxis(i, j)).SqrMagnitude;
                    if (sqrMagnitude > LevelConst.I_UIPlayerViewFadeSqrRange)
                        continue;

                    if (sqrMagnitude <= LevelConst.I_UIPlayerViewRevealSqrRange)
                    {
                        if (fogRevealation != enum_MapFogType.Cleared)
                            m_FogRevealMarkup.Add(new TileAxis(i, j), enum_MapFogType.Cleared);
                    }
                    else if (sqrMagnitude <= LevelConst.I_UIPlayerViewFadeSqrRange)
                    {
                        if (fogRevealation != enum_MapFogType.Faded)
                            m_FogRevealMarkup.Add(new TileAxis(i, j), enum_MapFogType.Faded);
                    }
                }
        }).TaskCoroutine();
        m_MinimapUpdating = false;

        UpdateMapFog(m_FogRevealMarkup);
        yield break;
    }

    public void ClearAllFog()
    {
        Dictionary<TileAxis, enum_MapFogType> _fogReveal = new Dictionary<TileAxis, enum_MapFogType>();
        for (int i = 0; i < m_MapSize.X; i++)
            for (int j = 0; j < m_MapSize.Y; j++)
                _fogReveal.Add(new TileAxis(i, j), enum_MapFogType.Cleared);
        UpdateMapFog(_fogReveal);
        _fogReveal = null;
    }

    void UpdateMapFog(Dictionary<TileAxis, enum_MapFogType> _fogReveal)
    {
        _fogReveal.Traversal((TileAxis axis, enum_MapFogType type) =>
        {
            if (m_FogRevealation[axis.X, axis.Y] == type)
                return;

            m_FogRevealation[axis.X, axis.Y] = type;
            m_FogTexture.SetPixel(axis.X, axis.Y, type == enum_MapFogType.Faded ? LevelConst.C_MapFogFadeColor : LevelConst.C_MapFogClearColor);
        });
        m_FogTexture.Apply();
    }

    #endregion

}

public static class LevelObjectManager
{
    public static void Register(TileItemBase[] chunkTileItems)
    {
        Clear();
        Dictionary<enum_TileSubType, List<TileItemBase>> itemDic = new Dictionary<enum_TileSubType, List<TileItemBase>>();
        chunkTileItems.Traversal((TileItemBase item) => {
            if (!itemDic.ContainsKey(item.m_Type))
                itemDic.Add(item.m_Type, new List<TileItemBase>());
            itemDic[item.m_Type].Add(GameObject.Instantiate(item));
        });

        itemDic.Traversal((enum_TileSubType type, List<TileItemBase> items) => {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!"); break;
                case enum_TileSubType.Terrain:
                    items.Traversal((TileItemBase item) =>
                    {
                        TileTerrainBase groundItem = item as TileTerrainBase;
                        ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.Register(groundItem.m_GroundType, groundItem, 1);
                    });
                    break;
                case enum_TileSubType.Object:
                    items.Traversal((TileItemBase item) =>
                    {
                        TileObjectBase objectItem = item as TileObjectBase;
                        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Register(objectItem.m_ObjectType, objectItem, 1);
                    });
                    break;
                case enum_TileSubType.EdgeObject:
                    items.Traversal((TileItemBase item) =>
                    {
                        TileEdgeObjectBase editorGroundItem = item as TileEdgeObjectBase;
                        ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Register(editorGroundItem.m_EdgeObjectType, editorGroundItem, 1);
                    });
                    break;
                case enum_TileSubType.Plants:
                    items.Traversal((TileItemBase item) =>
                    {
                        TilePlantsBase plantsItem = item as TilePlantsBase;
                        ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.Register(plantsItem.m_PlantsType, plantsItem, 1);
                    });
                    break;
                case enum_TileSubType.EditorGround:
                    items.Traversal((TileItemBase item) =>
                    {
                        LevelTileItemEditorTerrain editorGroundItem = item as LevelTileItemEditorTerrain;
                        ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.Register(editorGroundItem.m_EditorTerrainType, editorGroundItem, 1);
                    });
                    break;
            }
        });
    }

    public static void DestroyBatchedItem()
    {
        ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.DestroyPoolItem();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.DestroyPoolItem();
        ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.DestroyPoolItem();
        ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.DestroyPoolItem();
        ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.DestroyPoolItem();
    }

    public static void Clear()
    {
        ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.Destroy();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Destroy();
        ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Destroy();
        ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.Destroy();
        ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.Destroy();
    }
    
    public static bool HaveObjectItem(enum_TileObjectType type)=> ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Registed(type);
    public static bool HaveEdgeObjectItem(enum_TileEdgeObjectType type)=> ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Registed(type);

    public static TileObjectBase GetObjectItem(enum_TileObjectType type, Transform trans) => ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TileTerrainBase GetTerrainItem(enum_TileTerrainType type, Transform trans) => ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.Spawn(type, trans,Vector3.zero,Quaternion.identity);
    public static TileEdgeObjectBase GetEdgeObjectItem(enum_TileEdgeObjectType type, Transform trans) => ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TilePlantsBase GetPlantsItem(enum_TilePlantsType type, Transform trans) => ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static LevelTileItemEditorTerrain GetEditorGroundItem(enum_EditorTerrainType type,Transform trans)=> ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
}

public static class NavigationManager
{
    static Transform m_Transform;
    static NavMeshBuildSettings m_BuildSettings = NavMesh.GetSettingsByIndex(0);
    static NavMeshHit sampleHit;
    static NavMeshDataInstance m_DataInstance;
    static NavMeshData m_Data;
    static Bounds m_Bounds;
    public static void InitNavMeshData(Transform transform, Bounds bound)
    {
        ClearNavMeshDatas();
        m_Transform = transform;
        m_Bounds = bound;
        List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(bound,GameLayer.Mask.I_Static, NavMeshCollectGeometry.PhysicsColliders,0,new List<NavMeshBuildMarkup>(),m_Sources);
        m_Data = NavMeshBuilder.BuildNavMeshData(m_BuildSettings, m_Sources, m_Bounds, m_Transform.position, m_Transform.rotation);
        m_DataInstance = NavMesh.AddNavMeshData(m_Data);
        m_Sources = null;
    }

    public static void ClearNavMeshDatas()
    {
        NavMesh.RemoveNavMeshData(m_DataInstance);
    }

    public static Vector3 NavMeshPosition(Vector3 samplePosition)
    {
        if (NavMesh.SamplePosition(samplePosition, out sampleHit, 20,1>> 0))
            return sampleHit.position;
        return samplePosition;
    }
}