
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using System;

public class LevelBase : MonoBehaviour {
    public List<LevelTile> m_MapData;
    public enum_TileType m_levelType { get; private set; }
    protected Transform tf_LevelItem,tf_Model;
    public System.Random m_seed { get; private set; }
    public void Init()
    {
        tf_LevelItem = transform.Find("Item");
        tf_Model = transform.Find("Model");
    }

    #region TileMapInfos
    List<LevelTile> m_AllTiles=new List<LevelTile>();
    int m_PortalIndex;
    List<int> m_IndexEmptyInner = new List<int>();
    List<int> m_IndexEmptyOuter = new List<int>();
    List<int> m_IndexBorder = new List<int>();
    List<int> m_IndexItemMain=new List<int>();
    List<int> t_IndexTemp = new List<int>();
    Dictionary<enum_LevelItemType, List<LevelItemBase>> m_AllItemPrefabs = new Dictionary<enum_LevelItemType, List<LevelItemBase>>();
    public Dictionary<LevelItemBase,int> GenerateTileItems(SLevelGenerate _innerData,SLevelGenerate _outerData, Dictionary<enum_LevelItemType, List<LevelItemBase>> allItemPrefabs, enum_TileType _levelType, System.Random _seed, bool showPortal)
    {
        m_AllItemPrefabs = allItemPrefabs;
        m_seed = _seed;
        m_levelType = _levelType;

        int innerLength = _innerData.m_Length.RandomRangeInt(m_seed);
        int outerLength = _outerData.m_Length.RandomRangeInt(m_seed);
        outerLength = (innerLength + outerLength) % 2 == 0 ? outerLength+1 : outerLength;
        int totalRadius = innerLength + outerLength;
        tf_Model.localScale = Vector3.one * totalRadius*2;
        //Create Data
        int index = 0;
        TileAxis origin = new TileAxis(-totalRadius/2 , -totalRadius/2 );
        int borderLength = innerLength/2;
        for (int i = 0; i < totalRadius; i++)
        {
            for (int j = 0; j < totalRadius; j++)
            {
                TileAxis curTile = origin+new TileAxis(i, j);
                int curX = Mathf.Abs(curTile.X);
                int curY = Mathf.Abs(curTile.Y);
                enum_LevelTileOccupy occupation = enum_LevelTileOccupy.Invalid;
                if (curX<borderLength&&curY<borderLength)
                    occupation = enum_LevelTileOccupy.Inner;
                else if (curX > borderLength || curY > borderLength)
                    occupation = enum_LevelTileOccupy.Outer;
                else
                    occupation = enum_LevelTileOccupy.Border;

                m_AllTiles.Add(new LevelTile(curTile, occupation));
                switch (occupation)
                {
                    case enum_LevelTileOccupy.Inner:
                        m_IndexEmptyInner.Add(index);
                        break;
                    case enum_LevelTileOccupy.Outer:
                        m_IndexEmptyOuter.Add(index);
                        break;
                    case enum_LevelTileOccupy.Border:
                        m_IndexBorder.Add(index);
                        break;
                }
                index++;
            }
        }
        GeneratePortalTile(showPortal,TileAxis.Zero, 0);
        GenerateBorderTile(m_IndexBorder);
        GenerateRandomMainTile(_innerData,m_IndexEmptyInner);
        GenerateRandomMainTile(_outerData, m_IndexEmptyOuter);

        m_IndexItemMain.AddRange(m_IndexBorder);
        Dictionary<LevelItemBase, int> itemCountDic = new Dictionary<LevelItemBase, int>();
        itemCountDic.Add(m_AllItemPrefabs[enum_LevelItemType.Portal][0], 1);
        for (int i = 0; i < m_IndexItemMain.Count; i++)
        {
            LevelTileItem main = m_AllTiles[m_IndexItemMain[i]] as LevelTileItem;
            LevelItemBase item = m_AllItemPrefabs[main.m_LevelItemType][main.m_LevelItemListIndex];
            if (!itemCountDic.ContainsKey(item))
                itemCountDic.Add(item,0);
            itemCountDic[item]++;
        }
        return itemCountDic;
    }
    public void ShowAllItems()
    {
        for (int i = 0; i < m_IndexItemMain.Count; i++)
        {
            LevelTileItem main = m_AllTiles[m_IndexItemMain[i]] as LevelTileItem;
            LevelItemBase itemMain = ObjectManager.SpawnLevelItem (m_AllItemPrefabs[main.m_LevelItemType][main.m_LevelItemListIndex], tf_LevelItem, main.m_Offset);
            itemMain.Init(this, main.m_ItemDirection);
        }
    }
    public void ShowPortal(Action OnPortalInteracted)
    {
        if (m_PortalIndex == -1)
            return;

        LevelTilePortal portal = m_AllTiles[m_PortalIndex] as LevelTilePortal;
        LevelItemBase portalItem = ObjectManager.SpawnLevelItem(m_AllItemPrefabs[ enum_LevelItemType.Portal][portal.m_LevelItemListIndex],tf_LevelItem,portal.m_Offset);
        portalItem.Init(this,portal.m_ItemDirection);
        portalItem.GetComponentInChildren<InteractPortal>().InitPortal(OnPortalInteracted);
    }

