﻿
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
    static GameRenderData m_EditingData;
    string extraName="Please Edit This";

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "LevelEditor"||!EditorApplication.isPlaying)
        {
            GUILayout.TextArea("This Work Flow Can Only Activated While LevelEditor Playing");
            GUILayout.EndVertical();
            return;
        }

        enum_BattleStyle style = LevelChunkEditor.Instance.m_EditStyle;
        GameRenderData[] customizations = TResources.GetRenderData(style);
        Light directionalLight = LevelChunkEditor.Instance.m_DirectionalLight;
        Camera camera = CameraController.Instance.m_Camera;
        if(style== enum_BattleStyle.Invalid)
        {
            GUILayout.TextArea("Set Style In Edit Style View Mode");
            GUILayout.EndVertical();
            return;
        }


        GUILayout.TextArea(customizations.Length == 0 ? "Current Style Does Not Contains Any Customization,Please Save One" : ("Current Style:"+style.ToString()));
        if (m_EditingData==null)
        {
            customizations.Traversal((GameRenderData data) =>
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
                m_EditingData = GameRenderData.Default();
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
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("Shadow Color");
            m_EditingData.c_shadowColor = EditorGUILayout.ColorField(m_EditingData.c_shadowColor);
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

    static void SaveCustomization(GameRenderData data,enum_BattleStyle selectingStyleType,string dataName)
    {
        string dataFolder = TResources.ConstPath.S_ChunkRender + "/" + selectingStyleType;
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
