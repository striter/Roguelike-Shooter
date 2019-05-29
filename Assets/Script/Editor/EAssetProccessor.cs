
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

public class EPostProcessor : AssetPostprocessor
{
}


public class EModelWorkdFlow : EditorWindow
{
    [MenuItem("WorkFlow/LevelItemAddCollision")]
    public static void AddGameObjectCollision()
    {
        Object[] assets = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        for (int i = 0; i < assets.Length; i++)
        {
            LevelItemBase levelItem=(assets[i] as GameObject).GetComponent<LevelItemBase>() ;
            if (levelItem == null||levelItem.m_ItemType== enum_LevelItemType.Invalid)
            {
                Debug.LogError("This Work Flow Only Work With Componented And Setted LevelItem!"+levelItem.gameObject);
                break;
            }
            if (levelItem.m_ItemType == enum_LevelItemType.NoCollision)
                continue;

            Renderer[] renderers = levelItem.GetComponentsInChildren<Renderer>();
            for (int j = 0; j < renderers.Length; j++)
            {
                if(renderers[j].GetComponent<MeshCollider>()==null)
                    renderers[j].gameObject.AddComponent<MeshCollider>();
                if (renderers[j].GetComponent<HitCheckStatic>() == null)
                    renderers[j].gameObject.AddComponent<HitCheckStatic>();
            }
        }
        AssetDatabase.SaveAssets();
    }
    [MenuItem("WorkFlow/CreateShadedPrefabFromModel")]
    public static void CreateShadedPrefab()
    {
        // Get existing open window or if none, make a new one:
        EModelWorkdFlow window = GetWindow(typeof(EModelWorkdFlow)) as EModelWorkdFlow;
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
            GameObject model = assets[i] as GameObject;
            string prefabFolder = PrefabPath(isLevel,levelStyle);
            string prefabPath = prefabFolder + "/"+ model.name + ".prefab";
            if (!Directory.Exists(prefabFolder))
                Directory.CreateDirectory(prefabFolder);

            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
                AssetDatabase.DeleteAsset(prefabPath);

            GameObject instantiatePrefab = new GameObject(model.name);
            GameObject instantiateModel = GameObject.Instantiate(model, instantiatePrefab.transform);
            instantiateModel.name = "Model";
            if (isLevel)
            {
                GameObject itemPath = new GameObject("Item");
                itemPath.transform.SetParent(instantiatePrefab.transform);
            }
        
            Renderer[] renderers = instantiatePrefab.GetComponentsInChildren<Renderer>();

            for (int j = 0; j < renderers.Length; j++)
            {
                Material[] matList = new Material[renderers[j].sharedMaterials.Length];
                for (int k = 0; k < renderers[j].sharedMaterials.Length; k++)
                    matList[k] = (CreateMaterial(levelStyle, isLevel, renderers[j].sharedMaterials[k]));
                renderers[j].sharedMaterials = matList;

                if (isLevel)
                {
                    renderers[j].gameObject.AddComponent<MeshCollider>();
                    renderers[j].gameObject.AddComponent<HitCheckStatic>();
                }
            }

            if (isLevel)
                LevelBaseEditor.BakeData(instantiatePrefab.AddComponent<LevelBase>());
            else
                instantiatePrefab.AddComponent<LevelItemBase>();

            PrefabUtility.CreatePrefab(prefabPath, instantiatePrefab);
            DestroyImmediate(instantiatePrefab);
            Debug.Log("Prefab:" + prefabPath + " Generate Complete");
        }

        AssetDatabase.SaveAssets();
    }
    static string PrefabPath(bool isLevel,enum_LevelStyle style)
    {
        if (isLevel)
            return "Assets/Resources/"+TResources.ConstPath.S_LevelMain;
        else
            return "Assets/Resources/"+ TResources.ConstPath.S_LeveLItem+"/" + style.ToString();
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
        return "Assets/Material/Item_"  + matStyle.ToString() + ".mat";
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