    void GenerateBorderTile(List<int> borderTiles)
    {
        List<int> sortedBorder = new List<int>();
        //Selected The First Direction
        sortedBorder.Add(borderTiles[0]);
        for (int i = 1; i < borderTiles.Count; i++)
        {
            if (m_AllTiles[borderTiles[i]].m_TileAxis.AxisOffset(m_AllTiles[ sortedBorder[0]].m_TileAxis) == 1)
            {
                sortedBorder.Add(borderTiles[i]);
                break;
            }
        }
        if (sortedBorder.Count != 2)
            Debug.LogError("Error Border Sort!");

        for (int i = 1; i < borderTiles.Count-1; i++)
        {
            borderTiles.Traversal((int tileIndex) => {
                if (sortedBorder.Contains(tileIndex))
                    return;

                if (m_AllTiles[sortedBorder[i]].m_TileAxis.AxisOffset(m_AllTiles[tileIndex].m_TileAxis) == 1)
                    sortedBorder.Add(tileIndex);
            });
        }

        for (int i = 0; i < sortedBorder.Count; i++)
        {
            LevelTile tile1 = m_AllTiles[ sortedBorder[(sortedBorder.Count+ i-1)%sortedBorder.Count]];
            LevelTile tile2 = m_AllTiles[sortedBorder[(i + 1) % sortedBorder.Count]];
            enum_TileDirection direction = tile1.m_TileAxis.OffsetDirection(tile2.m_TileAxis);
            m_AllTiles[sortedBorder[i]] = new LevelTileBorder(m_AllTiles[sortedBorder[i]], 0, TTiles.TTiles.m_FourDirections.Contains(direction) ? enum_LevelItemType.BorderLinear : enum_LevelItemType.BorderOblique ,direction);
        }
    }

    void GeneratePortalTile(bool showPortal,TileAxis center,int styledPrefabIndex)
    {
        if (!showPortal)
        {
            m_PortalIndex = -1;
            return;
        }
        LevelItemBase portalItem = m_AllItemPrefabs[enum_LevelItemType.Portal][styledPrefabIndex];
        TileAxis startAxis =center+ new TileAxis(-portalItem.m_sizeXAxis / 2, -portalItem.m_sizeYAxis / 2);
        m_PortalIndex = m_IndexEmptyInner.Find(p => m_AllTiles[p].m_TileAxis == startAxis);
        List<int> subIndexes = new List<int>();
        if (!CheckIndexTileAreaAvailable(m_PortalIndex, portalItem.m_sizeXAxis, portalItem.m_sizeYAxis, ref subIndexes))
            Debug.LogError("WTF?");
        LevelTilePortal portal = new LevelTilePortal(m_AllTiles[m_PortalIndex],subIndexes,styledPrefabIndex);
        m_AllTiles[m_PortalIndex] = portal;
        for (int i = 0; i < subIndexes.Count; i++)
            m_AllTiles[subIndexes[i]] = new LevelTileSub(m_AllTiles[subIndexes[i]], m_PortalIndex);
    }

    void GenerateRandomMainTile(SLevelGenerate generate,List<int> emptyTile)
    {
        TCommon.Traversal(generate.m_ItemGenerate, (enum_LevelItemType type, RangeInt range) =>
        {
            int totalCount = range.RandomRangeInt(m_seed);
            if (totalCount == 0)
                return;

            if (!m_AllItemPrefabs.ContainsKey(type))
            {
                Debug.LogError("Current Level Does Not Contains Item With Type:" + type.ToString()+",But Excel Has Values!");
                return;
            }

            List<LevelItemBase> targetItems = m_AllItemPrefabs[type];
            for (int i = 0; i < totalCount; i++)
            {
                int currentItemIndex = targetItems.RandomIndex(m_seed);
                LevelItemBase currentItem = targetItems[currentItemIndex];
                if (emptyTile.Count < currentItem.m_sizeXAxis * currentItem.m_sizeYAxis)
                    continue;
                enum_TileDirection itemAngleDirection = TTiles.TTiles.m_FourDirections.RandomItem(m_seed);
                int currentTileIndex = RandomAvailableTileIndex(currentItem.m_sizeXAxis, currentItem.m_sizeYAxis,  itemAngleDirection == enum_TileDirection.Left || itemAngleDirection == enum_TileDirection.Right, emptyTile, ref t_IndexTemp);
                if (currentTileIndex != -1)
                {
                    m_AllTiles[currentTileIndex] = new LevelTileItem(m_AllTiles[currentTileIndex], currentItemIndex, currentItem.m_ItemType, itemAngleDirection, t_IndexTemp);
                    m_IndexItemMain.Add(currentTileIndex);
                    emptyTile.Remove(currentTileIndex);
                    foreach (int subTileIndex in t_IndexTemp)
                    {
                        m_AllTiles[subTileIndex] = new LevelTileSub(m_AllTiles[subTileIndex], currentTileIndex);
                        emptyTile.Remove(subTileIndex);
                    }
                }
            }
        });
    }

