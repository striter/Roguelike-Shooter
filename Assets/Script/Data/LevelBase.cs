using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System.Linq;
public class LevelBase : MonoBehaviour {
    
    public TileMapData data { get; private set; }
    public int CellWidthCount = 10, CellHeightCount = 10;
    public Vector2 CellOffset = Vector2.one;
    public bool b_bakeUnavailable=true;
    public enum_LevelType m_LevelType = enum_LevelType.Island;
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
        Dictionary<enum_GenerateOrder, List<LevelStaticBase>> m_statics = new Dictionary<enum_GenerateOrder, List<LevelStaticBase>>();
        LevelStaticBase[] allStatics = TResources.LoadAll<LevelStaticBase>("Level/Static/"+m_LevelType);
        foreach (LevelStaticBase staticbase in allStatics)
        {
            if (staticbase.m_GenerateOrder== enum_GenerateOrder.Invalid)
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
            int randomItem = EnviormentManager.m_randomSeed.Next(m_statics[enum_GenerateOrder.First].Count);
            LevelStaticBase staticbase= Instantiate(m_statics[enum_GenerateOrder.First][randomItem],tf_LevelStatic);
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
            for (int i = 0; i < CellWidthCount; i++)
            {
                for (int j = 0; j < CellHeightCount; j++)
                {
                    Vector3 position = transform.position 
                        + transform.forward* CellOffset.y * j
                        +transform.right* CellOffset.x * i 
                        -transform.right*(CellOffset.x * CellWidthCount / 2 - CellOffset.x / 2)
                        -transform.forward*( CellOffset.y * CellHeightCount / 2 - CellOffset.y / 2);
                    Gizmos.DrawCube(position, new Vector3(CellOffset.x / 2, 5f, CellOffset.y / 2));
                }
            }
        }
        else
        {
            CellWidthCount = data.I_Width;
            CellHeightCount = data.I_Height;
            CellOffset = data.m_Offset;
            List<TileMapData.TileInfo> nodes = data.m_MapData;
            foreach (TileMapData.TileInfo node in nodes)
            {
                Gizmos.color = node.m_Status == -1 ? Color.red : Color.green;
                Gizmos.DrawCube(transform.position + transform.right*node.m_Offset.x+Vector3.up * (node.m_Offset.y + .5f)+ transform.forward * node.m_Offset.z, new Vector3(CellOffset.x / 2, 1f, CellOffset.y / 2));
            }
        }
    }
#endif
}
