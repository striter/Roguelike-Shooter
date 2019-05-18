
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

public class EPostProcessor : AssetPostprocessor
{
    public static void CreatePrefab(GameObject go)
    {
        string prefabParent = "Assets/GeneratedPrefab/" + EMaterialImportSetting.levelType.ToString();
        string prefabPath = prefabParent + "/" + go.name + ".prefab";
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
            {
                matList[j]=( EMaterialImportSetting.GetMaterial(go, renderers[i].sharedMaterials[j],"material"+i.ToString()+j.ToString()));
            }
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
    public static enum_ShaderType shaderType { get; private set; } = enum_ShaderType.Invalid;
    public static enum_LevelType levelType { get; private set; } = enum_LevelType.Invalid;
    public enum enum_ShaderType
    {
        Invalid = -1,
        LowPoly_UVColorDiffuse = 1,
        LowPoly_OnlyColorDiffuse=2,
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.TextArea("Editing Shader Type");
        shaderType = (enum_ShaderType)EditorGUILayout.EnumPopup("Shader Type", shaderType);
        levelType = (enum_LevelType)EditorGUILayout.EnumPopup("Level Type", levelType);

        Object[] assets = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        if (assets.Length > 0)
        {
            EditorGUILayout.TextArea("SelectingAssets:");
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                EditorGUILayout.TextArea(Selection.objects[i].name);
            }

            if (shaderType != enum_ShaderType.Invalid && levelType != enum_LevelType.Invalid)
                if (EditorGUILayout.DropdownButton(new GUIContent("Create Shaded Prefab"), FocusType.Passive))
                    CreatePrefabFromModel(assets);
        }

        EditorGUILayout.EndVertical();

    }
    void CreatePrefabFromModel(Object[] assets)
    {
        for (int i = 0; i < assets.Length; i++)
        {
            EPostProcessor.CreatePrefab(assets[i] as GameObject);
        }
    }
    public static Material GetMaterial(GameObject go,Material sharedMaterial,string materialName)
    {
        switch (shaderType)
        {
            case enum_ShaderType.LowPoly_UVColorDiffuse:
                return AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/LowPoly_Diffuse.mat");
            default:
                {
                    

                    string folderParent = "Assets/Material/" + go.name;
                    string folderPath = folderParent + "/" + materialName + ".mat";

                    if (!Directory.Exists(folderParent))
                        Directory.CreateDirectory(folderParent);

                    if (AssetDatabase.LoadAssetAtPath<Material>(folderPath) != null)
                        AssetDatabase.DeleteAsset(folderPath);

                    Material target = new Material(sharedMaterial);
                    target.shader = CurrentShader();
                    AssetDatabase.CreateAsset(target, folderPath);

                    return AssetDatabase.LoadAssetAtPath<Material>(folderPath) ;
                }
                
        }
    }
    public static Shader CurrentShader()
    {
        Shader shader;
        switch (shaderType)
        {
            default:
                shader = null;
                break;
            case enum_ShaderType.LowPoly_UVColorDiffuse:
                shader = Shader.Find("Game/LowPoly_UVColor_Diffuse");
                break;
            case enum_ShaderType.LowPoly_OnlyColorDiffuse:
                shader = Shader.Find("Game/LowPoly_OnlyColor_Diffuse");
                    break;
        }
        if (shader == null)
        {
            Debug.LogWarning("Null Shader Found:" + shaderType.ToString());
        }
        return shader;
    }
}
