using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

[CustomEditor(typeof(LevelBase)),CanEditMultipleObjects]
public class LevelBaseEditor : Editor
{

    LevelBase levelTarget = null;

    static string GetDataPath(string name)
    {
        return "Resources/" + TResources.ConstPath.S_LevelData+"/" + name;
    }
    public override void OnInspectorGUI()
    {

        levelTarget = target as LevelBase;
        base.OnInspectorGUI();
        if (EditorApplication.isPlaying||AssetDatabase.IsNativeAsset(target))
            return;

        if (levelTarget.gizmosMapData == null )
        {
            if (GUILayout.Button("Bake"))
            {
                BakeData(levelTarget);
            }
        }
        else
        {
            if (GUILayout.Button("Rebake"))
            {
                DeleteData();
                BakeData(levelTarget);
            }
            if (GUILayout.Button("Clear"))
            {
                DeleteData();
            }
        }
    }
   public static  void BakeData(LevelBase levelTarget)
    {
        
        TileMapData data = TResources.Load<TileMapData>(GetDataPath(levelTarget.name));
        if (data == null)
        {
            data = CreateInstance<TileMapData>();
            if (!Directory.Exists("Assets/Resources/"+TResources.ConstPath.S_LevelData))
                Directory.CreateDirectory("Assets/Resources/" + TResources.ConstPath.S_LevelData);

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
