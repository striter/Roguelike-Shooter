using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TSpecialClasses;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_VisualizeHealth : UIT_GridItem {
    EntityBase m_AttachEntity;
    Image m_HealthBar;
    bool b_showItem = false;
    float f_hideCheck;
    Graphic[] m_Graphics;

    ValueLerpSeconds m_HealthLerp;

    public override void OnInitItem()
    {
        base.OnInitItem();
        m_HealthBar = rtf_Container.Find("HealthBar").GetComponent<Image>();
        m_Graphics = GetComponentsInChildren<Graphic>();
        rtf_RectTransform.anchoredPosition = UIConst.V2_UINumericVisualizeOffset;
    }

    public void Play(EntityBase _attachTo)
    {
        m_AttachEntity = _attachTo;
        OnHide();
        m_HealthLerp = new ValueLerpSeconds(m_AttachEntity.m_Health.F_HealthMaxScale, 1f, .5f, (float value) => m_HealthBar.fillAmount = value);
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
        //rtf_RectTransform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(m_AttachEntity.transform.position, CameraController.MainCamera.transform.position) / 30, 1, 3);
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
    
}
