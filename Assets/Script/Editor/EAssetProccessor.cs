using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSetting;
public class EPostProcessor : AssetPostprocessor
{
    public static void ExtractMaterial(GameObject go,ModelImporter importer)
    {
        if (EMaterialImportSetting.shaderType == EMaterialImportSetting.enum_ShaderType.LowPoly_Diffuse)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/LowPoly_Diffuse.mat");
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Everywhere);
        }
        else
        {
            Shader shader = EMaterialImportSetting.Get();

            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            for (int j = 0; j < renderers.Length; j++)
            {
                for (int k = 0; k < renderers[j].sharedMaterials.Length; k++)
                {
                    Material mat = renderers[j].sharedMaterials[k];

                    string folderParent = "Assets/Material/" + EMaterialImportSetting.levelType.ToString();
                    string folderPath = folderParent + "/" + go.name;
                    string materialPath = folderPath + "/" + mat.name + ".mat";
                    if (!AssetDatabase.IsValidFolder("Assets/Material"))
                        AssetDatabase.CreateFolder("Assets", "Material");

                    if (!AssetDatabase.IsValidFolder(folderParent))
                        AssetDatabase.CreateFolder("Assets/Material", EMaterialImportSetting.levelType.ToString());

                    if (!AssetDatabase.IsValidFolder(folderPath))
                        AssetDatabase.CreateFolder(folderParent, go.name);

                    if (AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) == null)
                        AssetDatabase.CreateAsset(new Material(mat), materialPath);

                    Material targetMaterial = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
                    targetMaterial.shader = shader;
                    renderers[j].sharedMaterials[k] = targetMaterial;
                }
            }
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Everywhere);
        }
    }
}


public class EMaterialImportSetting : EditorWindow
{
    [MenuItem("ImportSetting/ImportTypeShaderSetting")]
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
        LowPoly_Diffuse = 1,
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
                if (EditorGUILayout.DropdownButton(new GUIContent("Extract Materials"), FocusType.Passive))
                    ExtractMaterial(assets);
        }

        EditorGUILayout.EndVertical();

    }
    void ExtractMaterial(Object[] assets)
    {
        for (int i = 0; i < assets.Length; i++)
        {
            EPostProcessor.ExtractMaterial(assets[i] as GameObject,(ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(assets[i])));
        }
    }

    public static Shader Get()
    {
        Shader shader;
        switch (shaderType)
        {
            default:
                shader = null;
                break;
            case enum_ShaderType.LowPoly_Diffuse:
                shader = Shader.Find("Game/LowPoly_Diffuse");
                break;
        }
        if (shader == null)
        {
            Debug.LogWarning("Null Shader Found:" + shaderType.ToString());
        }
        return shader;
    }
}
