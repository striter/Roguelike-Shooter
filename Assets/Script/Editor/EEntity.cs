using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EntityEnermyBase))]
public class EEntity : Editor {
    PreviewRenderUtility m_Preview;
    GameObject m_PreviewObject;
    Animator m_PreviewAnimator;
    private void OnEnable()
    {
        EditorApplication.update += Repaint;
        if (HasPreviewGUI())
        {
            m_Preview = new PreviewRenderUtility();
            m_Preview.camera.fieldOfView = 30.0f;
            m_Preview.camera.nearClipPlane = 0.3f;
            m_Preview.camera.farClipPlane = 1000;
            m_PreviewObject = m_Preview.InstantiatePrefabInScene(((Component)target).gameObject);
            m_PreviewObject.SetActive(true);
            m_PreviewObject.transform.position = Vector3.zero;
            m_PreviewAnimator = m_PreviewObject.GetComponentInChildren<Animator>();
            m_PreviewAnimator.updateMode = AnimatorUpdateMode.Normal;
        }
    }
    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
        if (HasPreviewGUI())
        {
            m_Preview.Cleanup();
            m_Preview = null;
            m_PreviewObject = null;
            m_PreviewAnimator = null;
        }
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        m_Preview.BeginStaticPreview(r);
        m_Preview.camera.transform.position = new Vector3(0, 8, -8);
        m_Preview.camera.transform.LookAt(m_PreviewObject.transform);
        m_PreviewObject.transform.Rotate(0,1f,0);
        m_PreviewAnimator.Update(Time.deltaTime);
        m_Preview.camera.Render();
        GUI.DrawTexture(r, m_Preview.EndStaticPreview());
    }
    public override GUIContent GetPreviewTitle()
    {
        return new GUIContent("Entity Animation Preview");
    }
    public override bool HasPreviewGUI()
    {
        return AssetDatabase.IsNativeAsset(target);
    }
}
