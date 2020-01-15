
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
        selectingStyleType = enum_LevelStyle.Invalid;
        previousData = StyleColorData.Default();
        Init();
        previousData.SaveData(directionalLight);
        window.Show();
    }
    static enum_LevelStyle selectingStyleType = enum_LevelStyle.Invalid;
    static bool newStyleData = false;
    static Light directionalLight;
    static StyleColorData previousData;
    string extraName="Please Edit This";
    static void Init()
    {
        if (EditorApplication.isPlaying)
            directionalLight = GameLevelManager.Instance.m_DirectionalLight;
        else
            directionalLight = GameObject.Find("Enviorment/Directional Light").GetComponent<Light>();
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (EditorSceneManager.GetActiveScene().name != "2_Game")
        {
            GUILayout.TextArea("This Work Flow Can Only Activated At Scene: Game");
            GUILayout.EndVertical();
            return;
        }

        Init();

        if (selectingStyleType == enum_LevelStyle.Invalid)
            TCommon.TraversalEnum((enum_LevelStyle style) =>
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
                    selectingStyleType = enum_LevelStyle.Invalid;
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
