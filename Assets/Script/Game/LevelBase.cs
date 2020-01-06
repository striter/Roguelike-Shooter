
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using System;
using System.Collections;

public class LevelBase : MonoBehaviour{
    public List<LevelTile> m_MapData;
    public enum_LevelType m_levelType { get; private set; }
    protected Transform tf_Model;
    public Transform tf_LevelItem { get; private set; }
    public Transform tf_Interact { get; private set; }
    public System.Random m_seed { get; private set; }
    List<LevelTile> m_AllTiles = new List<LevelTile>();
    List<int> m_IndexEmtpyInteract = new List<int>();
    List<int> m_IndexBorder = new List<int>();
    Dictionary<bool, List<int>> m_IndexEmpty = new Dictionary<bool, List<int>>();
    Dictionary<bool, List<int>> m_IndexItemMain = new Dictionary<bool, List<int>>();
    List<int> t_IndexTemp = new List<int>();
    Dictionary<enum_LevelItemType, List<LevelItemBase>> m_AllItemPrefabs = new Dictionary<enum_LevelItemType, List<LevelItemBase>>();
    public float m_InnerBorder { get; private set; }
    public float m_TotalBorder { get; private set; }
    public void Init()
    {
        tf_LevelItem = transform.Find("Item");
        tf_Model = transform.Find("Model");
        tf_Interact = transform.Find("Interact");
        m_IndexEmpty.Add(true, new List<int>());
        m_IndexEmpty.Add(false, new List<int>());
        m_IndexItemMain.Add(true, new List<int>());
        m_IndexItemMain.Add(false, new List<int>());
    }

    #region TileMapInfos
    public void PrepareGenerateData(SLevelGenerate _innerData,SLevelGenerate _outerData, Dictionary<enum_LevelItemType, List<LevelItemBase>> allItemPrefabs, enum_LevelType _levelType, System.Random _seed)
    {
        m_AllItemPrefabs = allItemPrefabs;
        m_seed = _seed;
        m_levelType = _levelType;

        int _innerHalfRadius = _innerData.m_Length.Random(m_seed);
        int _outerHalfRadius = _outerData.m_Length.Random(m_seed);
        _outerHalfRadius = (_innerHalfRadius + _outerHalfRadius) % 2 == 0 ? _outerHalfRadius + 1 : _outerHalfRadius;
        int totalRadius = _innerHalfRadius + _outerHalfRadius;
        //Create Data
        int index = 0;
        TileAxis origin = new TileAxis(-totalRadius/2 , -totalRadius/2 );
        int borderRadius = _innerHalfRadius / 2;
        m_InnerBorder = borderRadius * 4 - 1.3f;
        m_TotalBorder = totalRadius * 2;
        for (int i = 0; i < totalRadius; i++)
        {
            for (int j = 0; j < totalRadius; j++)
            {
                TileAxis curTile = origin+new TileAxis(i, j);
                int curX = Mathf.Abs(curTile.X);
                int curY = Mathf.Abs(curTile.Y);
                enum_LevelItemTileOccupy occupation = enum_LevelItemTileOccupy.Invalid;
                if (curX<borderRadius&&curY<borderRadius)
                    occupation = enum_LevelItemTileOccupy.Inner;
                else if (curX > borderRadius || curY > borderRadius)
                    occupation = enum_LevelItemTileOccupy.Outer;
                else
                    occupation = enum_LevelItemTileOccupy.Border;

                m_AllTiles.Add(new LevelTile(curTile, occupation));
                switch (occupation)
                {
                    case enum_LevelItemTileOccupy.Inner:
                        m_IndexEmpty[true].Add(index);
                        break;
                    case enum_LevelItemTileOccupy.Outer:
                        m_IndexEmpty[false].Add(index);
                        break;
                    case enum_LevelItemTileOccupy.Border:
                        m_IndexBorder.Add(index);
                        break;
                }
                index++;
            }
        }
        ClearTileForInteracts();
        GenerateBorderTile(m_IndexBorder);
        GenerateRandomMainTile(_innerData,m_IndexEmpty[true],true);
        GenerateRandomMainTile(_outerData, m_IndexEmpty[false],false);

        m_IndexItemMain[false].AddRange(m_IndexBorder);

    }
    public void Generate( Action OnInnerItemDestroyed)
    {
        tf_Model.localScale = Vector3.one * m_TotalBorder;
        m_IndexItemMain.Traversal((bool isInner, List<int> indexes) => {
            indexes.Traversal((int mainIndex) => {
                LevelTileItem main = m_AllTiles[mainIndex] as LevelTileItem;
                LevelItemBase itemMain = GameObjectManager.SpawnLevelItem(m_AllItemPrefabs[main.m_LevelItemType][main.m_LevelItemListIndex], tf_LevelItem, main.m_Offset);
                itemMain.InitItem(this, main.m_ItemDirection, isInner ? OnInnerItemDestroyed : null);
            });
        });
    }

