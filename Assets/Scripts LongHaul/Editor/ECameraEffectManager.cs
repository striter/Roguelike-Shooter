using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraEffectManager))]
public class ECameraEffectManager : Editor {
    CameraEffectManager m_target = null;
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
            m_target = SceneView.lastActiveSceneView.camera.gameObject.AddComponent<CameraEffectManager>();
        GUILayout.BeginVertical();

        if (CameraEffectManager.GetCameraEffect<PE_BloomSpecific>() != null)
        {
            if (GUILayout.Button("BloomSpecific_Unview"))
                CameraEffectManager.RemoveCameraEffect<PE_BloomSpecific>();
        }
        else
        {
            if (GUILayout.Button("BloomSpecifc_Preview"))
                CameraEffectManager.AddCameraEffect<PE_BloomSpecific>();
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
