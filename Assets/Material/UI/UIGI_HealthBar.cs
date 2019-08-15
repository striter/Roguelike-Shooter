using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIGI_HealthBar : UIT_GridItem {
    EntityBase m_AttachEntity;
    Text m_Name;
    Slider m_HealthBar;
    bool b_showItem = false;
    float f_hideCheck;
    Graphic[] m_Graphics;
    protected override void Init()
    {
        base.Init();
        m_Name = tf_Container.Find("Name").GetComponent<Text>();
        m_HealthBar = tf_Container.Find("HealthBar").GetComponent<Slider>();
        m_Graphics = GetComponentsInChildren<Graphic>();
    }

    public void AttachItem(EntityBase _attachTo)
    {
        m_AttachEntity = _attachTo;
        m_Name.text = _attachTo.I_PoolIndex.ToString();
        OnHide();
    }

    public void OnHide()
    {
        transform.SetActivate(false);
        b_showItem = false;
    }

    public void OnShow()
    {
        m_Graphics.Traversal((Graphic graphic) => { graphic.color = TCommon.ColorAlpha(graphic.color, 1f); });

        f_hideCheck = 2f;
        if (b_showItem)
            return;

        m_HealthBar.value = m_AttachEntity.m_HealthManager.F_EHPScale;
        rtf_RectTransform.position = CameraController.MainCamera.WorldToScreenPoint(m_AttachEntity.tf_Head.position);
        rtf_RectTransform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(m_AttachEntity.tf_Head.position, CameraController.MainCamera.transform.position) / 30, 1, 3);
        transform.SetActivate(true);
        b_showItem = true;
    }
    private void Update()
    {
        if (!b_showItem)
            return;

        m_HealthBar.value = Mathf.Lerp(m_HealthBar.value, m_AttachEntity.m_HealthManager.F_EHPScale, Time.deltaTime * 20);

        rtf_RectTransform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(m_AttachEntity.tf_Head.position, CameraController.MainCamera.transform.position) / 20, 1, 3);
        rtf_RectTransform.position = Vector3.Lerp(rtf_RectTransform.position, CameraController.MainCamera.WorldToScreenPoint(m_AttachEntity.tf_Head.position), Time.deltaTime * 20);

        f_hideCheck -= Time.deltaTime;
        if (f_hideCheck < 1f)
            m_Graphics.Traversal((Graphic graphic)=> { graphic.color = TCommon.ColorAlpha(graphic.color,f_hideCheck); });

        if (f_hideCheck < 0)
            OnHide();
    }
}
