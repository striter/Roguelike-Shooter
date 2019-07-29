
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
public class LevelBase : MonoBehaviour {
    public enum_LevelType E_PrefabType = enum_LevelType.Invalid;
    public List<LevelTile> m_MapData;
    public enum_TileType m_levelType { get; private set; }
    protected Transform tf_LevelItem;
    public System.Random m_seed { get; private set; }
    public Dictionary<LevelItemBase, int> Init(int innerLength,int outerLength,SGenerateItem _levelGenerate, LevelItemBase[] _levelItems,enum_TileType _levelType, System.Random _seed, enum_TileDirection _connectedDireciton)
    {
        tf_LevelItem = transform.Find("Item");
        m_seed = _seed;
        m_levelType = _levelType;
        return GenerateTileItems(_levelGenerate,_levelItems, innerLength,outerLength, _connectedDireciton);
    }

    #region TileMapInfos
    public LevelTilePortal m_Portal;
    List<LevelTile> m_AllTiles=new List<LevelTile>();
    List<int> m_IndexEmpty=new List<int>();
    List<int> m_IndexMain=new List<int>();
    List<int> t_IndexTemp = new List<int>();
    Dictionary<enum_LevelItemType, List<LevelItemBase>> m_AllItems = new Dictionary<enum_LevelItemType, List<LevelItemBase>>();
    Dictionary<LevelItemBase,int> GenerateTileItems(SGenerateItem _itemData,LevelItemBase[] allItemPrefabs, int innerLength,int outerLength,enum_TileDirection _PortalDirection)
    {
        foreach (LevelItemBase levelItem in allItemPrefabs)
        {
            if (!m_AllItems.ContainsKey(levelItem.m_ItemType))
                m_AllItems.Add(levelItem.m_ItemType, new List<LevelItemBase>());
            m_AllItems[levelItem.m_ItemType].Add(levelItem);
        }

        //Select All Emptys And Create Data
        int index = 0;
        TileAxis origin = new TileAxis(-innerLength / 2, -innerLength / 2);
        for (int i = 0; i < innerLength; i++)
        {
            for (int j = 0; j < innerLength; j++)
            {
                TileAxis curTile = origin+new TileAxis(i, j);
                m_AllTiles.Add(new LevelTile(curTile, GameExpression.E_BigmapDirection(transform.TransformDirection(GameExpression.V3_TileWorldOffset(curTile)))));
                m_IndexEmpty.Add(index++);
            }
        }

        //Generate Portal Pos
        GenerateRangePortalTile(origin,_PortalDirection,GameConst.I_TileMapPortalMinusOffset);

        //Generate All Items
        TCommon.Traversal(_itemData.m_ItemGenerate, (enum_LevelItemType type, RangeInt range) =>
        {
            GenerateRandomItemTile(type, range.RandomRangeInt(m_seed));
        });

        Dictionary<LevelItemBase, int> itemCountDic = new Dictionary<LevelItemBase, int>();
        for (int i = 0; i < m_IndexMain.Count; i++)
        {
            LevelTileItemMain main = m_AllTiles[m_IndexMain[i]] as LevelTileItemMain;
            LevelItemBase item = m_AllItems[main.m_LevelItemType][main.m_LevelItemListIndex];
            if (!itemCountDic.ContainsKey(item))
                itemCountDic.Add(item,0);
            itemCountDic[item]++;
        }
        return itemCountDic;
    }
    public void ShowAllItems()
    {
        for (int i = 0; i < m_IndexMain.Count; i++)
        {
            LevelTileItemMain main = m_AllTiles[m_IndexMain[i]] as LevelTileItemMain;
            LevelItemBase itemMain = ObjectManager.SpawnLevelItem (m_AllItems[main.m_LevelItemType][main.m_LevelItemListIndex], tf_LevelItem, main.m_Offset);
            itemMain.Init(this, main.m_ItemDirection);
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
            enum_TileDirection itemAngleDirection = TTiles.TTiles.m_AllDirections.RandomItem(m_seed);
            int currentTileIndex = RandomAvailableTileIndex(currentItem.m_sizeXAxis,currentItem.m_sizeYAxis, itemAngleDirection== enum_TileDirection.Left||itemAngleDirection== enum_TileDirection.Right, ref t_IndexTemp);
            if (currentTileIndex != -1)
            {
                m_AllTiles[currentTileIndex] = new LevelTileItemMain(m_AllTiles[currentTileIndex],currentItemIndex, currentItem.m_ItemType, itemAngleDirection, t_IndexTemp);
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

    void GenerateRangePortalTile(TileAxis origin, enum_TileDirection portalDirection,int minusRange)
    {
            List<LevelTile> tileAvailable = m_AllTiles.FindAll(p => p.E_Direction == portalDirection && p.E_TileType== enum_LevelTileType.Empty);
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
            LevelTilePortal portal= new LevelTilePortal(m_AllTiles[index], portalDirection, transform.TransformPoint(m_AllTiles[index].m_Offset));
            m_AllTiles[index] = portal;
        m_Portal = portal;
    }
    int RandomAvailableTileIndex(int XCount,int YCount,bool angleRotated,ref List<int> areaIndexes)
    {
        if (XCount * YCount > m_IndexEmpty.Count)
            return -1;
        int checkCount =  XCount * YCount*2;
        for (int i = 0; i < checkCount; i++)
        {
            int randomTileIndex =    m_IndexEmpty.RandomItem(m_seed);
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

    public Vector3 RandomEmptyTilePosition(System.Random seed)
    {
        return transform.TransformPoint(m_AllTiles[m_IndexEmpty.RandomItem(seed)].m_Offset );
    }
    #endregion

    #region Gizmos For Test
#if UNITY_EDITOR
    public bool b_showWorldDirection=false;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_GameDebugGizmos)
            return;

        for (int i = 0; i < m_AllTiles.Count; i++)
        {
            Color targetColor = Color.black;
            Vector3 positon = transform.position + transform.right * m_AllTiles[i].m_Offset.x + Vector3.up * m_AllTiles[i].m_Offset.y + transform.forward * m_AllTiles[i].m_Offset.z;
            float sizeParam = .5f;
            if (b_showWorldDirection)
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
                if (m_AllTiles[i].E_TileType == enum_LevelTileType.Portal)
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
                }
            }
            Gizmos.color = targetColor;
            Gizmos.DrawSphere(positon, sizeParam * GameConst.F_LevelTileSize / 2);
        }


    }
#endif
    #endregion
}
