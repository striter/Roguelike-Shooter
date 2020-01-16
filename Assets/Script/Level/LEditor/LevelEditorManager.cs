using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using GameSetting;

public class LevelEditorManager : SimpleSingletonMono<LevelEditorManager>
{
    public bool B_DrawTileGizmos;
    

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
        LevelChunkData data = TResources.GetLevelData(dataName);
#if UNITY_EDITOR
        string dataPath = TResources.ConstPath.S_ChunkData + "/"+dataName + ".asset";
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<LevelChunkData>();
            UnityEditor.AssetDatabase.CreateAsset(data, "Assets/Resources/" + dataPath);
        }
        data.SaveData(LevelChunkEditor.Instance);

        UnityEditor.EditorUtility.SetDirty(data);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
#endif
        return data;
    }
#endregion
}

