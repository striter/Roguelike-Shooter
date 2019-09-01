
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSetting;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using EToolsEditor;

public class EPostProcessor : AssetPostprocessor
{
}
public class EWorkFlow_StyleColorCustomization : EditorWindow
{
    [MenuItem("Work Flow/Style_ColorSerialization(Direction,Ocean,Ambient)",false,0)]
    public static void StyleColorSerialization()
    {
        EWorkFlow_StyleColorCustomization window= GetWindow(typeof(EWorkFlow_StyleColorCustomization)) as EWorkFlow_StyleColorCustomization;
        selectingStyleType = enum_Style.Invalid;
        previousData = StyleColorData.Default();
        Init();
        previousData.SaveData(directionalLight);
        window.Show();
    }
    static enum_Style selectingStyleType = enum_Style.Invalid;
    static bool newStyleData = false;
    static Light directionalLight;
    static StyleColorData previousData;
    string extraName="Please Edit This";
    static void Init()
    {
        if (EditorApplication.isPlaying)
            directionalLight = EnvironmentManager.Instance.m_DirectionalLight;
        else
            directionalLight = GameObject.Find("Enviorment/Directional Light").GetComponent<Light>();
    }
    private void OnGUI()
    {

        GUILayout.BeginVertical();
        if (EditorSceneManager.GetActiveScene().name != "Game")
        {
            GUILayout.TextArea("This Work Flow Can Only Activated At Scene: Game");
            GUILayout.EndVertical();
            return;
        }

        Init();


        if (selectingStyleType == enum_Style.Invalid)
            TCommon.TraversalEnum((enum_Style style) =>
            {
                GUILayout.BeginHorizontal();
                StyleColorData[] customizations = TResources.GetAllStyleCustomization(style);
                GUILayout.TextArea(style.ToString() + ":" + customizations.Length.ToString() + " Customizations");
                if (customizations.Length > 0 && GUILayout.Button("Show Random"))
                {
                    StyleColorData data = customizations.RandomItem();
                    data.DataInit(directionalLight);
                    EditorGUIUtility.PingObject(data);
                }
                if (GUILayout.Button("Select " + style))
                    selectingStyleType = style;
                GUILayout.EndHorizontal();
            });
        else
        {
            StyleColorData[] customizations = TResources.GetAllStyleCustomization(selectingStyleType);
            GUILayout.TextArea(customizations.Length == 0 ? "Current Style Does Not Contains Any Customization,Please Save One" : "Current Style Customizations");
            if (!newStyleData)
            {
                customizations.Traversal((StyleColorData data) =>
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.TextArea(data.name);
                    if (GUILayout.Button("Edit"))
                    {
                        data.DataInit(directionalLight);
                        newStyleData = true;
                        extraName = data.name.Split('_')[1];
                        EditorGUIUtility.PingObject(data);
                    }
                    if (GUILayout.Button("Delete"))
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(data));
                    GUILayout.EndHorizontal();
                });

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("New"))
                    newStyleData = true;
                if (previousData && GUILayout.Button("Reset"))
                    previousData.DataInit(directionalLight);
                if (!EditorApplication.isPlaying&&GUILayout.Button("Return"))
                    selectingStyleType = enum_Style.Invalid;
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextArea("Directional Light:");
                directionalLight.color=EditorGUILayout.ColorField(directionalLight.color);
                EditorGUILayout.TextArea("Intensity:");
                directionalLight.intensity = EditorGUILayout.FloatField(directionalLight.intensity);
                EditorGUILayout.TextArea("Pitch:");
                float pitch = EditorGUILayout.FloatField(directionalLight.transform.eulerAngles.x);
                EditorGUILayout.TextArea("Yaw:");
                float yaw = EditorGUILayout.FloatField(directionalLight.transform.eulerAngles.y);
                directionalLight.transform.rotation = Quaternion.Euler(new Vector3(pitch, yaw, 0));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextArea("Ambient:Sky");
                RenderSettings.ambientSkyColor=EditorGUILayout.ColorField(RenderSettings.ambientSkyColor);
                EditorGUILayout.TextArea("Equator");
                RenderSettings.ambientEquatorColor=EditorGUILayout.ColorField(RenderSettings.ambientEquatorColor);
                EditorGUILayout.TextArea("Ground");
                RenderSettings.ambientGroundColor=EditorGUILayout.ColorField(RenderSettings.ambientGroundColor);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                 EditorGUILayout.TextArea("File Name");
                extraName = EditorGUILayout.TextField(extraName);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                bool od=false;
                for (int i = 0; i < customizations.Length; i++)
                {
                    if (customizations[i].name.Split('_')[1] == extraName)
                        od = true;
                }
                if (GUILayout.Button(od?"Override":"Save"))
                {
                    SaveCustomization(directionalLight, extraName);
                    newStyleData = false;
                }
                if (previousData&&GUILayout.Button("Reset"))
                    previousData.DataInit(directionalLight);
                if (GUILayout.Button("Cancel"))
                    newStyleData = false;
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }

    static void SaveCustomization(Light directional,string extraName)
    {
        string dataFolder = TResources.ConstPath.S_StyleCustomization + "/" + selectingStyleType;
        string dataPath = dataFolder + "/" +selectingStyleType+"_"+ extraName + ".asset";
        StyleColorData data = TResources.Load<StyleColorData>(dataPath);
        if (data == null)
        {
            data = CreateInstance<StyleColorData>();
            if (!Directory.Exists(TEditor.S_AssetDataBaseResources + dataFolder))
                Directory.CreateDirectory(TEditor.S_AssetDataBaseResources + dataFolder);

            AssetDatabase.CreateAsset(data, TEditor.S_AssetDataBaseResources + dataPath);
        }
        data.SaveData(directional);

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

public class EWorkFlow_ModelAutoPrefabPackaging : EditorWindow
{
    [MenuItem("Work Flow/Model_AutoPrefabPackaging(Material,Shader,Scripts)",false,0)]
    public static void ModelAutoPrefabPackaging()
    {
        // Get existing open window or if none, make a new one:
        EWorkFlow_ModelAutoPrefabPackaging window = GetWindow(typeof(EWorkFlow_ModelAutoPrefabPackaging)) as EWorkFlow_ModelAutoPrefabPackaging;
        window.Show();
    }
    public static enum_Style levelStyle { get; private set; } = enum_Style.Invalid;
    public enum enum_ShaderType
    {
        Invalid = -1,
        LowPoly_UVColorDiffuse = 1,
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        Object[] selected = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        List<Object> assets = selected.ToList();
        selected.Traversal((Object item) => { if (!AssetDatabase.IsMainAsset(item)||Path.GetExtension(AssetDatabase.GetAssetPath(item))!=".fbx") assets.Remove(item); });
        if(assets.Count!=selected.Length)
            EditorGUILayout.TextArea("Should Not Select Items That Aren't .fbx:");
        
        if (assets.Count > 0)
        {
            EditorGUILayout.TextArea("Currnet Available Assets:");
            for (int i = 0; i < assets.Count; i++)
                EditorGUILayout.TextArea(assets[i].name);

            levelStyle =(enum_Style) EditorGUILayout.EnumPopup("Item Style", levelStyle);

            if (levelStyle != enum_Style.Invalid)
                if (EditorGUILayout.DropdownButton(new GUIContent("Create Shaded Prefab"), FocusType.Passive))
                    CreatePrefabFromModel(assets, levelStyle);
        }

        EditorGUILayout.EndVertical();

    }
    void CreatePrefabFromModel(List<Object> assets,enum_Style levelStyle)
    {
        for (int i = 0; i < assets.Count; i++)
        {
            GameObject model = assets[i] as GameObject;
            string prefabFolder = PrefabPath(levelStyle);
            string prefabPath = prefabFolder + "/"+ model.name + ".prefab";
            if (!Directory.Exists(prefabFolder))
                Directory.CreateDirectory(prefabFolder);

            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
                AssetDatabase.DeleteAsset(prefabPath);

            GameObject instantiatePrefab = new GameObject(model.name);
            GameObject modelParent = new GameObject("Model");
            modelParent.transform.SetParent(instantiatePrefab.transform);
            GameObject instantiateModel = GameObject.Instantiate(model, modelParent.transform);
            if (instantiateModel.GetComponent<Animator>() != null)
                DestroyImmediate(instantiateModel.GetComponent<Animator>());
            instantiateModel.name = instantiatePrefab.name;
            ProcessItemModel(instantiatePrefab, levelStyle);

            PrefabUtility.CreatePrefab(prefabPath,instantiatePrefab);

            DestroyImmediate(instantiatePrefab);

            Debug.Log("Item Prefab:" + prefabPath + " Generate Complete");
        }

        AssetDatabase.SaveAssets();
    }
    
    void ProcessItemModel(GameObject prefab, enum_Style levelStyle)
    {
        enum_LevelItemType type= (enum_LevelItemType)System.Enum.Parse(typeof(enum_LevelItemType), prefab.name.Split('_')[0]);
        
        LevelItemBase levelItem=  prefab.AddComponent<LevelItemBase>();
        levelItem.m_ItemType = type;
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] matList = new Material[renderers[i].sharedMaterials.Length];
            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                matList[j] = (CreateMaterial(levelStyle, renderers[i].sharedMaterials[j]));
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
    static string PrefabPath(enum_Style style)
    {
            return TEditor.S_AssetDataBaseResources + TResources.ConstPath.S_LeveLItem+"/" + style.ToString();
    }
    public static Material CreateMaterial(enum_Style lsType,Material sharedMaterial=null)
    {
        string folderParent = "Assets/Material/Static/";
        string folderPath = GetMaterialPath(lsType);

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
    static string GetMaterialPath( enum_Style matStyle)
    {
        return "Assets/Material/Static/Item_" + matStyle.ToString() + ".mat";
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
                shader = Shader.Find("Game/Realistic/Diffuse_Texture");
                break;
        }
        if (shader == null)
        {
            Debug.LogWarning("Null Shader Found:" + sType.ToString());
        }
        return shader;
    }
}
