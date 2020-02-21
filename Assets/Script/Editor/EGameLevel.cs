using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameLevelManager))]
public class EGameLevel : Editor
{
    GameLevelManager m_GameLevel;
    enum_PreviewType m_Preview= enum_PreviewType.Invalid;
    float m_PreviewScale = 1;
    public enum enum_PreviewType
    {
        Invalid=-1,
        GameLevel,
        Fog,
    }
    private void OnEnable()
    {
        m_GameLevel = target as GameLevelManager;
        EditorApplication.update += Update;
    }
    void Update()
    {
        if(m_Preview== enum_PreviewType.Fog)
        EditorUtility.SetDirty(this);
    }
    private void OnDisable()
    {
        m_Preview = enum_PreviewType.Invalid;
        m_GameLevel = null;
        m_PreviewScale = 1;
        EditorApplication.update -= Update;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying)
            return;

        m_Preview = (enum_PreviewType)EditorGUILayout.EnumPopup("Preview Type:", m_Preview);
        m_PreviewScale = EditorGUILayout.FloatField("Preview Scale:", m_PreviewScale);
    }
    public override bool HasPreviewGUI()
    {
        return m_Preview!= enum_PreviewType.Invalid;
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
        Texture targetTexture = m_Preview == enum_PreviewType.Fog ? m_GameLevel.m_FogTexture : m_GameLevel.m_MapTexture;
        Vector2 textureSize = new Vector2(targetTexture.width, targetTexture.height)* m_PreviewScale;
        Rect mapRect = new Rect(r.position+r.size/2-textureSize/2,textureSize);
        GUI.DrawTexture(mapRect, targetTexture);
    }
}
