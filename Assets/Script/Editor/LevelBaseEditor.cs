using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

[CustomEditor(typeof(LevelBase))]
public class LevelBaseEditor : Editor
{
    string GetLevelDataFolder(enum_LevelType type,string name)
    {
        return "Resources/Level/Data/"+type.ToString()+"/" + name;
    }
    LevelBase levelTarget=null;
    public override void OnInspectorGUI()
    {
        levelTarget = target as LevelBase;
        base.OnInspectorGUI();
        if (levelTarget.m_LevelType ==  enum_LevelType.Invalid||EditorApplication.isPlaying)
            return;

        if (levelTarget.data == null )
        {
            if (GUILayout.Button("Bake"))
            {
                BakeData();
            }
        }
        else
        {
            if (GUILayout.Button("Rebake"))
            {
                DeleteData();
                BakeData();
            }
            if (GUILayout.Button("Clear"))
            {
                DeleteData();
            }
        }
    }
    void BakeData()
    {
        
        TileMapData data = TResources.Load<TileMapData>(GetLevelDataFolder(levelTarget.m_LevelType, levelTarget.name));
        if (data == null)
        {
            data = CreateInstance<TileMapData>();
            if (!Directory.Exists("Assets/Resources/Level/Data/" + levelTarget.m_LevelType.ToString()))
                Directory.CreateDirectory("Assets/Resources/Level/Data/" + levelTarget.m_LevelType.ToString());

            AssetDatabase.CreateAsset(data, "Assets/" + GetLevelDataFolder(levelTarget.m_LevelType, levelTarget.name) + ".asset");
        }

        data.Bake(levelTarget.transform, levelTarget.I_CellWidthCount, levelTarget.I_CellHeightCount, Vector2.one * GameConst.F_LevelTileSize, levelTarget.F_HeightDetect, levelTarget.B_IgnoreUnavailable);

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    void DeleteData()
    {
        AssetDatabase.DeleteAsset("Assets/" + GetLevelDataFolder(levelTarget.m_LevelType, levelTarget.name) + ".asset");
    }
}
