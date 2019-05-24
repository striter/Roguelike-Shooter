
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

public class EPostProcessor : AssetPostprocessor
{
    public static void CreatePrefab(GameObject go,bool isLevel)
    {
        string prefabParent = "Assets/GeneratedPrefab/" + EMaterialImportSetting.levelType.ToString();
        string prefabPath = prefabParent + "/"+  (isLevel?"Level":"Item") + go.name + ".prefab";
        if (!Directory.Exists(prefabParent))
            Directory.CreateDirectory(prefabParent);

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            AssetDatabase.DeleteAsset(prefabPath);

        GameObject prefab = PrefabUtility.CreatePrefab(prefabPath,go);

        if (EMaterialImportSetting.shaderType == EMaterialImportSetting.enum_ShaderType.LowPoly_UVColorDiffuse)
            EMaterialImportSetting.DeleteMaterial(isLevel ? "Level" : "Item", go.name);

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] matList = new Material[renderers[i].sharedMaterials.Length];
            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
            {
                matList[j]=( EMaterialImportSetting.GetMaterial(go, renderers[i].sharedMaterials[j],"material"+i.ToString()+j.ToString(), isLevel));
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
    public static enum_LevelStyle levelType { get; private set; } = enum_LevelStyle.Invalid;
    public static bool isLevel { get; private set; } = false;
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
        levelType = (enum_LevelStyle)EditorGUILayout.EnumPopup("Level Type", levelType);
        isLevel = EditorGUILayout.Toggle(isLevel);
        Object[] assets = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        if (assets.Length > 0)
        {
            EditorGUILayout.TextArea("SelectingAssets:");
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                EditorGUILayout.TextArea(Selection.objects[i].name);
            }

            if (shaderType != enum_ShaderType.Invalid && levelType != enum_LevelStyle.Invalid)
                if (EditorGUILayout.DropdownButton(new GUIContent("Create Shaded Prefab"), FocusType.Passive))
                    CreatePrefabFromModel(assets, isLevel);
        }

        EditorGUILayout.EndVertical();

    }
    void CreatePrefabFromModel(Object[] assets,bool isLevel)
    {
        for (int i = 0; i < assets.Length; i++)
        {
            EPostProcessor.CreatePrefab(assets[i] as GameObject, isLevel);
        }
    }
    public static Material GetMaterial(GameObject go,Material sharedMaterial,string materialName,bool isLevel)
    {
        switch (shaderType)
        {
            case enum_ShaderType.LowPoly_UVColorDiffuse:            //All Level Or Item Goes Here
                return CreateMaterial(isLevel?"Level/":"Item/" , go.name, shaderType, sharedMaterial);
            default:
                return CreateMaterial("Uncommon/" + go.name, sharedMaterial.name, shaderType, sharedMaterial);
        }
    }
    public static void DeleteMaterial(string parentFolder, string matName)
    {
        string path = "Assets/Material/" + parentFolder + "/" + matName + ".mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path) != null)
             AssetDatabase.DeleteAsset(path);
    }
    static Material CreateMaterial(string parentfolder,string matName,enum_ShaderType sType,Material sharedMaterial=null)
    {
        string folderParent = "Assets/Material/" + parentfolder;
        string folderPath = folderParent + "/" + matName + ".mat";

        if (!Directory.Exists(folderParent))
            Directory.CreateDirectory(folderParent);

        if (AssetDatabase.LoadAssetAtPath<Material>(folderPath) != null)
            return AssetDatabase.LoadAssetAtPath<Material>(folderPath);
        
        Shader targetShader = GetShader(sType);
        Material target = sharedMaterial==null?new Material(targetShader) :new Material(sharedMaterial);
        target.shader = targetShader;
        AssetDatabase.CreateAsset(target, folderPath);

        return AssetDatabase.LoadAssetAtPath<Material>(folderPath);
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
