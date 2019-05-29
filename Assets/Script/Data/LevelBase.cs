
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;
using TTiles;
public class LevelBase : MonoBehaviour {
    public enum_LevelPrefabType E_PrefabType = enum_LevelPrefabType.Invalid;
    public enum_LevelType m_levelType { get; private set; }
    protected Transform tf_LevelItem;
    public System.Random m_seed { get; private set; }
    public void Init(TileMapData levelData,SLevelGenerate _levelGenerate, LevelItemBase[] _levelItems,enum_LevelType _levelType, System.Random _seed, List<enum_TileDirection> _connectedDireciton)
    {
        tf_LevelItem = transform.Find("Item");
        m_seed = _seed;
        m_levelType = _levelType;
        GenerateTileItems(_levelGenerate,_levelItems, levelData, _connectedDireciton);
    }
    #region TileMapInfos
    public List<LevelTilePortal> m_Portals { get; private set; } = new List<LevelTilePortal>();
    List<LevelTile> m_AllTiles=new List<LevelTile>();
    List<int> m_IndexEmpty=new List<int>();
    List<int> m_IndexMain=new List<int>();
    List<int> t_IndexTemp = new List<int>();
    Dictionary<enum_LevelItemType, List<LevelItemBase>> m_AllItems = new Dictionary<enum_LevelItemType, List<LevelItemBase>>();
    void GenerateTileItems(SLevelGenerate _itemData,LevelItemBase[] allItemPrefabs, TileMapData _data,List<enum_TileDirection> _connectedDireciton)
    {
        foreach (LevelItemBase levelItem in allItemPrefabs)
        {
            if (!m_AllItems.ContainsKey(levelItem.m_ItemType))
                m_AllItems.Add(levelItem.m_ItemType, new List<LevelItemBase>());
            m_AllItems[levelItem.m_ItemType].Add(levelItem);
        }

        //Select All Emptys And Create Data
        int index = 0;
        for (int i = 0; i < _data.m_MapData.Count; i++)
        {
            if (_data.m_MapData[i].m_Status != -1)
            {
                m_AllTiles.Add(new LevelTile(_data.m_MapData[i], GameExpression.E_BigmapDirection(transform.TransformDirection(_data.m_MapData[i].m_Offset))));
                m_IndexEmpty.Add(index++);
            }
        }

        //Generate Portal Pos
        TileAxis origin = new TileAxis(_data.I_Width/2, _data.I_Height/2);
        GenerateRangePortalTile(origin,_connectedDireciton,GameConst.I_TileMapPortalMinusOffset);

        //Generate All Items
        TCommon.TraversalUnchange(_itemData.m_ItemGenerate, (enum_LevelItemType type, RangeInt range) =>
        {
            GenerateRandomItemTile(type, m_seed.Next(range.start,range.end+1));
        });
        
        for (int i = 0; i < m_IndexMain.Count; i++)
        {
            LevelTileItemMain main = m_AllTiles[m_IndexMain[i]] as LevelTileItemMain;

            LevelItemBase itemMain = GameObject.Instantiate(m_AllItems[main.m_LevelItemType][main.m_LevelItemListIndex], tf_LevelItem);
            itemMain.transform.localPosition = main.m_Offset;
            itemMain.Init(this);
        }
    }

