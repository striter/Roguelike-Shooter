
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;

public class EPostProcessor : AssetPostprocessor
{
}


public class EModelWorkFlow : EditorWindow
{
    [MenuItem("WorkFlow/CreateShadedPrefabFromModel")]
    public static void CreateShadedPrefab()
    {
        // Get existing open window or if none, make a new one:
        EModelWorkFlow window = GetWindow(typeof(EModelWorkFlow)) as EModelWorkFlow;
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
                ProcessLevelModel(instantiatePrefab);
            else
                ProcessItemModel(instantiatePrefab, levelStyle);

            PrefabUtility.CreatePrefab(prefabPath,instantiatePrefab);

            DestroyImmediate(instantiatePrefab);

            Debug.Log(isLevel?"Level":"Item"+ " Prefab:" + prefabPath + " Generate Complete");
        }

        AssetDatabase.SaveAssets();
    }

    void ProcessLevelModel(GameObject prefab)
    {
        GameObject itemPath = new GameObject("Item");
        itemPath.transform.SetParent(prefab.transform);
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        for (int j = 0; j < renderers.Length; j++)
        {
            Material[] matList = new Material[renderers[j].sharedMaterials.Length];
            for (int k = 0; k < renderers[j].sharedMaterials.Length; k++)
                matList[k] = (CreateMaterial(levelStyle, isLevel, renderers[j].sharedMaterials[k]));
            renderers[j].sharedMaterials = matList;

            renderers[j].gameObject.AddComponent<MeshCollider>();
            renderers[j].gameObject.AddComponent<HitCheckStatic>();
        }
        LevelBase level = prefab.AddComponent<LevelBase>();
        level.E_PrefabType = (enum_LevelPrefabType)System.Enum.Parse(typeof(enum_LevelPrefabType), prefab.name.Split('_')[0]); 
        LevelBaseEditor.BakeData(level);
    }
    void ProcessItemModel(GameObject prefab, enum_LevelStyle levelStyle)
    {
        enum_LevelItemType type= (enum_LevelItemType)System.Enum.Parse(typeof(enum_LevelItemType), prefab.name.Split('_')[0]);
        
        LevelItemBase levelItem=  prefab.AddComponent<LevelItemBase>();
        levelItem.m_ItemType = type;
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] matList = new Material[renderers[i].sharedMaterials.Length];
            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                matList[j] = (CreateMaterial(levelStyle, isLevel, renderers[i].sharedMaterials[j]));
            renderers[i].sharedMaterials = matList;

            if (levelItem.m_ItemType == enum_LevelItemType.NoCollisionMore||levelItem.m_ItemType== enum_LevelItemType.NoCollisionLess)
            {
                levelItem.m_sizeXAxis = 1;
                levelItem.m_sizeYAxis = 1;
            }
            else
            {
                MeshCollider collider = renderers[i].gameObject.AddComponent<MeshCollider>();
                renderers[i].gameObject.AddComponent<HitCheckStatic>();

                levelItem.m_sizeXAxis = (int)(collider.bounds.extents.x * 2 / GameConst.F_LevelTileSize) + 1;
                levelItem.m_sizeYAxis = (int)(collider.bounds.extents.z * 2 / GameConst.F_LevelTileSize) + 1;
            }
            levelItem.ItemRecenter();
        }
    }
    static string PrefabPath(bool isLevel,enum_LevelStyle style)
    {
        if (isLevel)
            return "Assets/Resources/"+TResources.ConstPath.S_LevelMain;
        else
            return "Assets/Resources/"+ TResources.ConstPath.S_LeveLItem+"/" + style.ToString();
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
