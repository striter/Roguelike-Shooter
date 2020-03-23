
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelChunkBase),true)]
public class ELevelChunk : Editor
{
    const float m_PreviewScale = 1;
    LevelChunkBase m_GameLevel;
    private void OnEnable()
    {
        m_GameLevel = target as LevelChunkBase;
    }

    public override bool HasPreviewGUI()
    {
        return EditorApplication.isPlaying&&m_GameLevel.m_TerrainMapping;
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
        Texture targetTexture = m_GameLevel.m_TerrainMapping;
        Vector2 textureSize = new Vector2(targetTexture.width, targetTexture.height) * m_PreviewScale;
        Rect mapRect = new Rect(r.position + r.size / 2 - textureSize / 2, textureSize);
        GUI.DrawTexture(mapRect, targetTexture);
    }
}
