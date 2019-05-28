using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

[CustomEditor(typeof(LevelBase))]
public class LevelBaseEditor : Editor
{
    string GetDataPath(string name)
    {
        return "Resources/Level/Main/" + name;
    }
    LevelBase levelTarget=null;
    public override void OnInspectorGUI()
    {
        levelTarget = target as LevelBase;
        base.OnInspectorGUI();
        if (EditorApplication.isPlaying)
            return;

        if (levelTarget.gizmosMapData == null )
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
        
        TileMapData data = TResources.Load<TileMapData>(GetDataPath(levelTarget.name));
        if (data == null)
        {
            data = CreateInstance<TileMapData>();
            if (!Directory.Exists("Assets/Resources/Level/Main"))
                Directory.CreateDirectory("Assets/Resources/Level/Main");

            AssetDatabase.CreateAsset(data, "Assets/" + GetDataPath( levelTarget.name) + ".asset");
        }

        data.Bake(levelTarget.transform, levelTarget.I_DiamCellCount, levelTarget.I_DiamCellCount, Vector2.one * GameConst.F_LevelTileSize, levelTarget.F_HeightDetect, levelTarget.B_IgnoreUnavailable, levelTarget.b_BakeCircle);

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    void DeleteData()
    {
        AssetDatabase.DeleteAsset("Assets/" + GetDataPath(levelTarget.name) + ".asset");
    }
}
