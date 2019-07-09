using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EntityEnermyBase))]
public class EEntity : Editor {
    PreviewRenderUtility m_Preview;
    GameObject m_PreviewObject;
    Animator m_PreviewAnimator;
    EntityEnermyBase m_EnermyBase;
    int hs_activate = Animator.StringToHash("t_activate");
    int hs_attack = Animator.StringToHash("t_attack");
    int hs_b_attack = Animator.StringToHash("b_attack");
    int hs_weaponType = Animator.StringToHash("i_weaponType");
    int hs_forward = Animator.StringToHash("f_forward");
    Vector3 v3_center;
    private void OnEnable()
    {
        m_EnermyBase = target as EntityEnermyBase;
        EditorApplication.update += Update;
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
            m_PreviewAnimator.fireEvents = false;
            v3_center = m_PreviewObject.GetComponentInChildren<MeshRenderer>().bounds.center;
            m_Preview.camera.transform.position = v3_center + new Vector3(0,8,6);
            m_Preview.camera.transform.LookAt(m_PreviewObject.transform);
        }
    }
    private void OnDisable()
    {
        EditorApplication.update -= Update;
        if (HasPreviewGUI())
        {
            m_Preview.Cleanup();
            m_Preview = null;
            m_PreviewObject = null;
            m_PreviewAnimator = null;
        }
    }
    Vector2 rotateDelta;
    void Update()
    {
        Repaint();
        m_PreviewAnimator.Update(Time.deltaTime);
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        if (Event.current != null && Event.current.type == EventType.MouseDrag)
            m_PreviewObject.transform.Rotate(new Vector3(0,Event.current.delta.x,0) );
        m_Preview.BeginStaticPreview(r);
        m_Preview.camera.Render();
        if (m_PreviewAnimator.GetInteger(hs_weaponType) != (int)m_EnermyBase.E_AnimatorIndex)
        {
            m_PreviewAnimator.SetInteger(hs_weaponType,(int)m_EnermyBase.E_AnimatorIndex);
            m_PreviewAnimator.SetTrigger(hs_activate);
        }
        GUI.DrawTexture(r, m_Preview.EndStaticPreview());
    }
    public override GUIContent GetPreviewTitle()
    {
        return new GUIContent("Entity Animation Preview");
    }
    public override void OnPreviewSettings()
    {
        if (GUILayout.Button("Attack"))
        {
            m_PreviewAnimator.SetTrigger(hs_attack);
            m_PreviewAnimator.SetBool(hs_b_attack, !m_PreviewAnimator.GetBool(hs_b_attack));
        }
        if (GUILayout.Button("Walk"))
        {
            m_PreviewAnimator.SetFloat(hs_forward, m_PreviewAnimator.GetFloat(hs_forward) == 1 ? 0 : 1);
        }
    }
    public override bool HasPreviewGUI()
    {
        return AssetDatabase.IsNativeAsset(target);
    }
}
