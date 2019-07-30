﻿
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
public class LevelBase : MonoBehaviour {
    public List<LevelTile> m_MapData;
    public enum_TileType m_levelType { get; private set; }
    protected Transform tf_LevelItem,tf_Model;
    public System.Random m_seed { get; private set; }
    public Dictionary<LevelItemBase, int> Init(SLevelGenerate _innerData,SLevelGenerate _outerData, LevelItemBase[] _levelItems,enum_TileType _levelType, System.Random _seed, enum_TileDirection _connectedDireciton)
    {
        tf_LevelItem = transform.Find("Item");
        tf_Model = transform.Find("Model");
        m_seed = _seed;
        m_levelType = _levelType;
        return GenerateTileItems(_innerData,_outerData,_levelItems, _connectedDireciton);
    }

    #region TileMapInfos
    public LevelTilePortal m_Portal;
    List<LevelTile> m_AllTiles=new List<LevelTile>();
    List<int> m_IndexEmptyInner = new List<int>();
    List<int> m_IndexEmptyOuter = new List<int>();
    List<int> m_IndexBorder = new List<int>();
    List<int> m_IndexItemMain=new List<int>();
    List<int> t_IndexTemp = new List<int>();
    Dictionary<enum_LevelItemType, List<LevelItemBase>> m_AllItems = new Dictionary<enum_LevelItemType, List<LevelItemBase>>();
    Dictionary<LevelItemBase,int> GenerateTileItems(SLevelGenerate _innerData,SLevelGenerate _outerData,LevelItemBase[] allItemPrefabs,enum_TileDirection _PortalDirection)
    {

        foreach (LevelItemBase levelItem in allItemPrefabs)
        {
            if (!m_AllItems.ContainsKey(levelItem.m_ItemType))
                m_AllItems.Add(levelItem.m_ItemType, new List<LevelItemBase>());
            m_AllItems[levelItem.m_ItemType].Add(levelItem);
        }

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

                m_AllTiles.Add(new LevelTile(curTile, GameExpression.E_BigMapFourDirection(transform.TransformDirection(GameExpression.V3_TileAxisOffset(curTile))),occupation));
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
        GenerateBorderTile(m_IndexBorder);
        GenerateRangePortalTile(origin, _PortalDirection, GameConst.I_TileMapPortalMinusOffset);
        GenerateRandomMainTile(_innerData,m_IndexEmptyInner);
        GenerateRandomMainTile(_outerData, m_IndexEmptyOuter);

        m_IndexItemMain.AddRange(m_IndexBorder);
        Dictionary<LevelItemBase, int> itemCountDic = new Dictionary<LevelItemBase, int>();
        for (int i = 0; i < m_IndexItemMain.Count; i++)
        {
            LevelTileItemMain main = m_AllTiles[m_IndexItemMain[i]] as LevelTileItemMain;
            LevelItemBase item = m_AllItems[main.m_LevelItemType][main.m_LevelItemListIndex];
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
            LevelTileItemMain main = m_AllTiles[m_IndexItemMain[i]] as LevelTileItemMain;
            LevelItemBase itemMain = ObjectManager.SpawnLevelItem (m_AllItems[main.m_LevelItemType][main.m_LevelItemListIndex], tf_LevelItem, main.m_Offset);
            itemMain.Init(this, main.m_ItemDirection);
        }
    }

    void GenerateBorderTile(List<int> borderTiles)
    {
        borderTiles.Traversal((int tileIndex) => {
            LevelTile borderTile = m_AllTiles[tileIndex];
            List<TileAxis> nearbyTiles=new List<TileAxis>();
            borderTiles.Traversal((int nearbyIndex)=> {
                if (nearbyIndex == tileIndex)
                    return;
                LevelTile nearbyTile = m_AllTiles[nearbyIndex];
                if (nearbyTile.m_TileAxis.AxisOffset(borderTile.m_TileAxis) == 1)
                    nearbyTiles.Add(nearbyTile.m_TileAxis);
            });

            if (nearbyTiles.Count == 2)
            {
                enum_TileDirection direction1 = nearbyTiles[0].OffsetDirection(nearbyTiles[1]);
                enum_TileDirection direction2 = TileAxis.Zero.OffsetDirection(borderTile.m_TileAxis);
                m_AllTiles[tileIndex] = new LevelTileBorder(m_AllTiles[tileIndex], 0, enum_LevelItemType.BorderBlock,TTiles.TTiles.m_FourDirections.Contains(direction1)?direction1:direction2);
            }
            else
                borderTiles.Remove(tileIndex);
        });
    }

