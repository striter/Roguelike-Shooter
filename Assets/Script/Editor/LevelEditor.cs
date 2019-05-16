using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSetting;
[CustomEditor(typeof(LevelBase))]
public class LevelEditor : Editor {
    string GetLevelDataFolder(enum_LevelType type,string name)
    {
        return "Resources/Level/Data/"+type.ToString()+"/" + name;
    }
    public override void OnInspectorGUI()
    {
        LevelBase levelTarget = target as LevelBase;
        base.OnInspectorGUI();
        if (levelTarget.m_LevelType ==  enum_LevelType.Invalid)
            return;

        if (levelTarget.data == null )
        {
            if (GUILayout.Button("Null Data,Click To Bake"))
            {
                TileMapData data = TResources.Load<TileMapData>(GetLevelDataFolder(levelTarget.m_LevelType,levelTarget.name));
                if (data == null)
                {
                    data = CreateInstance<TileMapData>();
                    AssetDatabase.CreateAsset(data, "Assets/"+GetLevelDataFolder(levelTarget.m_LevelType, levelTarget.name) + ".asset");
                    AssetDatabase.SaveAssets();
                }

                data.Bake(levelTarget.transform, levelTarget.CellWidthCount, levelTarget.CellHeightCount, levelTarget.CellOffset,levelTarget.b_bakeUnavailable);
            }
        }
        else
        {
            if (GUILayout.Button("Already Have Data,Click To Clear"))
            {
                AssetDatabase.DeleteAsset("Assets/" + GetLevelDataFolder(levelTarget.m_LevelType, levelTarget.name) + ".asset");
            }
        }
    }
}
