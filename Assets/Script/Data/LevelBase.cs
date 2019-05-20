
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class LevelBase : MonoBehaviour {
    
    public int I_CellWidthCount = 64, I_CellHeightCount = 64;
    public bool B_IgnoreUnavailable=true;
    public float F_HeightDetect = .5f;
    public enum_LevelType m_LevelType = enum_LevelType.Invalid;
    protected Transform tf_LevelItem;
   
    public void Init(SLevelGenerate _itemData,TileMapData _data)
    {
        tf_LevelItem = transform.Find("Item");
        m_LevelType = _itemData.m_LevelType;
        GenerateTileItems(_itemData,_data);
    }
    #region TileMapInfos
    List<LevelTile> m_AllTiles=new List<LevelTile>();
    List<int> m_IndexEmpty=new List<int>();
    List<int> m_IndexMain=new List<int>();
    List<int> t_IndexTemp = new List<int>();
    Dictionary<enum_LevelItemType, List<LevelItemBase>> m_AllItems = new Dictionary<enum_LevelItemType, List<LevelItemBase>>();
    void GenerateTileItems(SLevelGenerate _itemData, TileMapData _data)
    {
        //Load All Level Item Info
        LevelItemBase[] allItems = TResources.LoadAll<LevelItemBase>("Level/Item/" + m_LevelType);
        foreach (LevelItemBase levelItem in allItems)
        {
            if (levelItem.m_ItemType == enum_LevelItemType.Invalid)
                Debug.LogError("Please Edit Static Item(Something invalid): Resources/Level/Item/" + m_LevelType + "/" + levelItem.name);

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
                m_AllTiles.Add(new LevelTile(_data.m_MapData[i]));
                m_IndexEmpty.Add(index++);
            }
        }

        TCommon.TraversalDic(_itemData.m_ItemGenerate, (enum_LevelItemType type, RangeInt range) =>
        {
            GenerateRandomItem(type,EnviormentManager.m_randomSeed.Next(range.start,range.end+1));
        });
        
        for (int i = 0; i < m_IndexMain.Count; i++)
        {
            LevelTileMain main = m_AllTiles[m_IndexMain[i]] as LevelTileMain;
            LevelItemBase itemMain=   GameObject.Instantiate(m_AllItems[main.m_LevelItemType][main.m_LevelItemListIndex],tf_LevelItem);
            itemMain.transform.localPosition = main.m_Offset;
            itemMain.Init();
        }
    }

    void GenerateRandomItem(enum_LevelItemType type, int totalCount)
    {
        List<LevelItemBase> targetItems = m_AllItems[type];
        for (int i = 0; i < totalCount; i++)
        {
            int currentItemIndex = targetItems.ListRandomIndex(EnviormentManager.m_randomSeed);
            LevelItemBase currentItem = targetItems[currentItemIndex];
            int currentTileIndex = RandomAvailableTileIndex(currentItem.m_sizeXAxis,currentItem.m_sizeYAxis, ref t_IndexTemp);
            if (currentTileIndex != -1)
            {
                m_AllTiles[currentTileIndex] = new LevelTileMain(m_AllTiles[currentTileIndex],currentItemIndex,currentItem.m_ItemType, t_IndexTemp);
                m_IndexMain.Add(currentTileIndex);
                m_IndexEmpty.Remove(currentTileIndex);
                foreach (int subTileIndex in t_IndexTemp)
                {
                    m_AllTiles[subTileIndex] = new LevelTileSub(m_AllTiles[subTileIndex],currentTileIndex);
                    m_IndexEmpty.Remove(subTileIndex);
                }
            }
        }
    }
    int RandomAvailableTileIndex(int XCount,int YCount,ref List<int> areaIndexes)
    {
        int checkCount = XCount * YCount + 1;
        for (int i = 0; i < checkCount; i++)
        {
            int randomTileIndex =    m_IndexEmpty.ListRandom(EnviormentManager.m_randomSeed);
            if (CheckIndexTileAreaAvailable(randomTileIndex, XCount, YCount, ref areaIndexes))
                return randomTileIndex;
        }
        return -1;
    }
    bool CheckIndexTileAreaAvailable(int tileIndex, int XCount, int YCount, ref List<int> areaIndexes)
    {
        areaIndexes.Clear();
        LevelTile origin = m_AllTiles[tileIndex];
        for (int i = 0; i < XCount; i++)
        {
            for (int j = 0; j < YCount; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int index = m_AllTiles.FindIndex(p => p.i_axisX == origin.i_axisX + i && p.i_axisY == origin.i_axisY+j&&p.E_TileType== enum_TileType.Empty);
                if (index == -1)
                    return false;

                areaIndexes.Add(index);
            }
        }
        return true;
    }
    #endregion
    #region TileMapClass
    enum enum_TileType
    {
        Invaid=-1,
        Empty=0,
        Main,
        Sub,
    }
    class LevelTileSub : LevelTile
    {
        public override enum_TileType E_TileType => enum_TileType.Sub;
        public int m_ParentMainIndex { get; private set; }
        public LevelTileSub(TileMapData.TileInfo current, int _parentMainIndex) : base(current)
        {
            m_ParentMainIndex = _parentMainIndex;
        }
    }
    class LevelTileMain : LevelTile
    {
        public override enum_TileType E_TileType => enum_TileType.Main;
        public int m_LevelItemListIndex { get; private set; }
        public enum_LevelItemType m_LevelItemType { get; private set; }
        public List<int> m_AreaTiles { get; private set; }
        public LevelTileMain(TileMapData.TileInfo current,int levelItemListIndex,enum_LevelItemType levelItemType, List<int> _AreaTiles) : base(current)
        {
            m_LevelItemListIndex = levelItemListIndex;
            m_LevelItemType = levelItemType;
            m_AreaTiles = _AreaTiles;
        }
    }
    class LevelTile : TileMapData.TileInfo
    {
        public virtual enum_TileType E_TileType => enum_TileType.Empty;
        public LevelTile(TileMapData.TileInfo current):base(current.i_axisX,current.i_axisY,current.m_Offset,current.m_Status)
        {
        }
    }
    #endregion
