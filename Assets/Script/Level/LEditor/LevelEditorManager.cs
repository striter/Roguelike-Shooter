using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using GameSetting;

public class LevelEditorManager : SingletonMono<LevelEditorManager>
{
    private void Start()
    {
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BloomSpecific>().m_Blur.SetEffect(PE_Blurs.enum_BlurType.GaussianBlur,3,10,2);
        CameraController.Instance.m_Effect.SetMainTextureCamera(true);
        CameraController.Instance.m_Effect.ResetCameraEffectParams();
    }
    #region FileEdit
    public void New(int sizeX,int sizeY)=>LevelChunkEditor.Instance.Init(LevelChunkData.NewData(sizeX, sizeY));
    
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
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(m_Data));
#endif
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