    void GenerateRangePortalTile(TileAxis origin, enum_TileDirection portalDirection, int minusRange)
    {
        List<LevelTile> tileAvailable = m_AllTiles.FindAll(p => p.E_WorldDireciton == portalDirection && p.E_TileType == enum_LevelTileType.Empty);
        LevelTile closestTile = tileAvailable[0];
        int closestOffset = int.MaxValue;
        for (int j = 0; j < tileAvailable.Count; j++)
        {
            int offset = origin.AxisOffset(tileAvailable[j].m_TileAxis);
            if (offset > minusRange && closestOffset > offset)
            {
                closestTile = tileAvailable[j];
                closestOffset = offset;
            }
        }
        int index = m_AllTiles.IndexOf(closestTile);
        LevelTilePortal portal = new LevelTilePortal(m_AllTiles[index], portalDirection, transform.TransformPoint(m_AllTiles[index].m_Offset));
        m_AllTiles[index] = portal;
        m_Portal = portal;
    }

    void GenerateRandomMainTile(SLevelGenerate generate,List<int> emptyTile)
    {
        TCommon.Traversal(generate.m_ItemGenerate, (enum_LevelItemType type, RangeInt range) =>
        {
            int totalCount = range.RandomRangeInt(m_seed);
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
                if (emptyTile.Count < currentItem.m_sizeXAxis * currentItem.m_sizeYAxis)
                    continue;
                enum_TileDirection itemAngleDirection = TTiles.TTiles.m_FourDirections.RandomItem(m_seed);
                int currentTileIndex = RandomAvailableTileIndex(currentItem.m_sizeXAxis, currentItem.m_sizeYAxis,  itemAngleDirection == enum_TileDirection.Left || itemAngleDirection == enum_TileDirection.Right, emptyTile, ref t_IndexTemp);
                if (currentTileIndex != -1)
                {
                    m_AllTiles[currentTileIndex] = new LevelTileItemMain(m_AllTiles[currentTileIndex], currentItemIndex, currentItem.m_ItemType, itemAngleDirection, t_IndexTemp);
                    m_IndexItemMain.Add(currentTileIndex);
                    emptyTile.Remove(currentTileIndex);
                    foreach (int subTileIndex in t_IndexTemp)
                    {
                        m_AllTiles[subTileIndex] = new LevelTileItemSub(m_AllTiles[subTileIndex], currentTileIndex);
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
                case GameManager.enumDebug_LevelDrawMode.DrawWorldDirection:
                    {
                        sizeParam = .5f;
                        switch (m_AllTiles[i].E_WorldDireciton)
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
                        }
                        if (m_AllTiles[i].E_TileType == enum_LevelTileType.Portal)
                        {
                            sizeParam = 1.5f;
                        }
                    }
                    break;
                case GameManager.enumDebug_LevelDrawMode.DrawItemDirection:
                    {
                        sizeParam = 0f;
                        if (m_AllTiles[i].E_TileType == enum_LevelTileType.Main||m_AllTiles[i].E_TileType== enum_LevelTileType.Border)
                        {
                            sizeParam = 1f;
                            switch((m_AllTiles[i] as LevelTileItemMain).m_ItemDirection) 
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
                                    targetColor = TCommon.ColorAlpha(Color.yellow, 1f);
                                    break;
                                case enum_TileDirection.BottomLeft:
                                    targetColor = TCommon.ColorAlpha(Color.red, 1f);
                                    break;
                                case enum_TileDirection.BottomRight:
                                    targetColor = TCommon.ColorAlpha(Color.green, 1f);
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
