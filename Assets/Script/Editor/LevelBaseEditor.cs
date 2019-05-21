using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

[CustomEditor(typeof(LevelBase))]
public class LevelBaseEditor : Editor
{
    string GetLevelDataFolder(enum_LevelStyle type,string name)
    {
        return "Resources/Level/Main/" + type.ToString()+"/" + name;
    }
    LevelBase levelTarget=null;
    public override void OnInspectorGUI()
    {
        levelTarget = target as LevelBase;
        base.OnInspectorGUI();
        if (levelTarget.m_levelStyle ==  enum_LevelStyle.Invalid||EditorApplication.isPlaying)
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
        
        TileMapData data = TResources.Load<TileMapData>(GetLevelDataFolder(levelTarget.m_levelStyle, levelTarget.name));
        if (data == null)
        {
            data = CreateInstance<TileMapData>();
            if (!Directory.Exists("Assets/Resources/Level/Main/" + levelTarget.m_levelStyle.ToString()))
                Directory.CreateDirectory("Assets/Resources/Level/Main/" + levelTarget.m_levelStyle.ToString());

            AssetDatabase.CreateAsset(data, "Assets/" + GetLevelDataFolder(levelTarget.m_levelStyle, levelTarget.name) + ".asset");
        }

        data.Bake(levelTarget.transform, levelTarget.I_DiamCellCount, levelTarget.I_DiamCellCount, Vector2.one * GameConst.F_LevelTileSize, levelTarget.F_HeightDetect, levelTarget.B_IgnoreUnavailable, levelTarget.b_BakeCircle);

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    void DeleteData()
    {
        AssetDatabase.DeleteAsset("Assets/" + GetLevelDataFolder(levelTarget.m_levelStyle, levelTarget.name) + ".asset");
    }
}
