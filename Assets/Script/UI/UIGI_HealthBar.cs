using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIGI_HealthBar : UIT_GridItem {
    EntityBase m_AttachEntity;
    Image m_HealthBar1,m_HealthBar2,m_HealthBar3;
    bool b_showItem = false;
    float f_hideCheck;
    Graphic[] m_Graphics;
    public override void Init()
    {
        base.Init();
        m_HealthBar1 = tf_Container.Find("HealthBar1").GetComponent<Image>();
        m_HealthBar2 = tf_Container.Find("HealthBar2").GetComponent<Image>();
        m_HealthBar3 = tf_Container.Find("HealthBar3").GetComponent<Image>();
        m_Graphics = GetComponentsInChildren<Graphic>();
    }

    public void AttachItem(EntityBase _attachTo)
    {
        m_AttachEntity = _attachTo;
        OnHide();
    }

    public void OnHide()
    {
        transform.SetActivate(false);
        b_showItem = false;
    }

    float m_currnetHealthValue;
    public void OnShow()
    {
        m_Graphics.Traversal((Graphic graphic) => { graphic.color = TCommon.ColorAlpha(graphic.color, 1f); });

        f_hideCheck = 2f;
        if (b_showItem)
            return;
        SetHealthValue(m_AttachEntity.m_Health.F_HealthBaseScale);
        rtf_RectTransform.SetWorldViewPortAnchor(m_AttachEntity.transform.position,CameraController.MainCamera);
        rtf_RectTransform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(m_AttachEntity.transform.position, CameraController.MainCamera.transform.position) / 30, 1, 3);
        transform.SetActivate(true);
        b_showItem = true;
    }

    private void Update()
    {
        if (!b_showItem)
            return;

        SetHealthValue(Mathf.Lerp(m_currnetHealthValue,  m_AttachEntity.m_Health.F_HealthBaseScale,Time.deltaTime*5));

        rtf_RectTransform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(m_AttachEntity.transform.position, CameraController.MainCamera.transform.position) / 20, 1, 3);
        rtf_RectTransform.SetWorldViewPortAnchor(m_AttachEntity.transform.position, CameraController.MainCamera, Time.deltaTime*20f);

        f_hideCheck -= Time.deltaTime;
        if (f_hideCheck < 1f)
            m_Graphics.Traversal((Graphic graphic)=> { graphic.color = TCommon.ColorAlpha(graphic.color,f_hideCheck); });

        if (f_hideCheck < 0)
            OnHide();
    }

    void SetHealthValue(float value)
    {
        m_HealthBar1.fillAmount = value<1?value:1;
        m_HealthBar2.fillAmount = value<2?value-1:1;
        m_HealthBar3.fillAmount = value<3?value-2:1;
        m_currnetHealthValue = value;
    }
}