    void ClearTileForInteracts()        //Clear 3x3 For Interacts
    {
        TileAxis startAxis = TileAxis.Zero + new TileAxis(-3 / 2, -3 / 2);
        int centerIndex = m_IndexEmpty[true].Find(p => m_AllTiles[p].m_TileAxis == startAxis);
        List<int> subIndexes = new List<int>();
        if (!CheckIndexTileAreaAvailable(centerIndex, 3, 3, ref subIndexes))
            Debug.LogError("WTF?");

        m_AllTiles[centerIndex] = new LevelTileInteract(m_AllTiles[centerIndex]);
        m_IndexEmpty[true].Remove(centerIndex);
        m_IndexEmtpyInteract.Add(centerIndex);
        for (int i = 0; i < subIndexes.Count; i++)
        {
            m_AllTiles[subIndexes[i]] = new LevelTileInteract(m_AllTiles[subIndexes[i]]);
            m_IndexEmpty[true].Remove(subIndexes[i]);
            m_IndexEmtpyInteract.Add(subIndexes[i]);
        }
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
            m_AllTiles[sortedBorder[i]] = new LevelTileBorder(m_AllTiles[sortedBorder[i]], 0, TTiles.TileTools.m_FourDirections.Contains(direction) ? enum_LevelItemType.BorderLinear : enum_LevelItemType.BorderOblique ,direction);
        }
    }

    void GenerateRandomMainTile(SLevelGenerate generate, List<int> emptyTile,bool isInner)
    {
        TCommon.Traversal(generate.m_ItemGenerate, (enum_LevelItemType type, RangeInt range) =>
        {
            int totalCount = range.Random(m_seed);
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
                enum_TileDirection itemAngleDirection = TTiles.TileTools.m_FourDirections.RandomItem(m_seed);
                int currentTileIndex = RandomAvailableTileIndex(currentItem.m_sizeXAxis, currentItem.m_sizeYAxis,  itemAngleDirection == enum_TileDirection.Left || itemAngleDirection == enum_TileDirection.Right, emptyTile, ref t_IndexTemp);
                if (currentTileIndex != -1)
                {
                    m_AllTiles[currentTileIndex] = new LevelTileItem(m_AllTiles[currentTileIndex], currentItemIndex, currentItem.m_ItemType, itemAngleDirection, t_IndexTemp);
                    m_IndexItemMain[isInner].Add(currentTileIndex);
                    emptyTile.Remove(currentTileIndex);
                    foreach (int subTileIndex in t_IndexTemp)
                    {
                        m_AllTiles[subTileIndex] = new LevelTileSub(m_AllTiles[subTileIndex], currentTileIndex);
                        emptyTile.Remove(subTileIndex);
                    }
                }
            }
        });

        if (emptyTile.Count == 0)
            Debug.LogWarning("Level Generate Warning! Tile All Filled!");
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

        if (origin.E_TileType != enum_LevelItemTileType.Empty)
            return false;

        for (int i = 0; i < XCount; i++)
        {
            for (int j = 0; j < YCount; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int index = m_AllTiles.FindIndex(p => p.m_TileAxis==origin.m_TileAxis+new TileAxis(i,j)&&p.E_TileType== enum_LevelItemTileType.Empty);
                if (index == -1)
                    return false;

                areaIndexes.Add(index);
            }
        }
        return true;
    }

    #endregion

    public Vector3 OffsetToWorldPosition(Vector3 offset) => transform.TransformPoint(offset);
    public Vector3 RandomInnerEmptyTilePosition(System.Random seed, bool occupy = true)
    {
        int emptyIndex = m_IndexEmpty[true].RandomItem(seed);
        LevelTile tile = m_AllTiles[emptyIndex];
        if(occupy)
        {
            m_IndexEmpty[true].Remove(emptyIndex);
            m_AllTiles[emptyIndex] = new LevelTileInteract(tile);
        }
        return OffsetToWorldPosition(tile.m_Offset);
    }
    public Vector3 NearestInteractTilePosition(TileAxis axis,System.Random seed,bool occupy=true)
    {
        int targetTile = -1;
        int closestOffset=int.MaxValue;
        m_IndexEmtpyInteract.TraversalRandomBreak((int interactIndex) =>
        {

            TileAxis _axis= m_AllTiles[interactIndex].m_TileAxis;
            int offset = axis.AxisOffset(_axis);
            if (closestOffset>offset)
                targetTile = interactIndex;
            return offset == 0;
        },seed);
        if (targetTile > 0)
        {
            if(occupy)
                m_IndexEmtpyInteract.Remove(targetTile);
            return OffsetToWorldPosition(m_AllTiles[targetTile].m_Offset);
        }
        return Vector3.zero;
    }
#if UNITY_EDITOR
    #region Gizmos For Test
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_GameLevelDebugGizmos)
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
                        if (m_AllTiles[i].E_TileType == enum_LevelItemTileType.Main||m_AllTiles[i].E_TileType== enum_LevelItemTileType.Border)
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
                            }
                        }
                    }
                    break;
                case GameManager.enumDebug_LevelDrawMode.DrawTypes:
                    {
                        switch (m_AllTiles[i].E_TileType)
                        {
                            case enum_LevelItemTileType.Empty:
                                targetColor = TCommon.ColorAlpha(Color.green, .3f);
                                break;
                            case enum_LevelItemTileType.Main:
                                targetColor = TCommon.ColorAlpha(Color.red, .5f); sizeParam = 1f;
                                break;
                            case enum_LevelItemTileType.Item:
                                targetColor = TCommon.ColorAlpha(Color.blue, .5f); sizeParam = 1f;
                                break;
                            case enum_LevelItemTileType.Interact:
                                targetColor = TCommon.ColorAlpha(Color.white, .5f); sizeParam = 1f;
                                break;
                            case enum_LevelItemTileType.Border:
                                targetColor = TCommon.ColorAlpha(Color.grey, .5f);
                                break;
                        }
                    }
                    break;
                case GameManager.enumDebug_LevelDrawMode.DrawOccupation:
                    {
                        switch (m_AllTiles[i].E_Occupation)
                        {
                            case enum_LevelItemTileOccupy.Inner:
                                targetColor = TCommon.ColorAlpha(Color.green,.3f);
                                break;
                            case enum_LevelItemTileOccupy.Outer:
                                targetColor = TCommon.ColorAlpha(Color.grey, .3f);
                                break;
                            case enum_LevelItemTileOccupy.Border:
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
