using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PostEffectManager))]
public class EPostEffectManager : Editor {
    PostEffectManager m_target=null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginVertical();
        if (m_target)
        {
            if (GUILayout.Button("Remove Scene Post Effect"))
                DestroyImmediate(m_target);
        }
        else
        {
            if (GUILayout.Button("Add Scene Post Effect"))
            {
                m_target = SceneView.lastActiveSceneView.camera.gameObject.AddComponent<PostEffectManager>();
            }
        }
        GUILayout.EndVertical();

        if (!m_target)
            return;


        if (PostEffectManager.GetPostEffect<PE_BloomSpecific>() != null)
        {
            if (GUILayout.Button("Remove Effect"))
                PostEffectManager.RemoveAllPostEffect();
        }
        else
        {

            if (GUILayout.Button("Add Effect"))
                PostEffectManager.AddPostEffect<PE_BloomSpecific>();
        }
    }
    private void OnDisable()
    {
        if (m_target)
            DestroyImmediate(m_target);
    }
}
