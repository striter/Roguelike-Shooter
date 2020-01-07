using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using GameSetting;
using UnityEditor;

public class LevelEditorManager : SimpleSingletonMono<LevelEditorManager>
{
    public bool B_DrawTileGizmos;

    protected override void Awake()
    {
        base.Awake();
        LevelObjectManager.Init();
    }
    private void Start()
    {
        LevelObjectManager.Register(enum_LevelStyle.Forest);
    }


    #region FileEdit
    public void New(int sizeX,int sizeY,enum_ChunkType type)=>LevelChunkEditor.Instance.Init(LevelChunkData.NewData(sizeX, sizeY,type));
    
    public void Read(string dataName)
    {
        if (dataName == "")
        {
            Debug.LogError("Edit Data Name Before Read!");
            return;
        }
        LevelChunkData m_Data = TResources.GetLevelData(dataName);
        if (!m_Data)
            return;
        LevelChunkEditor.Instance.Init(m_Data);
    }

    public void Save(string dataName)
    {
        if (dataName == "")
        {
            Debug.LogError("Edit Data Name Before Save!");
            return;
        }

        string dataPath = TResources.ConstPath.S_LevelChunkData + "/"+dataName + ".asset";
        LevelChunkData data = TResources.GetLevelData(dataName);
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<LevelChunkData>();
            AssetDatabase.CreateAsset(data, "Assets/Resources/" + dataPath);
        }
        LevelChunkEditor.Instance.Desize();
        data.SaveData(LevelChunkEditor.Instance);

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #endregion
}

public static class LevelObjectManager
{
    public static bool m_Registed=false;
    public static void Init()
    {
        ObjectPoolManager.Init();
    }
    public static void Register(enum_LevelStyle style)
    {
        m_Registed = true;
        TResources.GetLevelItemsNew(style).Traversal((enum_TileSubType type,List<LevelTileItemBase> items)=> {
            switch (type)
            {
                default:Debug.LogError("Invalid Pharse Here!");break;
                case enum_TileSubType.Ground:
                    items.Traversal((LevelTileItemBase item) =>
                    {
                        TileGroundBase groundItem = item as TileGroundBase;
                        ObjectPoolManager<enum_TileGroundType, TileGroundBase>.Register(groundItem.m_GroundType, groundItem, 1);
                    });
                    break;
                case enum_TileSubType.Object:
                    items.Traversal((LevelTileItemBase item) =>
                    {
                        TileObjectBase objectItem = item as TileObjectBase;
                        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Register(objectItem.m_ObjectType, objectItem, 1);
                    });
                    break;
                case enum_TileSubType.Pillar:
                    items.Traversal((LevelTileItemBase item) =>
                    {
                        TilePillarBase pillarItem = item as TilePillarBase;
                        ObjectPoolManager<enum_TilePillarType, TilePillarBase>.Register(pillarItem.m_PillarType, pillarItem, 1);
                    });
                    break;
            }
        });
    }

    public static void Clear()
    {
        m_Registed = false;
        ObjectPoolManager<enum_TileGroundType, TileGroundBase>.DestroyAll();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.DestroyAll();
        ObjectPoolManager<enum_TilePillarType, TilePillarBase>.DestroyAll();
    }

    public static TilePillarBase GetPillar(enum_TilePillarType type, Transform trans)=> ObjectPoolManager<enum_TilePillarType, TilePillarBase>.Spawn(type, trans);
    public static TileObjectBase GetObject(enum_TileObjectType type, Transform trans) => ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Spawn(type, trans);
    public static TileGroundBase GetGround(enum_TileGroundType type, Transform trans) => ObjectPoolManager<enum_TileGroundType, TileGroundBase>.Spawn(type, trans);
}