#if UNITY_EDITOR
    public bool b_showGizmos=true;
    public TileMapData data;
    private void OnDrawGizmos()
    {
        if (!b_showGizmos)
            return;

        if (UnityEditor.EditorApplication.isPlaying)            //Draw While Playing
        {
            for (int i = 0; i < m_AllTiles.Count; i++)
            {
                Vector3 positon = transform.position + transform.right* m_AllTiles[i].m_Offset.x + Vector3.up * m_AllTiles[i].m_Offset.y + transform.forward*m_AllTiles[i].m_Offset.z;
                Color targetColor;
                float sizeParam = .5f;
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
                    case enum_TileType.Sub:
                        targetColor = TCommon.ColorAlpha(Color.blue, .5f); sizeParam = 1f;
                        break;
                }
                Gizmos.color = targetColor;
                Gizmos.DrawSphere(positon,sizeParam*GameConst.F_LevelTileSize/2);
            }
            return;
        }

        if (data == null && m_LevelType != enum_LevelType.Invalid)
            data = EnviormentManager.GetLevelData(m_LevelType, gameObject.name);

        if (data == null)
            Debug.LogWarning("Please Bake This Level First");

        if (data == null || data.m_MapData == null)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < I_CellWidthCount; i++)
            {
                for (int j = 0; j < I_CellHeightCount; j++)
                {
                    Vector3 position = transform.position
                        + transform.forward * GameConst.F_LevelTileSize * j
                        + transform.right * GameConst.F_LevelTileSize * i
                        - transform.right * (GameConst.F_LevelTileSize * I_CellWidthCount / 2 - GameConst.F_LevelTileSize / 2)
                        - transform.forward * (GameConst.F_LevelTileSize * I_CellHeightCount / 2 - GameConst.F_LevelTileSize / 2);
                    Gizmos.DrawCube(position, new Vector3(GameConst.F_LevelTileSize / 2, 5f, GameConst.F_LevelTileSize / 2));
                }
            }
        }
        else
        {
            I_CellWidthCount = data.I_Width;
            I_CellHeightCount = data.I_Height;
            List<TileMapData.TileInfo> nodes = data.m_MapData;
            foreach (TileMapData.TileInfo node in nodes)
            {
                Gizmos.color = node.m_Status == -1 ? Color.red : Color.green;
                Gizmos.DrawCube(transform.position + transform.right * node.m_Offset.x + Vector3.up * (node.m_Offset.y + .5f) + transform.forward * node.m_Offset.z, new Vector3(GameConst.F_LevelTileSize / 2, 1f, GameConst.F_LevelTileSize / 2));
            }
        }
    }
#endif
}
