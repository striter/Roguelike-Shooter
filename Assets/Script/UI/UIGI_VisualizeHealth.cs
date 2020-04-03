using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TSpecialClasses;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_VisualizeHealth : UIT_GridItem {
    EntityBase m_AttachEntity;
    Image m_HealthBar1,m_HealthBar2,m_HealthBar3;
    bool b_showItem = false;
    float f_hideCheck;
    Graphic[] m_Graphics;

    ValueLerpSeconds m_HealthLerp;

    public override void Init()
    {
        base.Init();
        m_HealthBar1 = rtf_Container.Find("HealthBar1").GetComponent<Image>();
        m_HealthBar2 = rtf_Container.Find("HealthBar2").GetComponent<Image>();
        m_HealthBar3 = rtf_Container.Find("HealthBar3").GetComponent<Image>();
        m_Graphics = GetComponentsInChildren<Graphic>();
        rtf_RectTransform.anchoredPosition = UIConst.V2_UINumericVisualizeOffset;
    }

    public void Play(EntityBase _attachTo)
    {
        m_AttachEntity = _attachTo;
        OnHide();
        m_HealthLerp = new ValueLerpSeconds(m_AttachEntity.m_Health.F_HealthMaxScale, 1f, .5f, SetHealthValue);
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

        rtf_RectTransform.SetWorldViewPortAnchor(m_AttachEntity.transform.position,CameraController.MainCamera);
        rtf_RectTransform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(m_AttachEntity.transform.position, CameraController.MainCamera.transform.position) / 30, 1, 3);
        transform.SetActivate(true);
        b_showItem = true;
    }

    private void Update()
    {
        if (!b_showItem)
            return;

        m_HealthLerp.ChangeValue(m_AttachEntity.m_Health.F_HealthMaxScale);
        m_HealthLerp.TickDelta(Time.deltaTime);
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
        m_HealthBar1.fillAmount = value>1?1:value;
        m_HealthBar2.fillAmount = value>2?1:value-1;
        m_HealthBar3.fillAmount = value>3?1:value-2;
    }
}
