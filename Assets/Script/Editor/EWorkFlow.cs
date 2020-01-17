
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
    public static void StyleColorSerialization()=> (GetWindow(typeof(EWorkFlow_StyleColorCustomization)) as EWorkFlow_StyleColorCustomization).Show(); 
    static StyleColorData m_EditingData;
    string extraName="Please Edit This";

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "2_Game"||!EditorApplication.isPlaying)
        {
            GUILayout.TextArea("This Work Flow Can Only Activated While 2_Game Playing");
            GUILayout.EndVertical();
            return;
        }

        enum_LevelStyle style = GameManager.Instance.m_GameLevel.m_GameStyle;
        StyleColorData[] customizations = TResources.GetAllStyleCustomization(style);
        Light directionalLight = GameLevelManager.Instance.m_DirectionalLight;
        Camera camera = CameraController.Instance.m_Camera;

        GUILayout.TextArea(customizations.Length == 0 ? "Current Style Does Not Contains Any Customization,Please Save One" : "Current Style Customizations");
        if (m_EditingData==null)
        {
            customizations.Traversal((StyleColorData data) =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.TextArea(data.name);
                if (GUILayout.Button("Edit"))
                {
                    m_EditingData = data;
                    data.DataInit(directionalLight, CameraController.Instance.m_Camera);
                    extraName = data.name;
                    EditorGUIUtility.PingObject(data);
                }
                if (GUILayout.Button("Delete"))
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(data));
                GUILayout.EndHorizontal();
            });

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New"))
                m_EditingData = StyleColorData.Default();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("Directional Light:");
            m_EditingData.c_lightColor = EditorGUILayout.ColorField(m_EditingData.c_lightColor);
            EditorGUILayout.TextArea("Intensity:");
            m_EditingData.f_lightItensity = EditorGUILayout.Slider(m_EditingData.f_lightItensity, 0, 2);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("Pitch:");
            m_EditingData.f_pitch = EditorGUILayout.Slider(m_EditingData.f_pitch,0,90);
            EditorGUILayout.TextArea("Yaw:");
            m_EditingData.f_yaw = EditorGUILayout.Slider(m_EditingData.f_yaw,0,360);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("Ambient");
            m_EditingData.c_ambient = EditorGUILayout.ColorField(RenderSettings.ambientSkyColor);
            EditorGUILayout.TextArea("Sky Color");
            m_EditingData.c_skyColor = EditorGUILayout.ColorField(m_EditingData.c_skyColor);
            EditorGUILayout.TextArea("Lambert:");
            m_EditingData.f_lambert = EditorGUILayout.Slider(m_EditingData.f_lambert, 0, 1);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("File Name");
            extraName = EditorGUILayout.TextField(extraName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            m_EditingData.DataInit(directionalLight, camera);
            
            if (GUILayout.Button(AssetDatabase.IsMainAsset(m_EditingData) ? "Override" : "Save"))
            { 
                SaveCustomization(m_EditingData, style, extraName);
                m_EditingData = null;
            }

            if (GUILayout.Button("Cancel"))
                m_EditingData = null;
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    static void SaveCustomization(StyleColorData data,enum_LevelStyle selectingStyleType,string dataName)
    {
        string dataFolder = TResources.ConstPath.S_StyleCustomization + "/" + selectingStyleType;
        string targetPath = dataFolder + "/"+dataName + ".asset";
        if (!AssetDatabase.IsMainAsset(data))
        {
            if (!Directory.Exists(TEditor.S_AssetDataBaseResources + dataFolder))
                Directory.CreateDirectory(TEditor.S_AssetDataBaseResources + dataFolder);
            AssetDatabase.CreateAsset(data, TEditor.S_AssetDataBaseResources + targetPath);
        }
        else
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(data),  dataName);
        }


        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
