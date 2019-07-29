using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PostEffectManager))]
public class EPostEffectManager : Editor {
    PostEffectManager m_target = null;
    public enum enum_PostEffect
    {
        BloomSpecific=1,
        FogDepthNoise=2,
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
            return;

        if(!m_target)
            m_target = SceneView.lastActiveSceneView.camera.gameObject.AddComponent<PostEffectManager>();
        GUILayout.BeginVertical();

        if (PostEffectManager.GetPostEffect<PE_BloomSpecific>() != null)
        {
            if (GUILayout.Button("BloomSpecific_Unview"))
                PostEffectManager.RemovePostEffect<PE_BloomSpecific>();
        }
        else
        {
            if (GUILayout.Button("BloomSpecifc_Preview"))
                PostEffectManager.AddPostEffect<PE_BloomSpecific>();
        }
        GUILayout.EndVertical();
        SceneView.lastActiveSceneView.Repaint();
    }
    private void OnDisable()
    {
        if (m_target)
            DestroyImmediate(m_target);
    }
}
