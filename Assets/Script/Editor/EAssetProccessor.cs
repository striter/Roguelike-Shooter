
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

public class EPostProcessor : AssetPostprocessor
{
    public static void CreatePrefab( GameObject go, enum_LevelStyle levelStyle, bool isLevel)
    {
        string prefabParent = "Assets/GeneratedPrefab/" +(isLevel?"Level":EMaterialImportSetting.levelStyle.ToString());
        string prefabPath = prefabParent + "/"+  (isLevel?"Level":"Item") + go.name + ".prefab";
        if (!Directory.Exists(prefabParent))
            Directory.CreateDirectory(prefabParent);

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            AssetDatabase.DeleteAsset(prefabPath);

        GameObject prefab = PrefabUtility.CreatePrefab(prefabPath,go);

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] matList = new Material[renderers[i].sharedMaterials.Length];
            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                matList[j]=( EMaterialImportSetting.CreateMaterial(levelStyle, isLevel, renderers[i].sharedMaterials[j]));
            renderers[i].sharedMaterials = matList;
        }
        Debug.Log("Prefab:" + prefabPath + " Generate Complete");   
    }
}


public class EMaterialImportSetting : EditorWindow
{
    [MenuItem("ImportSetting/GenerateModelShadedPrefab")]
    public static void ChangeGameObjectShader()
    {
        // Get existing open window or if none, make a new one:
        EMaterialImportSetting window = GetWindow(typeof(EMaterialImportSetting)) as EMaterialImportSetting;
        window.Show();
    }
    public static enum_LevelStyle levelStyle { get; private set; } = enum_LevelStyle.Invalid;
    public static bool isLevel { get; private set; } = false;
    public enum enum_ShaderType
    {
        Invalid = -1,
        LowPoly_UVColorDiffuse = 1,
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        isLevel = EditorGUILayout.Toggle("IsLevel",isLevel);
        if(!isLevel)
            levelStyle = (enum_LevelStyle)EditorGUILayout.EnumPopup("Level Type", levelStyle);

        Object[] assets = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        if (assets.Length > 0)
        {
            EditorGUILayout.TextArea("SelectingAssets:");
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                EditorGUILayout.TextArea(Selection.objects[i].name);
            }

            if (isLevel||levelStyle != enum_LevelStyle.Invalid)
                if (EditorGUILayout.DropdownButton(new GUIContent("Create Shaded Prefab"), FocusType.Passive))
                    CreatePrefabFromModel(assets,levelStyle, isLevel);
        }

        EditorGUILayout.EndVertical();

    }
    void CreatePrefabFromModel(Object[] assets,enum_LevelStyle levelStyle,bool isLevel)
    {
        DeleteMaterial(levelStyle, isLevel);

        for (int i = 0; i < assets.Length; i++)
        {
            EPostProcessor.CreatePrefab(assets[i] as GameObject, levelStyle, isLevel);
        }
    }
    public static void DeleteMaterial(enum_LevelStyle levelStype,bool islevel)
    {
        string path = GetMaterialPath(levelStype, islevel);
        if (AssetDatabase.LoadAssetAtPath<Material>(path) != null)
             AssetDatabase.DeleteAsset(path);
    }
    public static Material CreateMaterial(enum_LevelStyle lsType,bool isLevel,Material sharedMaterial=null)
    {
        string folderParent = "Assets/Material/";
        string folderPath = GetMaterialPath(lsType,isLevel);

        if (!Directory.Exists(folderParent))
            Directory.CreateDirectory(folderParent);

        if (AssetDatabase.LoadAssetAtPath<Material>(folderPath) != null)
            return AssetDatabase.LoadAssetAtPath<Material>(folderPath);
        
        Shader targetShader = GetShader( enum_ShaderType.LowPoly_UVColorDiffuse);
        Material target = sharedMaterial==null?new Material(targetShader) :new Material(sharedMaterial);
        target.shader = targetShader;
        AssetDatabase.CreateAsset(target, folderPath);

        return AssetDatabase.LoadAssetAtPath<Material>(folderPath);
    }
    static string GetMaterialPath( enum_LevelStyle matStyle, bool isLevel)
    {
        if (isLevel)
            return "Assets/Material/Level.mat";
        return "Assets/Material" + (isLevel ? "Item_" : "Level_") + matStyle.ToString() + ".mat";
    }
    public static Shader GetShader(enum_ShaderType sType)
    {
        Shader shader;
        switch (sType)
        {
            default:
                shader = null;
                break;
            case enum_ShaderType.LowPoly_UVColorDiffuse:
                shader = Shader.Find("Game/LowPoly_UVColor_Diffuse");
                break;
        }
        if (shader == null)
        {
            Debug.LogWarning("Null Shader Found:" + sType.ToString());
        }
        return shader;
    }
}
