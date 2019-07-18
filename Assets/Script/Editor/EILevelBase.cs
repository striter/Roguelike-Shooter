using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

[CustomEditor(typeof(LevelBase)),CanEditMultipleObjects]
public class EILevelBase : Editor
{
    LevelBase levelTarget = null;
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
                EWorkFlow_LevelDataGenerating.BakeData(levelTarget);
            }
        }
        else
        {
            if (GUILayout.Button("Rebake"))
            {
                EWorkFlow_LevelDataGenerating.DeleteData(levelTarget);
                EWorkFlow_LevelDataGenerating.BakeData(levelTarget);
            }
            if (GUILayout.Button("Clear"))
            {
                EWorkFlow_LevelDataGenerating.DeleteData(levelTarget);
            }
        }
    }
}