    void GenerateRandomItemTile(enum_LevelItemType type, int totalCount)
    {
        if (totalCount == 0)
            return;
        if (!m_AllItems.ContainsKey(type))
        {
            Debug.LogWarning("Current Level Does Not Contains Item That Type:" + type.ToString());
            return;
        }

        List<LevelItemBase> targetItems = m_AllItems[type];
        for (int i = 0; i < totalCount; i++)
        {
            int currentItemIndex = targetItems.RandomIndex(m_seed);
            LevelItemBase currentItem = targetItems[currentItemIndex];
            if (m_IndexEmpty.Count < currentItem.m_sizeXAxis * currentItem.m_sizeYAxis)
                continue;

            int currentTileIndex = RandomAvailableTileIndex(currentItem.m_sizeXAxis,currentItem.m_sizeYAxis, ref t_IndexTemp);
            if (currentTileIndex != -1)
            {
                m_AllTiles[currentTileIndex] = new LevelTileItemMain(m_AllTiles[currentTileIndex],currentItemIndex,currentItem.m_ItemType, t_IndexTemp);
                m_IndexMain.Add(currentTileIndex);
                m_IndexEmpty.Remove(currentTileIndex);
                foreach (int subTileIndex in t_IndexTemp)
                {
                    m_AllTiles[subTileIndex] = new LevelTileItemSub(m_AllTiles[subTileIndex],currentTileIndex);
                    m_IndexEmpty.Remove(subTileIndex);
                }
            }
        }
    }