    int RandomAvailableTileIndex(int XCount,int YCount,bool angleRotated,List<int> emptyTiles,ref List<int> areaIndexes)
    {
        if (XCount * YCount > emptyTiles.Count)
            return -1;
        int checkCount =  XCount * YCount*2;
        for (int i = 0; i < checkCount; i++)
        {
            int randomTileIndex = emptyTiles.RandomItem(m_seed);
            if (CheckIndexTileAreaAvailable(randomTileIndex, angleRotated ? YCount:XCount, angleRotated ? XCount: YCount, ref areaIndexes))
                    return randomTileIndex;
        }
        return -1;
    }
    bool CheckIndexTileAreaAvailable(int tileIndex, int XCount, int YCount, ref List<int> areaIndexes)
    {
        areaIndexes.Clear();
        LevelTile origin = m_AllTiles[tileIndex];

        if (origin.E_TileType != enum_LevelTileType.Empty)
            return false;

        for (int i = 0; i < XCount; i++)
        {
            for (int j = 0; j < YCount; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int index = m_AllTiles.FindIndex(p => p.m_TileAxis==origin.m_TileAxis+new TileAxis(i,j)&&p.E_TileType== enum_LevelTileType.Empty);
                if (index == -1)
                    return false;

                areaIndexes.Add(index);
            }
        }
        return true;
    }

    #endregion

    public Vector3 RandomEmptyTilePosition(System.Random seed, bool isInner = true) => transform.TransformPoint(m_AllTiles[isInner ? m_IndexEmptyInner.RandomItem(seed) : m_IndexEmptyOuter.RandomItem(seed)].m_Offset);
    public Vector3 OffsetToWorldPosition(Vector3 offset) => transform.TransformPoint(offset);
#if UNITY_EDITOR
#region Gizmos For Test
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_LevelDebugGizmos)
            return;

        for (int i = 0; i < m_AllTiles.Count; i++)
        {
            Color targetColor = Color.magenta;
            Vector3 positon = transform.position + transform.right * m_AllTiles[i].m_Offset.x + Vector3.up * m_AllTiles[i].m_Offset.y + transform.forward * m_AllTiles[i].m_Offset.z;
            float sizeParam = 1f;
            switch (GameManager.Instance.E_LevelDebug)
            {
                case GameManager.enumDebug_LevelDrawMode.DrawItemDirection:
                    {
                        sizeParam = 0f;
                        if (m_AllTiles[i].E_TileType == enum_LevelTileType.Main||m_AllTiles[i].E_TileType== enum_LevelTileType.Border)
                        {
                            sizeParam = 1f;
                            switch((m_AllTiles[i] as LevelTileItem).m_ItemDirection) 
                            {
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
                                case enum_TileDirection.TopLeft:
                                    targetColor = TCommon.ColorAlpha(Color.blue, 1f);
                                    break;
                                case enum_TileDirection.TopRight:
                                    targetColor = TCommon.ColorAlpha(Color.green, 1f);
                                    break;
                                case enum_TileDirection.BottomLeft:
                                    targetColor = TCommon.ColorAlpha(Color.red, 1f);
                                    break;
                                case enum_TileDirection.BottomRight:
                                    targetColor = TCommon.ColorAlpha(Color.yellow, 1f);
                                    break;
                            }
                        }
                    }
                    break;
                case GameManager.enumDebug_LevelDrawMode.DrawTypes:
                    {
                        switch (m_AllTiles[i].E_TileType)
                        {
                            case enum_LevelTileType.Empty:
                                targetColor = TCommon.ColorAlpha(Color.green, .3f);
                                break;
                            case enum_LevelTileType.Main:
                                targetColor = TCommon.ColorAlpha(Color.red, .5f); sizeParam = 1f;
                                break;
                            case enum_LevelTileType.Item:
                                targetColor = TCommon.ColorAlpha(Color.blue, .5f); sizeParam = 1f;
                                break;
                            case enum_LevelTileType.Portal:
                                targetColor = TCommon.ColorAlpha(Color.white, .5f); sizeParam = 1f;
                                break;
                            case enum_LevelTileType.Border:
                                targetColor = TCommon.ColorAlpha(Color.grey, .5f);
                                break;
                        }
                    }
                    break;
                case GameManager.enumDebug_LevelDrawMode.DrawOccupation:
                    {
                        switch (m_AllTiles[i].E_Occupation)
                        {
                            case enum_LevelTileOccupy.Inner:
                                targetColor = TCommon.ColorAlpha(Color.green,.3f);
                                break;
                            case enum_LevelTileOccupy.Outer:
                                targetColor = TCommon.ColorAlpha(Color.grey, .3f);
                                break;
                            case enum_LevelTileOccupy.Border:
                                targetColor = TCommon.ColorAlpha(Color.red, .3f);
                                break;
                        }
                    }
                    break;
            }
            Gizmos.color = targetColor;
            Gizmos.DrawSphere(positon, sizeParam * GameConst.F_LevelTileSize / 2);
        }
    }
    #endregion
#endif
}
