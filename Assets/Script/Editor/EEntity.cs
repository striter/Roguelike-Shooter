using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EntityEnermyBase))]
public class EEntity : Editor {
    PreviewRenderUtility m_Preview;
    GameObject m_PreviewObject;
    private void OnEnable()
    {
        if (HasPreviewGUI())
        {
            m_Preview = new PreviewRenderUtility();
            m_Preview.camera.fieldOfView = 30.0f;
            m_Preview.camera.allowHDR = false;
            m_Preview.camera.allowMSAA = false;
            m_Preview.ambientColor = new Color(.1f, .1f, .1f, 0);
            m_Preview.lights[0].intensity = 1.4f;
            m_Preview.lights[0].transform.rotation = Quaternion.Euler(40f, 40f, 0);
            m_Preview.lights[1].intensity = 1.4f;
            m_Preview.camera.transform.position = new Vector3(1, 1f, -6);


            m_PreviewObject = m_Preview.InstantiatePrefabInScene(((Component)target).gameObject);
            m_PreviewObject.SetActive(true);
            m_PreviewObject.transform.position = Vector3.zero;
        }
    }
    private void OnDisable()
    {
        if (HasPreviewGUI())
        {
            m_Preview.Cleanup();
            m_Preview = null;
            m_PreviewObject = null;
        }
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        m_Preview.BeginStaticPreview(r);
        m_Preview.camera.transform.position = -(Vector3.forward * 8f);
        m_Preview.camera.transform.rotation = Quaternion.identity;
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
