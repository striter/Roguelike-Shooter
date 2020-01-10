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
    }

    #region FileEdit
    public void New(int sizeX,int sizeY,enum_ChunkType type)=>LevelChunkEditor.Instance.Init(LevelChunkData.NewData(sizeX, sizeY,type));
    
    public LevelChunkData Read(string dataName)
    {
        if (dataName == "")
        {
            Debug.LogError("Edit Data Name Before Read!");
            return null;
        }
        LevelChunkData m_Data = TResources.GetLevelData(dataName);
        if (!m_Data)
            return null;
        LevelChunkEditor.Instance.Init(m_Data);
        return m_Data;
    }

    public LevelChunkData Save(string dataName)
    {
        if (dataName == "")
        {
            Debug.LogError("Edit Data Name Before Save!");
            return null;
        }

        string dataPath = TResources.ConstPath.S_LevelChunkData + "/"+dataName + ".asset";
        LevelChunkData data = TResources.GetLevelData(dataName);
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<LevelChunkData>();
            AssetDatabase.CreateAsset(data, "Assets/Resources/" + dataPath);
        }
        data.SaveData(LevelChunkEditor.Instance);

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return data;
    }
    #endregion
}