    void GenerateRangePortalTile(TileAxis origin, List<enum_TileDirection> portalDirection,int minusRange)
    {
        for (int i = 0; i < portalDirection.Count; i++)
        {
            List<LevelTile> tileAvailable = m_AllTiles.FindAll(p => p.E_Direction == portalDirection[i]&&p.E_TileType== enum_TileType.Empty);
            LevelTile closestTile = tileAvailable[0];
            int closestOffset = int.MaxValue;
            for (int j = 0; j < tileAvailable.Count; j++)
            {
                int offset = origin.AxisOffset(tileAvailable[j].m_TileAxis);
                if (offset>minusRange&&closestOffset > offset)
                {
                    closestTile = tileAvailable[j];
                    closestOffset = offset;
                }
            }
            int index = m_AllTiles.IndexOf(closestTile);
            LevelTilePortal portal= new LevelTilePortal(m_AllTiles[index], portalDirection[i], transform.TransformPoint(m_AllTiles[index].m_Offset));
            m_AllTiles[index] = portal;
            m_Portals.Add(portal);
        }
    }
    int RandomAvailableTileIndex(int XCount,int YCount,ref List<int> areaIndexes)
    {
        if (XCount * YCount > m_IndexEmpty.Count)
            return -1;

        int checkCount = XCount * YCount + 1;
        for (int i = 0; i < checkCount; i++)
        {
            int randomTileIndex =    m_IndexEmpty.RandomItem(m_seed);
            if (CheckIndexTileAreaAvailable(randomTileIndex, XCount, YCount, ref areaIndexes))
                    return randomTileIndex;
        }
        return -1;
    }
    bool CheckIndexTileAreaAvailable(int tileIndex, int XCount, int YCount, ref List<int> areaIndexes)
    {
        areaIndexes.Clear();
        LevelTile origin = m_AllTiles[tileIndex];

        if (origin.E_TileType != enum_TileType.Empty)
            return false;

        for (int i = 0; i < XCount; i++)
        {
            for (int j = 0; j < YCount; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int index = m_AllTiles.FindIndex(p => p.m_TileAxis==origin.m_TileAxis+new TileAxis(i,j)&&p.E_TileType== enum_TileType.Empty);
                if (index == -1)
                    return false;

                areaIndexes.Add(index);
            }
        }
        return true;
    }
    #endregion
    #region Gizmos For Test
#if UNITY_EDITOR
    public bool b_BakeCircle = true;
    public int I_DiamCellCount = 64;
    public bool B_IgnoreUnavailable = true;
    public float F_HeightDetect = .5f;
    public bool b_showGizmos=true,b_showGameTiles=true,b_showDirection=true;
    public TileMapData gizmosMapData;
    private void OnDrawGizmos()
    {
        if (!b_showGizmos)
            return;

        if (b_showGameTiles&&UnityEditor.EditorApplication.isPlaying)            //Draw While Playing
        {
            for (int i = 0; i < m_AllTiles.Count; i++)
            {
                Color targetColor=Color.black;
                Vector3 positon = transform.position + transform.right * m_AllTiles[i].m_Offset.x + Vector3.up * m_AllTiles[i].m_Offset.y + transform.forward * m_AllTiles[i].m_Offset.z;
                float sizeParam = .5f;
                if (b_showDirection)
                {
                    sizeParam = .5f;
                    switch (m_AllTiles[i].E_Direction)
                    {
                        default:
                            targetColor = Color.magenta; sizeParam = 1f;
                            break;
                        case enum_TileDirection.Top:
                            targetColor = TCommon.ColorAlpha(Color.green, .5f);
                            break;
                        case enum_TileDirection.Bottom:
                            targetColor = TCommon.ColorAlpha(Color.red, .5f);
                            break;
                        case enum_TileDirection.Left:
                            targetColor = TCommon.ColorAlpha(Color.blue, .5f);
                            break;
                        case enum_TileDirection.Right:
                            targetColor = TCommon.ColorAlpha(Color.yellow, .5f); 
                            break;
                    }
                    if (m_AllTiles[i].E_TileType == enum_TileType.Portal)
                    {
                         sizeParam = 1.5f;
                    }
                }
                else
                {
                    switch (m_AllTiles[i].E_TileType)
                    {
                        default:
                            targetColor = Color.magenta; sizeParam = 1f;
                            break;
                        case enum_TileType.Empty:
                            targetColor = TCommon.ColorAlpha(Color.green, .3f);
                            break;
                        case enum_TileType.Main:
                            targetColor = TCommon.ColorAlpha(Color.red, .5f); sizeParam = 1f;
                            break;
                        case enum_TileType.Item:
                            targetColor = TCommon.ColorAlpha(Color.blue, .5f); sizeParam = 1f;
                            break;
                        case enum_TileType.Portal:
                            targetColor = TCommon.ColorAlpha(Color.white, .5f);sizeParam = 1f;
                            break;
                    }
                }
                Gizmos.color = targetColor;
                Gizmos.DrawSphere(positon, sizeParam * GameConst.F_LevelTileSize / 2);
            }
            return;
        }
        
        gizmosMapData = TResources.GetLevelData(gameObject.name);

        if (gizmosMapData == null)
            Debug.LogWarning("Please Bake This Level First");

        if (gizmosMapData == null || gizmosMapData.m_MapData == null)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < I_DiamCellCount; i++)
            {
                for (int j = 0; j < I_DiamCellCount; j++)
                {
                    if (b_BakeCircle && Vector2.SqrMagnitude(new Vector2(i - I_DiamCellCount / 2, j - I_DiamCellCount / 2)) >Mathf.Pow(I_DiamCellCount/2 * GameConst.F_LevelTileSize / 2 ,2))
                        continue;
                    Vector3 position = transform.position
                        + transform.forward * GameConst.F_LevelTileSize * j
                        + transform.right * GameConst.F_LevelTileSize * i
                        - transform.right * (GameConst.F_LevelTileSize * I_DiamCellCount / 2 - GameConst.F_LevelTileSize / 2)
                        - transform.forward * (GameConst.F_LevelTileSize * I_DiamCellCount / 2 - GameConst.F_LevelTileSize / 2);
                    Gizmos.DrawCube(position, new Vector3(GameConst.F_LevelTileSize / 2, 5f, GameConst.F_LevelTileSize / 2));
                }
            }
        }
        else
        {
            I_DiamCellCount = gizmosMapData.I_Width;
            List<TileMapData.TileInfo> nodes = gizmosMapData.m_MapData;
            foreach (TileMapData.TileInfo node in nodes)
            {
                Gizmos.color = node.m_Status == -1 ? Color.red : Color.green;
                Gizmos.DrawCube(transform.position + transform.right * node.m_Offset.x + Vector3.up * (node.m_Offset.y + .5f) + transform.forward * node.m_Offset.z, new Vector3(GameConst.F_LevelTileSize / 2, 1f, GameConst.F_LevelTileSize / 2));
            }
        }
    }
#endif
    #endregion
}
