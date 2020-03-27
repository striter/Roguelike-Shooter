using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using GameSetting;
using UnityEditor;

public class LevelEditorManager : SingletonMono<LevelEditorManager>
{
    private void Start()
    {
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BloomSpecific>().m_Blur.SetEffect(PE_Blurs.enum_BlurType.GaussianBlur,3,10,2);
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<CB_GenerateOpaqueTexture>();
        CameraController.Instance.m_Effect.SetCameraEffects( DepthTextureMode.Depth);
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
        LevelChunkData m_Data = TResources.GetChunkData(dataName);
        if (!m_Data)
            return null;
        LevelChunkEditor.Instance.Init(m_Data);
        return m_Data;
    }

    public void Delete(string dataName)
    {
        LevelChunkData m_Data = TResources.GetChunkData(dataName);
        if (!m_Data)
            return;
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(m_Data));
    }

    public LevelChunkData Save(string dataName)
    {
        if (dataName == "")
        {
            Debug.LogError("Edit Data Name Before Save!");
            return null;
        }
        LevelChunkData data = TResources.GetChunkData(dataName);
#if UNITY_EDITOR
        string dataPath = TResources.ConstPath.S_ChunkData + "/"+dataName + ".asset";
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<LevelChunkData>();
            UnityEditor.AssetDatabase.CreateAsset(data, "Assets/Resources/" + dataPath);
        }
        data.SaveData(LevelChunkEditor.Instance.CheckDatas());

        UnityEditor.EditorUtility.SetDirty(data);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
#endif
        return data;
    }
#endregion
}

