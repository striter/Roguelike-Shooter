using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System.Linq;
public class LevelBase : MonoBehaviour {
    
    public TileMapData data { get; private set; }
    public int I_CellWidthCount = 10, I_CellHeightCount = 10;
    public bool B_IgnoreUnavailable=true;
    public enum_LevelType m_LevelType = enum_LevelType.Forest;
    protected Transform tf_LevelStatic;

    public void Init(enum_LevelType level,TileMapData _data)
    {
        tf_LevelStatic = transform.Find("LevelStatic");
        m_LevelType = level;
        data = _data;
        GenerateStatics();
    }

    void GenerateStatics()
    {
        Dictionary<enum_LevelItemCollide, List<LevelStaticBase>> m_statics = new Dictionary<enum_LevelItemCollide, List<LevelStaticBase>>();
        LevelStaticBase[] allStatics = TResources.LoadAll<LevelStaticBase>("Level/Static/"+m_LevelType);
        foreach (LevelStaticBase staticbase in allStatics)
        {
            if (staticbase.m_GenerateOrder== enum_LevelItemCollide.Invalid)
                Debug.LogError("Please Edit Static Item(Something invalid): Resources/Level/Static/"+ m_LevelType + "/"+staticbase.name);

            if (!m_statics.ContainsKey(staticbase.m_GenerateOrder))
                m_statics.Add(staticbase.m_GenerateOrder, new List<LevelStaticBase>());
            m_statics[staticbase.m_GenerateOrder].Add(staticbase);
        }

        //Select All Emptys And Create Data
        List<LevelTile> tiles = new List<LevelTile>();
        for (int i = 0; i < data.m_MapData.Count; i++)
        {
            if (data.m_MapData[i].m_Status != -1)
            {
                tiles.Add(data.m_MapData[i] as LevelTile);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            int randomIndex = EnviormentManager.m_randomSeed.Next(tiles.Count);
            int randomItem = EnviormentManager.m_randomSeed.Next(m_statics[enum_LevelItemCollide.NoCollide].Count);
            LevelStaticBase staticbase= Instantiate(m_statics[enum_LevelItemCollide.NoCollide][randomItem],tf_LevelStatic);
            staticbase.transform.localPosition = tiles[randomIndex].m_Offset;
        }
        
        m_statics.Clear();
    }

    enum enum_NodeStatus
    {
        Invaid=-1,
        Empty=0,
        Occupied=1,
    }
    class LevelTile : TileMapData.TileInfo
    {
        public enum_NodeStatus E_NodeStatus => (enum_NodeStatus)m_Status;
        public LevelTile(TileMapData.TileInfo main):base(main.i_axisX,main.i_axisY,main.m_Offset,main.m_Status)
        {
        }
    }

#if UNITY_EDITOR
    public bool b_showGizmos;
    private void OnDrawGizmos()
    {
        if (data == null && m_LevelType != enum_LevelType.Invalid)
            data = EnviormentManager.GetLevelData(m_LevelType, gameObject.name);

        if (data == null)
            Debug.LogWarning("Please Bake This Level First");
        if (b_showGizmos)
        {
            DrawGizmos();
        }
    }
    void DrawGizmos()
    {
        if (data == null || data.m_MapData == null)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < I_CellWidthCount; i++)
            {
                for (int j = 0; j < I_CellHeightCount; j++)
                {
                    Vector3 position = transform.position 
                        + transform.forward* GameConst.F_LevelTileSize * j
                        +transform.right* GameConst.F_LevelTileSize * i 
                        -transform.right*(GameConst.F_LevelTileSize * I_CellWidthCount / 2 - GameConst.F_LevelTileSize / 2)
                        -transform.forward*(GameConst.F_LevelTileSize * I_CellHeightCount / 2 - GameConst.F_LevelTileSize / 2);
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
                Gizmos.DrawCube(transform.position + transform.right*node.m_Offset.x+Vector3.up * (node.m_Offset.y + .5f)+ transform.forward * node.m_Offset.z, new Vector3(GameConst.F_LevelTileSize / 2, 1f, GameConst.F_LevelTileSize / 2));
            }
        }
    }
#endif
}
